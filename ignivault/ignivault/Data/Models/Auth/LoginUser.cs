using Microsoft.IdentityModel.Logging;

namespace ignivault.Data.Models.Auth
{
    using ignivault.Data.Models.Data;
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class LoginUser
    {
        public string Token { get; set; } = "";
        public bool TwoFactorEnabled { get; set; }
        public string Id { get; set; } = "";
        public string Email { get; set; } = "";
        public string Username { get; set; } = "";
        public string EncryptedMasterKey { get; set; } = "";
        public string KeySalt { get; set; } = "";
        public string MasterIV { get; set; } = "";
        public DateTime? LastPasswordChange { get; set; }
    }
}
