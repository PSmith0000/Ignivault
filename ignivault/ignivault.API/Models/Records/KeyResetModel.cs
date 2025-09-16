using System.ComponentModel.DataAnnotations;

namespace ignivault.API.Models.Records
{
    public class KeyResetModel
    {
        public string? CurrentKey { get; set; }

        [Required]
        public string NewKey { get; set; }
    }
}
