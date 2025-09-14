using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ignivault.API.Models.Records
{
    public class UserActivity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string ActivityType { get; set; }

        [Required]
        public DateTime ActivityTime { get; set; } = DateTime.UtcNow;

        public string? Details { get; set; }
    }
}
