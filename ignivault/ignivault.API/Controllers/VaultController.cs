using ignivault.API.Core.Interface;
using ignivault.API.Models;
using ignivault.API.Models.Records;
using ignivault.API.Security;
using ignivault.API.Security.Auth;
using ignivault.API.Services;
using ignivault.API.SQL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System.Security.Claims;

public record VaultItemDto(int Id, string Name, string Type, DateTime CreatedAt, DateTime? UpdatedAt);

// Your controller code:
namespace ignivault.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Verified")]
    public class VaultController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly UserManager<LoginUser> _userManager;
        private readonly UserActivityService _activityService;
        private readonly ICryptService _cryptService;

        public VaultController(AppDbContext db, UserManager<LoginUser> userManager, UserActivityService userActivityService, ICryptService cryptService)
        {
            _db = db;
            _userManager = userManager;
            _activityService = userActivityService;
            _cryptService = cryptService;
        }

        [HttpGet("myvault")]
        public async Task<IActionResult> GetVaultData()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            try
            {
                var items = await _db.VaultItems
                    .Where(v => v.UserId == userId)
                    .Select(v => new VaultItem
                    {
                        Id = v.Id,
                        Name = v.Name,
                        Type = v.Type,
                        CreatedAt = v.CreatedAt,
                        UpdatedAt = v.UpdatedAt,
                        UserId = v.UserId,
                        IV = v.IV,
                        EncryptedData = v.Type == "File" ? new byte[] { 0x0 } : v.EncryptedData
                    }).ToListAsync();

                return Ok(new { success = true, data = items });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching vault: {ex.Message}");
                return StatusCode(500, "Failed to fetch vault items.");
            }
        }

        [HttpGet("download-file")]
        public async Task<IActionResult> DownloadFile([FromQuery] int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // OPTIMIZED: Select only the fields needed, reducing data transfer from DB.
            var fileData = await _db.VaultItems
                .Where(i => i.Id == id && i.UserId == userId && i.Type == "File")
                .Select(i => new { i.Name, i.EncryptedData })
                .FirstOrDefaultAsync();

            if (fileData == null) return NotFound();

            // NOTE: Prepending the item ID to the file bytes is an unusual pattern.
            // If not strictly required by your client, consider removing it and just returning fileData.EncryptedData.
            byte[] recordIdBytes = BitConverter.GetBytes(id);
            byte[] responseData = recordIdBytes.Concat(fileData.EncryptedData).ToArray();

            return File(responseData, "application/octet-stream", fileData.Name ?? $"vaultfile-{id}");
        }

        [RequestSizeLimit(60_000_000)]
        [HttpPost("add")]
        public async Task<IActionResult> AddVaultItem([FromBody] VaultItem model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            model.UserId = userId;
            model.CreatedAt = DateTime.UtcNow;

            _db.Add(model);
            await _activityService.LogActivityAsync(userId, "Add Item", $"Added vault item '{model.Name}'");

            await _db.SaveChangesAsync();

            return Ok(new { success = true, message = "Vault item added successfully.", newItemId = model.Id });
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteVaultItem([FromQuery] int itemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var rowsAffected = await _db.VaultItems
                .Where(v => v.Id == itemId && v.UserId == userId)
                .ExecuteDeleteAsync();

            if (rowsAffected == 0) return NotFound();

            await _activityService.LogActivityAsync(userId, "Delete Item", $"Deleted vault item with ID {itemId}");
            await _db.SaveChangesAsync();

            return Ok("Vault item deleted.");
        }

        /// <summary>
        /// Updates a vault item's mutable properties.
        /// </summary>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateVaultItem([FromBody] VaultItem model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var rowsAffected = await _db.VaultItems
                .Where(v => v.Id == model.Id && v.UserId == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(v => v.Name, model.Name)
                    .SetProperty(v => v.IV, model.IV)
                    .SetProperty(v => v.EncryptedData, model.EncryptedData)
                    .SetProperty(v => v.UpdatedAt, DateTime.UtcNow)
                );

            if (rowsAffected == 0) return NotFound();

            await _activityService.LogActivityAsync(userId, "Update Item", $"Updated vault item with ID {model.Id}");
            await _db.SaveChangesAsync();

            return Ok("Vault item updated successfully.");
        }

        /// <summary>
        /// Resets the user's vault key, re-encrypting all data or deleting it.
        /// </summary>
        [HttpPost("vault-key-reset")]
        public async Task<IActionResult> VaultKeyReset([FromBody] KeyResetModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();

            var hasCurrentKey = !string.IsNullOrWhiteSpace(model.CurrentKey);

            var newMasterKey = _cryptService.GenerateRandomKey();
            var newSalt = _cryptService.GenerateSalt();
            var newKhk = _cryptService.DeriveKey(model.NewKey, newSalt);
            var (encryptedMasterKey, masterIv) = _cryptService.Encrypt(newMasterKey, newKhk);

            byte[]? currentMasterKey = null;
            if (hasCurrentKey)
            {
                try
                {
                    var khk = _cryptService.DeriveKey(model.CurrentKey, user.KeySalt);
                    currentMasterKey = _cryptService.Decrypt(user.EncryptedMasterKey, khk, user.MasterIV);
                }
                catch { return BadRequest(new { Success = false, Message = "Vault decryption failed." }); }
            }

            await using var tx = await _db.Database.BeginTransactionAsync();

            if (hasCurrentKey && currentMasterKey != null)
            {
                IAsyncEnumerable<VaultItem> vaultItems = _db.VaultItems
                    .Where(v => v.UserId == userId)
                    .AsAsyncEnumerable();

                int processedCount = 0;
                await foreach (var item in vaultItems)
                {
                    var decrypted = _cryptService.Decrypt(item.EncryptedData, currentMasterKey, item.IV);
                    var (newEncrypted, newIv) = _cryptService.Encrypt(decrypted, newMasterKey);
                    item.EncryptedData = newEncrypted;
                    item.IV = newIv;

                    processedCount++;
                    if (processedCount % 100 == 0)
                    {
                        await _db.SaveChangesAsync();
                    }
                }
            }
            else
            {
                await _db.VaultItems.Where(v => v.UserId == userId).ExecuteDeleteAsync();
            }

            user.EncryptedMasterKey = encryptedMasterKey;
            user.MasterIV = masterIv;
            user.KeySalt = newSalt;
            await _userManager.UpdateAsync(user);

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            var actionMessage = hasCurrentKey ? "Vault key successfully changed." : "Vault key reset. All vault data deleted.";
            await _activityService.LogActivityAsync(userId, "Vault Key Reset", actionMessage);

            return Ok(new { Success = true, Message = actionMessage });
        }

        [HttpGet("storage-report")]
        public async Task<ActionResult<IEnumerable<VaultStorageMonthlyDto>>> GetVaultStorageReport()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            DateTime start = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(-11);

            var grouped = await _db.VaultItems
                .Where(v => v.UserId == userId && v.CreatedAt >= start)
                .GroupBy(v => new { v.CreatedAt.Year, v.CreatedAt.Month })
                .Select(g => new VaultStorageMonthlyDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    ItemCount = g.Count(),
                    TotalSizeKb = g.Sum(v => (double)(v.EncryptedData.Length + v.IV.Length)) / 1024.0,
                    AverageItemSizeKb = g.Average(v => (double)(v.EncryptedData.Length + v.IV.Length)) / 1024.0
                })
                .ToListAsync();

            var groupedDict = grouped.ToDictionary(g => (g.Year, g.Month));
            var results = new List<VaultStorageMonthlyDto>();
            for (int i = 0; i < 12; i++)
            {
                var month = start.AddMonths(i);
                if (groupedDict.TryGetValue((month.Year, month.Month), out var existing))
                {
                    existing.TotalSizeKb = Math.Round(existing.TotalSizeKb, 2);
                    existing.AverageItemSizeKb = Math.Round(existing.AverageItemSizeKb, 2);
                    results.Add(existing);
                }
                else
                {
                    results.Add(new VaultStorageMonthlyDto
                    {
                        Year = month.Year,
                        Month = month.Month,
                        TotalSizeKb = 0,
                        AverageItemSizeKb = 0,
                        ItemCount = 0
                    });
                }
            }
            return Ok(results.OrderBy(r => r.Year).ThenBy(r => r.Month));
        }
    }
}