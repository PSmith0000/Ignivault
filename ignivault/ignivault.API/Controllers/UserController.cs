using ignivault.API.Models;
using ignivault.API.Models.Records;
using ignivault.API.Security.Auth;
using ignivault.API.Services;
using ignivault.API.SQL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ignivault.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme, Policy = "Verified")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<LoginUser> _userManager;
        private readonly SignInManager<LoginUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _db;
        private readonly UserActivityService _activityService;

        public UserController(AppDbContext db, UserManager<LoginUser> userManager, SignInManager<LoginUser> signInManager, IConfiguration configuration, UserActivityService userActivityService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _db = db;
            _activityService = userActivityService;
        }

        [HttpPost("disable-two-factor")]
        public async Task<IActionResult> DisableTwoFactor()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();
            var result = await _userManager.SetTwoFactorEnabledAsync(user, false);
            var resetresult = await _userManager.ResetAuthenticatorKeyAsync(user);
            if (!result.Succeeded && !resetresult.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return BadRequest(new { Success = false, Message = errors });
            }
            await _activityService.LogActivityAsync(user.Id, "2FA Disabled", "User disabled two-factor authentication");
            return Ok(new { Success = true, Message = "Two-factor authentication disabled." });
        }

        [HttpPost("two-factor-setup")]
        public async Task<IActionResult> SetupTwoFactor()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            await _userManager.ResetAuthenticatorKeyAsync(user);

            var key = await _userManager.GetAuthenticatorKeyAsync(user);


            //QR
            var qrCodeUri = $"otpauth://totp/Ignivault:{Uri.EscapeDataString(user.Email)}" + $"?secret={key}&issuer={Uri.EscapeDataString("Ignivault")}&digits=6";

            Console.WriteLine($"2FA: {qrCodeUri}");

            return Ok(new { SecretKey = key, QrCodeUri = qrCodeUri });
        }

        [HttpGet("userdata")]
        public async Task<IActionResult> GetUserProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var dto = new LoginUserDto(user, string.Empty);
            return Ok(dto);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.CurrentPassword) || string.IsNullOrWhiteSpace(model.NewPassword))
                return BadRequest("Invalid data.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();


            if (!await _userManager.CheckPasswordAsync(user, model.CurrentPassword)) return BadRequest("Current password is incorrect.");

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return BadRequest(new { Success = false, Message = errors });
            }

            await _activityService.LogActivityAsync(user.Id, "Password Change", "User changed their password (logged in)");

            return Ok(new { Success = true, Message = "Password changed successfully." });
        }


        [HttpGet("activities")]
        public async Task<IActionResult> GetUserActivity(int limit = 5)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var activities = await _activityService.GetRecentActivitiesAsync(userId, limit);
                return Ok(activities);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user activities: {ex.Message}");
                return StatusCode(500, "Failed to fetch user activities.");
            }
        }
    }
}
