using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ignivault.Shared.DTOs.Auth
{
    public class ErrorResponseDto
    {
        public string Message { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }

    public class Enable2faResponseDto
    {
        public string SecretKey { get; set; }
        public string QrCodeUrl { get; set; }
    }

    public class Verify2faResponseDto
    {
        public string Message { get; set; }
        public IEnumerable<string> RecoveryCodes { get; set; }
    }

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

    public class LoginResponsePayload
    {
        public string Token { get; set; }
        public string KeySalt { get; set; }
    }

    public class LoginResponseDto
    {
        public bool Is2faRequired { get; set; } = false;
        public LoginResponsePayload? LoginResponse { get; set; }
    }

    public class LoginRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class Login2faRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Code { get; set; }
    }

    public class Verify2faRequestDto
    {
        [Required]
        public string Code { get; set; }
    }

    public class ForgotPasswordRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

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
