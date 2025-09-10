using ignivault.API.Models.Records;
using ignivault.API.Security;
using ignivault.API.Security.Auth;
using ignivault.API.SQL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ignivault.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class VaultController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly UserManager<LoginUser> _userManager;
        public VaultController(AppDbContext db, UserManager<LoginUser> userManager) { _db = db; _userManager = userManager; }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("myvault")]
        public async Task<IActionResult> GetVaultData()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized();

            var vaultItems = await _db.VaultItems.Where(v => v.UserId == userId).ToListAsync();

            return Ok(vaultItems);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("add")]
        public async Task<IActionResult> AddVaultItem([FromBody] VaultItem model)
        {     
            VaultItem item = model;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            Console.WriteLine($"USER ID: {userId}");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {               
                return Unauthorized();
            }

            item.UserId = userId;
            item.CreatedAt = DateTime.UtcNow;

            await _db.AddAsync<VaultItem>(item);

            await _db.SaveChangesAsync();

            return Ok("User Vault Updated.");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteVaultItem([FromQuery] int itemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            var item = await _db.VaultItems.FirstOrDefaultAsync(v => v.Id == itemId && v.UserId == userId);
            if (item == null) return NotFound();
            _db.VaultItems.Remove(item);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateVaultItem([FromBody] VaultItem model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            var item = await _db.VaultItems.FirstOrDefaultAsync(v => v.Id == model.Id && v.UserId == userId);
            if (item == null) return NotFound();
            


            await _db.SaveChangesAsync();
            return Ok();
        }

        #region DTOs
        #endregion
    }
}
