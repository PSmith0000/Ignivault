using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ignivault.Shared.Models
{
    public class VaultModels
    {
        /// <summary>
        /// CredentialType represents a set of login credentials, including URL, username, password, and optional notes.
        /// </summary>
        public record CredentialType
        {
            public string? Url { get; set; }
            public string? Username { get; set; }
            public string? Password { get; set; }
            public string? Notes { get; set; }
        }

        /// <summary>
        /// NoteType represents a simple text note with a title and content.
        /// </summary>
        public record NoteType
        {
            public string? Title { get; set; }
            public string? Content { get; set; }
        }

        /// <summary>
        /// FileType represents a file with its name, extension, and size.
        /// </summary>
        public record FileType
        {
            public string? FileName { get; set; }
            public string? Extension { get; set; }
            public long Size { get; set; }
        }
    }
}
