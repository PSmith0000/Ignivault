using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ignivault.Shared.DTOs.Auth
{
    /// <summary>
    /// UserProfileDto represents a user's profile information including username, email, and 2FA status.
    /// </summary>
    public class UserProfileDto
    {
        public string Username { get; set; }
        public string Email { get; set; }

        public bool Is2faEnabled { get; set; }
    }

    /// <summary>
    /// UpdatePasswordRequestDto is used to change a user's login password by verifying the old password and providing a new one.
    /// </summary>
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

    /// <summary>
    /// ReEncryptedItemDto represents a vault item that has been re-encrypted with a new master password.
    /// </summary>
    public class ReEncryptedItemDto
    {
        public int ItemId { get; set; }
        [Required]
        public byte[] EncryptedData { get; set; }
        [Required]
        public byte[] Iv { get; set; }
    }

    /// <summary>
    /// UpdateMasterPasswordRequestDto is used to securely update a user's master password.
    /// </summary>
    public class UpdateMasterPasswordRequestDto
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        public byte[] NewSalt { get; set; }

        [Required]
        public List<ReEncryptedItemDto> ReEncryptedItems { get; set; }
    }

    /// <summary>
    /// UserActivityDto represents a log entry for significant actions performed by users or the system for auditing and monitoring purposes.
    /// </summary>
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
