using ignivault.API.Models.Records;

namespace ignivault.API.Security.Auth
{
    public class LoginUserDto
    {
        public LoginUserDto(LoginUser user, string token) {
           Username = user.UserName;
           EncryptedMasterKey = user.EncryptedMasterKey;
           KeySalt = user.KeySalt;
           MasterIV = user.MasterIV;
           LastPasswordChange = user.LastPasswordChange;
           Id = user.Id;
           Email = user.Email;
           TwoFactorEnabled = user.TwoFactorEnabled;
           
           Token = token;
        }

        public string Token { get; set; }

        public bool TwoFactorEnabled { get; set; }
        public string Id { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }
        public string EncryptedMasterKey { get; set; }

        public string KeySalt { get; set; }

        public string MasterIV { get; set; }

        public DateTime? LastPasswordChange { get; set; }
    }
}
