using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace ignivault.Data.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Net.Http.Json;
    using System.Runtime.CompilerServices;
    using System.Text.Json;

    public class VaultItem
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public byte[] EncryptedData { get; set; }
        public byte[] Iv { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public enum VaultItemType
        {
            Text,
            File,
            Login
        }
    }

}
