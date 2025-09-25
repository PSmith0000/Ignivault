namespace ignivault.WebAPI.Data.Entities
{
    /// <summary>
    /// UserActivity logs significant actions performed by users or the system for auditing and monitoring purposes.
    /// </summary>
    public class UserActivity
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public LoginUser User { get; set; }
        public ActivityType ActivityType { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [StringLength(45)]
        public string IpAddress { get; set; }

        public bool Success { get; set; }

        [StringLength(255)]
        public string? Details { get; set; }
        public string? UserAgent { get; set; }
    }
}
