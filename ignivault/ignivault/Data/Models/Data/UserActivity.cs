using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ignivault.Data.Models.Data
{
    public class UserActivity
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ActivityType { get; set; }

        public DateTime ActivityTime { get; set; } = DateTime.UtcNow;

        public string? Details { get; set; }
    }
}
