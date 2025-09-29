namespace ignivault.WebAPI.Controllers
{
    [Route(ApiEndpoints.Auth.Base)]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<LoginUser> _userManager;
        private readonly SignInManager<LoginUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IUserActivityLogger _userActivityLogger;

        public AuthController(UserManager<LoginUser> userManager, SignInManager<LoginUser> signInManager, IConfiguration configuration, IEmailService emailService, IUserActivityLogger userActivityLogger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailService = emailService;
            _userActivityLogger = userActivityLogger;
        }


        /// <summary>
        /// Registers a new user with the provided email and password.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost(ApiEndpoints.Auth.Register)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null) return BadRequest(new ErrorResponseDto { Message = "User with this email already exists." });

            var salt = RandomNumberGenerator.GetBytes(16);
            var user = new LoginUser { UserName = model.Email, Email = model.Email, KeySalt = salt };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded) return BadRequest(new ErrorResponseDto { Message = "User creation failed.", Errors = result.Errors.Select(e => e.Description) });

            await _userActivityLogger.LogActivityAsync(user.Id, ActivityType.UserRegistered, true, "User registered successfully.");

            return Ok(new { Message = "User created successfully! Please log in." });
        }


        /// <summary>
        /// Logs in a user with the provided email and password.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost(ApiEndpoints.Auth.Login)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized(new ErrorResponseDto { Message = "Invalid email or password." });

            if(user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
            {
                return Unauthorized(new ErrorResponseDto { Message = "Account is locked. Please try again later." });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
            if (!result.Succeeded) {
                await _userManager.AccessFailedAsync(user);
                return Unauthorized(new ErrorResponseDto { Message = "Invalid email or password." });
            }

            await _userManager.ResetAccessFailedCountAsync(user);
            if (user.TwoFactorEnabled)
            {
                return Ok(new LoginResponseDto { Is2faRequired = true });
            }

            var token = await GenerateJwtToken(user);
            return Ok(new LoginResponseDto
            {
                Is2faRequired = false,
                LoginResponse = new LoginResponsePayload
                {
                    Token = token,
                    KeySalt = Convert.ToBase64String(user.KeySalt)
                }
            });
        }


        /// <summary>
        /// Logs in a user using 2FA code.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost(ApiEndpoints.Auth.Login2fa)]
        [AllowAnonymous]
        public async Task<IActionResult> Login2Fa([FromBody] Login2faRequestDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized(new ErrorResponseDto { Message = "Invalid request." });

            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);
            bool isCodeValid = false;

            var result = await _userManager.RedeemTwoFactorRecoveryCodeAsync(user, verificationCode);
            if (result.Succeeded)
            {
                isCodeValid = true;
            }
            else
            {
                isCodeValid = await _userManager.VerifyTwoFactorTokenAsync(user,
                    _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);
            }

            if (!isCodeValid)
            {
                await _userManager.AccessFailedAsync(user);
                return Unauthorized(new ErrorResponseDto { Message = "Invalid 2FA code." });
            }

            await _userManager.ResetAccessFailedCountAsync(user);

            var token = await GenerateJwtToken(user);
            return Ok(new LoginResponsePayload
            {
                Token = token,
                KeySalt = Convert.ToBase64String(user.KeySalt)
            });
        }

        /// <summary>
        /// Enables 2FA for the authenticated user and returns the secret key and QR code URL.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet(ApiEndpoints.Auth.Enable2fa)]
        public async Task<IActionResult> Enable2Fa()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null) return Unauthorized();

            var secretKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(secretKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                secretKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var issuer = _configuration["TwoFactorAuth:Issuer"];
            var uriFormat = _configuration["TwoFactorAuth:TotpUriFormat"];

            var qrCodeUrl = string.Format(uriFormat!,Uri.EscapeDataString(issuer!), Uri.EscapeDataString(user.Email!), secretKey);

            return Ok(new Enable2faResponseDto
            {
                SecretKey = secretKey!,
                QrCodeUrl = qrCodeUrl
            });
        }


        /// <summary>
        /// Verifies the provided 2FA code and enables 2FA for the authenticated user.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost(ApiEndpoints.Auth.Verify2fa)]
        public async Task<IActionResult> VerifyAndEnable2Fa([FromBody] Verify2faRequestDto model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null) return Unauthorized();

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, model.Code);
            if (!isValid) return BadRequest(new ErrorResponseDto { Message = "Verification code is invalid." });

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

            await _userActivityLogger.LogActivityAsync(user.Id, ActivityType.TwoFactorEnabled, true, "2FA enabled successfully.");

            return Ok(new Verify2faResponseDto
            {
                Message = "2FA has been enabled successfully.",
                RecoveryCodes = recoveryCodes!
            });
        }


        /// <summary>
        /// Forgot password - sends a password reset link to the user's email if the account exists.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost(ApiEndpoints.Auth.ForgotPassword)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLinkFormat = _configuration["ClientConfiguration:PasswordResetLinkFormat"];

                var resetLink = string.Format(resetLinkFormat!,Uri.EscapeDataString(user.Email!), Uri.EscapeDataString(token));
                await _emailService.SendPasswordResetEmailAsync(model.Email, resetLink);
            }
            return Ok(new { Message = "If an account with this email exists, a password reset link has been sent." });
        }


        /// <summary>
        /// Resets the user's password using the provided token and new password.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost(ApiEndpoints.Auth.ResetPassword)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return BadRequest(new ErrorResponseDto { Message = "Invalid password reset request." });

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (!result.Succeeded) return BadRequest(new ErrorResponseDto { Message = "Password reset failed.", Errors = result.Errors.Select(e => e.Description) });
            await _userActivityLogger.LogActivityAsync(user.Id, ActivityType.PasswordChanged, true, "Password reset successfully.");
            return Ok(new { Message = "Your password has been reset successfully." });
        }


        /// <summary>
        /// Generates a JWT token for the authenticated user including their roles.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<string> GenerateJwtToken(LoginUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}