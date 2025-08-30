using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ignivault.Data.Models.Auth
{
    public class RegistrationModel
    {
        [Required, MinLength(4, ErrorMessage = "Password must be at least 4 characters")]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }


        [Required, Compare("Password", ErrorMessage = "Passwords do not match"), Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
