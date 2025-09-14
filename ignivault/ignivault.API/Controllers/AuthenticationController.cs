using ignivault.API.Models;
using ignivault.API.Security;
using ignivault.API.Security.Auth;
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

        public AuthenticationController(UserManager<LoginUser> userManager, SignInManager<LoginUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new LoginUser
            {
                UserName = model.Username,
                Email = model.Email
            };

            var masterKey = Crypt.GenerateRandomKey();
            var salt = Crypt.GenerateSalt();
            var KHK = Crypt.DeriveKey(model.Password, salt);
            var (encryptedMasterKey, iv) = Crypt.Encrypt(masterKey, KHK);

            user.EncryptedMasterKey = Convert.ToBase64String(encryptedMasterKey);
            user.KeySalt = Convert.ToBase64String(salt);
            user.MasterIV = Convert.ToBase64String(iv);

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

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

        private string GenerateJwtToken(LoginUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
