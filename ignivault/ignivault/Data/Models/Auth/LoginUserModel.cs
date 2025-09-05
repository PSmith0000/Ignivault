using System.ComponentModel.DataAnnotations;

namespace ignivault.Data.Models.Auth
{
    public class LoginUserModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
