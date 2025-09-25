using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ignivault.Shared.DTOs.Auth
{
    /// <summary>
    /// Error response DTO for conveying error messages and details.
    /// </summary>
    public class ErrorResponseDto
    {
        public string Message { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }


    /// <summary>
    /// Enabled 2FA response DTO containing the secret key and QR code URL.
    /// </summary>
    public class Enable2faResponseDto
    {
        public string SecretKey { get; set; }
        public string QrCodeUrl { get; set; }
    }


    /// <summary>
    /// Verify 2FA response DTO containing a message and recovery codes if applicable.
    /// </summary>
    public class Verify2faResponseDto
    {
        public string Message { get; set; }
        public IEnumerable<string> RecoveryCodes { get; set; }
    }

    /// <summary>
    /// Register request DTO for creating a new user account.
    /// </summary>
    public class RegisterRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; }

        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }


    /// <summary>
    /// Login response payload containing the JWT token and key salt.
    /// </summary>
    public class LoginResponsePayload
    {
        public string Token { get; set; }
        public string KeySalt { get; set; }
    }


    /// <summary>
    /// Login response DTO indicating if 2FA is required and containing the login response payload.
    /// </summary>
    public class LoginResponseDto
    {
        public bool Is2faRequired { get; set; } = false;
        public LoginResponsePayload? LoginResponse { get; set; }
    }


    /// <summary>
    /// Login request DTO for user authentication.
    /// </summary>
    public class LoginRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }


    /// <summary>
    /// Login 2FA request DTO for submitting the 2FA code during login.
    /// </summary>
    public class Login2faRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Code { get; set; }
    }

    /// <summary>
    /// Verify 2FA request DTO for submitting the 2FA code to enable or verify 2FA.
    /// </summary>
    public class Verify2faRequestDto
    {
        [Required]
        public string Code { get; set; }
    }

    /// <summary>
    /// Forgot password request DTO for initiating a password reset.
    /// </summary>
    public class ForgotPasswordRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }


    /// <summary>
    /// Reset password request DTO for completing a password reset with a new password.
    /// </summary>
    public class ResetPasswordRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; }

        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
