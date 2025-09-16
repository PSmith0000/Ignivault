using System.ComponentModel.DataAnnotations;

namespace ignivault.Data.Models.Auth
{
    public class KeyResetModel
    {
        public string? CurrentKey { get; set; }

        [Required]
        public string NewKey { get; set; }
    }
}
