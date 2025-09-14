using ignivault.API.Models.Records;
using ignivault.API.Security.Auth;
using ignivault.API.SQL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public VaultController(AppDbContext db, UserManager<LoginUser> userManager)
        {
            _db = db;
            _userManager = userManager;
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

            await _db.SaveChangesAsync();
            return Ok("Vault item updated successfully.");
        }
    }
}
