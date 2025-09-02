using ignivault.API.Models.Records;
using ignivault.API.Security;
using ignivault.API.Security.Auth;
using ignivault.API.SQL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ignivault.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] //JWT Authorization required for all endpoints in this controller
    public class VaultController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly UserManager<LoginUser> _userManager;
        public VaultController(AppDbContext db, UserManager<LoginUser> userManager) { _db = db; _userManager = userManager; }


        [HttpGet("myvault")]
        public async Task<IActionResult> GetVaultData()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized();

            var vaultItems = await _db.VaultItems.Where(v => v.UserId == userId).ToListAsync();

            return Ok(vaultItems);
        }


        [HttpPost("add")]
        public async Task<IActionResult> AddVaultItem([FromBody] VaultItemDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

          
            await _db.SaveChangesAsync();

            //return item id
            return Ok();
        }



        #region DTOs
        public class VaultItemDto
        {
           
        }
        #endregion
    }
}
