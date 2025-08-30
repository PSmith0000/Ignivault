using System.ComponentModel.DataAnnotations;

namespace ignivault.Data.Models.Auth
{
    public class ForgotPasswordModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
