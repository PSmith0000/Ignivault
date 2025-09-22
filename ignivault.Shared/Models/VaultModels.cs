using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ignivault.Shared.Models
{
    public class VaultModels
    {
        public record CredentialType
        {
            public string? Url { get; set; }
            public string? Username { get; set; }
            public string? Password { get; set; }
            public string? Notes { get; set; }
        }

        public record NoteType
        {
            public string? Title { get; set; }
            public string? Content { get; set; }
        }

        public record FileType
        {
            public string? FileName { get; set; }
            public string? Extension { get; set; }
            public long Size { get; set; }
        }
    }
}
