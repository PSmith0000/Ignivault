using ignivault.API.Models.Records;

namespace ignivault.API.Security.Auth
{
    public class LoginUserDto
    {
        public LoginUserDto(LoginUser user, string token, bool twofactor = false) {
           Username = user.UserName ?? "";
           EncryptedMasterKey = user.EncryptedMasterKey ?? new byte[] {0x0};
           KeySalt = user.KeySalt ?? new byte[] { 0x0 };
           MasterIV = user.MasterIV ?? new byte[] { 0x0 };
           LastPasswordChange = user.LastPasswordChange ?? default(DateTime);
           Id = user.Id ?? "";
           Email = user.Email ?? "";
           TwoFactorEnabled = this.Username == null ? true : user.TwoFactorEnabled;
           
           Token = token ?? "";
        }

        public LoginUserDto(LoginUser user)
        {
            Username = "";
            EncryptedMasterKey = new byte[] { 0x0 };
            KeySalt = new byte[] { 0x0 };
            MasterIV = new byte[] { 0x0 };
            LastPasswordChange =  default(DateTime);
            Id = user.Id;
            Email = "";
            TwoFactorEnabled = user.TwoFactorEnabled;

            Token = "";
        }

        public string Token { get; set; }

        public bool TwoFactorEnabled { get; set; }
        public string Id { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }
        public byte[] EncryptedMasterKey { get; set; }

        public byte[] KeySalt { get; set; }

        public byte[] MasterIV { get; set; }

        public DateTime? LastPasswordChange { get; set; }
    }
}
