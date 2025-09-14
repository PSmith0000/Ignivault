using ignivault.API.Models.Records;
using Microsoft.AspNetCore.Identity;

namespace ignivault.API.Security.Auth
{
    public class LoginUser : IdentityUser
    {
        public string EncryptedMasterKey { get; set; }

        public string KeySalt { get; set; }

        public string MasterIV { get; set; }

        public ICollection<VaultItem> VaultItems { get; set; } = new List<VaultItem>();
        public ICollection<UserActivity> UserActivities { get; set; } = new List<UserActivity>();

        public DateTime? LastPasswordChange { get; set; }
    }
}
