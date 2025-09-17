using ignivault.API.Security.Auth;

namespace ignivault.API.Models
{
    public class _2FALoginModel
    {
        public bool Success { get; set; }

        public LoginUserDto User { get; set; }

        public string Message { get; set; }

        public string token { get; set; }

        public IEnumerable<string> Codes { get; set; }
    }
}
