using ignivault.API.Models.Records;
using Microsoft.AspNetCore.Identity;

namespace ignivault.API.Security.Auth
{
    public class LoginUser : IdentityUser
    {
        public byte[] EncryptedMasterKey { get; set; }

        public byte[] KeySalt { get; set; }

        public ICollection<VaultItem> VaultItems { get; set; } = new List<VaultItem>();

        public DateTime? LastPasswordChange { get; set; }
    }
}
