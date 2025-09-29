using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ignivault.Shared.DTOs
{
    /// <summary>
    /// User Data Transfer Object (DTO) for transferring user information.
    /// </summary>
    public class UserDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool IsLocked => LockoutEnd.HasValue && LockoutEnd.Value > DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Represents a request to assign or modify a role for a user.
    /// </summary>
    public class RoleRequestDto
    {
        public string RoleName { get; set; }
    }
}
