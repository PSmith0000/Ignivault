using ignivault.API.Models;
using ignivault.API.Security;
using ignivault.API.Security.Auth;
using ignivault.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ignivault.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<LoginUser> _userManager;
        private readonly SignInManager<LoginUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly UserActivityService _activityService;
        private readonly IEmailService _emailService;

        public AuthenticationController(UserManager<LoginUser> userManager, SignInManager<LoginUser> signInManager, IConfiguration configuration, UserActivityService userActivityService, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _activityService = userActivityService;
            _emailService = emailService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new LoginUser
            {
                UserName = model.Username,
                Email = model.Email
            };

            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(model));

            var masterKey = Crypt.GenerateRandomKey();
            var salt = Crypt.GenerateSalt();
            var KHK = Crypt.DeriveKey(model.EncryptionKey, salt);
            var (encryptedMasterKey, iv) = Crypt.Encrypt(masterKey, KHK);

            user.EncryptedMasterKey = Convert.ToBase64String(encryptedMasterKey);
            user.KeySalt = Convert.ToBase64String(salt);
            user.MasterIV = Convert.ToBase64String(iv);

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _activityService.LogActivityAsync(user.Id, "Registration", "User registered successfully");

            return Ok(new { Message = "User registered successfully!" });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized("Invalid credentials");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return Unauthorized("Invalid credentials");

            var token = GenerateJwtToken(user);
            var loginDto = new LoginUserDto(user, token);

            return Ok(loginDto);
        }

        [HttpGet("userdata")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var dto = new LoginUserDto(user, string.Empty);
            return Ok(dto);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Success = false, Message = "Invalid ModelState" });

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest(new { Success = false, Message = "Invalid Request" });

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _activityService.LogActivityAsync(user.Id, "Password Reset", "User reset their password via email link");

            return Ok(new { Success = true, Message = "Password has been reset successfully." });
        }

        [HttpPost("request-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(new { Success = false, Message = "Email is required" });

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return Ok();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = $"https://localhost:7085/reset-password?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";

            Console.WriteLine($"ResetLink: {resetLink}");

            //await _emailService.SendPasswordResetAsync(user.Email, resetLink);

            return Ok(new { Success = true, Message = "Password reset link sent if the email exists." });
        }

        private string GenerateJwtToken(LoginUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),               
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), //temp (normal 1 hour)
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
