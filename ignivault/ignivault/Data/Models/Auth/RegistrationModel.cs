using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ignivault.Data.Models.Auth
{
    public class RegistrationModel
    {
        [Required, MinLength(4, ErrorMessage = "Username must be atleast 4 charecters.")]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6, ErrorMessage = "Password must be atleast 6 charecters.")]
        public string Password { get; set; }

        [Required, Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required, MinLength(6, ErrorMessage = "Vault password must be at least 6 characters.")]
        public string EncryptionKey { get; set; } = string.Empty;

        [Required, Compare("EncryptionKey", ErrorMessage = "Keys do not match.")]
        public string EncryptionKeyConfirm { get; set; } = string.Empty;
    }
}
