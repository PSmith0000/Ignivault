using Microsoft.IdentityModel.Logging;

namespace ignivault.Data.Models.Auth
{
    using ignivault.Data.Models.Data;
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class LoginUser
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("twoFactorEnabled")]
        public bool TwoFactorEnabled { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("encryptedMasterKey")]
        public string EncryptedMasterKey { get; set; }

        [JsonPropertyName("keySalt")]
        public string KeySalt { get; set; }

        [JsonPropertyName("masterIV")]
        public string MasterIV { get; set; }

        [JsonPropertyName("lastPasswordChange")]
        public DateTime? LastPasswordChange { get; set; }
    }
}
