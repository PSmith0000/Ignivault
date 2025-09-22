using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ignivault.Shared.DTOs.Auth
{
    public class UserProfileDto
    {
        public string Username { get; set; }
        public string Email { get; set; }

        public bool Is2faEnabled { get; set; }
    }

    public class UpdatePasswordRequestDto
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "New password must be at least 8 characters long.")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ReEncryptedItemDto
    {
        public int ItemId { get; set; }
        [Required]
        public byte[] EncryptedData { get; set; }
        [Required]
        public byte[] Iv { get; set; }
    }

    public class UpdateMasterPasswordRequestDto
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        public byte[] NewSalt { get; set; }

        [Required]
        public List<ReEncryptedItemDto> ReEncryptedItems { get; set; }
    }

    public class UserActivityDto
    {
        public string ActivityType { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Success { get; set; }
        public string IpAddress { get; set; }
        public string? Details { get; set; }
        public string DisplayText => $"[{Timestamp.ToLocalTime()}, {ActivityType}]  {Details}";
    }
}
