using ignivault.API.Security.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ignivault.API.Models.Records
{
    public class VaultItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Type { get; set; } // Password/File

        [Required]
        public string Name { get; set; }

        [Required]
        public string EncryptedData { get; set; }

        [Required]
        public string IV { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
