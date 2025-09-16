using ignivault.API.Models;
using ignivault.API.Models.Records;
using ignivault.API.Security;
using ignivault.API.Security.Auth;
using ignivault.API.Services;
using ignivault.API.SQL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System.Security.Claims;

namespace ignivault.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class VaultController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly UserManager<LoginUser> _userManager;
        private readonly UserActivityService _activityService;

        public VaultController(AppDbContext db, UserManager<LoginUser> userManager, UserActivityService userActivityService)
        {
            _db = db;
            _userManager = userManager;
            _activityService = userActivityService;
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
                        EncryptedData = v.Type == "File" ? "NULL" : v.EncryptedData
                    }).ToListAsync();

                return Ok(new { success = true, data = items });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching vault: {ex.Message}");
                return StatusCode(500, "Failed to fetch vault items.");
            }
        }

        [HttpGet("getfile")]
        public async Task<IActionResult> DownloadVaultItem([FromQuery] int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var item = await _db.VaultItems.FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId && i.Type == "File");
            if (item == null) return NotFound();

            byte[] recordId = BitConverter.GetBytes(item.Id);
            byte[] fileBytes = Convert.FromBase64String(item.EncryptedData);
            var data = recordId.Concat(fileBytes).ToArray();

            return File(data, "application/octet-stream", item.Name ?? $"vaultfile-{id}");
        }

        [RequestSizeLimit(450_000_000)]
        [HttpPost("add")]
        public async Task<IActionResult> AddVaultItem([FromBody] VaultItem model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            model.UserId = userId;
            model.CreatedAt = DateTime.UtcNow;

            try
            {
                await _db.AddAsync(model);

                await _activityService.LogActivityAsync(userId, "Add Item", $"Added vault item with name {model.Name} and type {model.Type}");

                await _db.SaveChangesAsync();
                return Ok("Vault item added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding vault item: {ex.Message}");
                return StatusCode(500, "Failed to add vault item.");
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteVaultItem([FromQuery] int itemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var item = await _db.VaultItems.FirstOrDefaultAsync(v => v.Id == itemId && v.UserId == userId);
            if (item == null) return NotFound();

            _db.VaultItems.Remove(item);
            
            await _db.UserActivities.AddAsync(new UserActivity
            {
                UserId = userId,
                ActivityType = "Delete Item",
                ActivityTime = DateTime.UtcNow,
                Details = $"Deleted vault item with ID {itemId}"
            });

            await _db.SaveChangesAsync();

            return Ok("Vault item deleted.");
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateVaultItem([FromBody] VaultItem model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var item = await _db.VaultItems.FirstOrDefaultAsync(v => v.Id == model.Id && v.UserId == userId);
            if (item == null) return NotFound();

            item.Name = model.Name;
            item.IV = model.IV;
            item.EncryptedData = model.EncryptedData;
            item.UpdatedAt = DateTime.UtcNow;

            await _db.UserActivities.AddAsync(new UserActivity
            {
                UserId = userId,
                ActivityType = "Update Item",
                ActivityTime = DateTime.UtcNow,
                Details = $"Updated vault item with ID {model.Id}"
            });

            await _db.SaveChangesAsync();
            return Ok("Vault item updated successfully.");
        }

        [HttpPost("vault-key-reset")]
        public async Task<IActionResult> VaultKeyReset([FromBody] KeyResetModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();

            var hasCurrentKey = !string.IsNullOrWhiteSpace(model.CurrentKey);

            var newMasterKey = Crypt.GenerateRandomKey();
            var newSalt = Crypt.GenerateSalt();
            var newKhk = Crypt.DeriveKey(model.NewKey, newSalt);
            var (encryptedMasterKey, masterIv) = Crypt.Encrypt(newMasterKey, newKhk);

            byte[]? currentMasterKey = null;

            if (hasCurrentKey)
            {
                try
                {
                    var khk = Crypt.DeriveKey(model.CurrentKey,
                                Convert.FromBase64String(user.KeySalt));
                    currentMasterKey = Crypt.Decrypt(
                        Convert.FromBase64String(user.EncryptedMasterKey),
                        khk,
                        Convert.FromBase64String(user.MasterIV));
                }
                catch
                {
                    return BadRequest(new { Success = false, Message = "Vault decryption failed." });
                }
            }

            await using var tx = await _db.Database.BeginTransactionAsync().ConfigureAwait(false);

            if (hasCurrentKey)
            {
                await foreach (var item in _db.VaultItems.Where(v => v.UserId == userId).AsNoTracking().AsAsyncEnumerable().ConfigureAwait(false))
                {
                    try
                    {
                        var decryptedData = Crypt.Decrypt(
                                Convert.FromBase64String(item.EncryptedData),
                                currentMasterKey!,
                                Convert.FromBase64String(item.IV));

                        var (encryptedData, iv) = Crypt.Encrypt(decryptedData, newMasterKey);

                        _db.VaultItems.Attach(item);
                        item.EncryptedData = Convert.ToBase64String(encryptedData);
                        item.IV = Convert.ToBase64String(iv);
                    }
                    catch
                    {
                        return BadRequest(new
                        {
                            Success = false,
                            Message = $"Failed to decrypt vault item '{item.Name}'"
                        });
                    }
                }
            }
            else
            {
                await _db.VaultItems.Where(v => v.UserId == userId).ExecuteDeleteAsync().ConfigureAwait(false);
            }

            user.EncryptedMasterKey = Convert.ToBase64String(encryptedMasterKey);
            user.MasterIV = Convert.ToBase64String(masterIv);
            user.KeySalt = Convert.ToBase64String(newSalt);

            await _userManager.UpdateAsync(user).ConfigureAwait(false);
            await _db.SaveChangesAsync().ConfigureAwait(false);
            await tx.CommitAsync().ConfigureAwait(false);

            var actionMessage = hasCurrentKey
                ? "Vault key successfully changed."
                : "Vault key reset. All vault data deleted.";

            await _activityService.LogActivityAsync(userId, "Vault Key Reset", actionMessage).ConfigureAwait(false);

            return Ok(new { Success = true, Message = actionMessage });
        }






        /// <summary>
        /// Returns vault storage report
        /// </summary>
        [HttpGet("storage-report")]
        public async Task<ActionResult<IEnumerable<VaultStorageMonthlyDto>>> GetVaultStorageReport()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            DateTime start = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(-11);

            //last 12 months
            var query = await _db.VaultItems
                .Where(v => v.UserId == userId && v.CreatedAt >= start)
                .ToListAsync();

            var grouped = query
                .GroupBy(v => new { v.CreatedAt.Year, v.CreatedAt.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g =>
                {
                    //sizeof (EncryptedData + IV)
                    double totalSizeKb = g.Sum(v =>
                    {
                        int bytes = 0;
                        if (!string.IsNullOrEmpty(v.EncryptedData))
                            bytes += System.Text.Encoding.UTF8.GetByteCount(v.EncryptedData);
                        if (!string.IsNullOrEmpty(v.IV))
                            bytes += System.Text.Encoding.UTF8.GetByteCount(v.IV);
                        return bytes / 1024.0;
                    });

                    double avgKb = g.Any() ? totalSizeKb / g.Count() : 0;

                    return new VaultStorageMonthlyDto
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        TotalSizeKb = Math.Round(totalSizeKb, 2),
                        AverageItemSizeKb = Math.Round(avgKb, 2),
                        ItemCount = g.Count()
                    };
                })
                .ToList();

            var results = new List<VaultStorageMonthlyDto>();
            for (int i = 0; i < 12; i++)
            {
                var month = start.AddMonths(i);
                var existing = grouped.FirstOrDefault(x => x.Year == month.Year && x.Month == month.Month);
                results.Add(existing ?? new VaultStorageMonthlyDto
                {
                    Year = month.Year,
                    Month = month.Month,
                    TotalSizeKb = 0,
                    AverageItemSizeKb = 0,
                    ItemCount = 0
                });
            }

            return Ok(results);
        }
    }

}
