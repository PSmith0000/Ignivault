using ignivault.API.Security.Auth;
using System.ComponentModel.DataAnnotations;

namespace ignivault.API.Models.Records
{
    public class VaultItem
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public LoginUser User { get; set; }

        [Required]
        public string Type { get; set; } // Password/File

        [Required]
        public string Name { get; set; }

        [Required]
        public byte[] EncryptedData { get; set; }

        [Required]
        public byte[] IV { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
