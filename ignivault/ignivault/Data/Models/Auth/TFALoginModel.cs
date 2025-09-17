using ignivault.Data.Models.Auth;

namespace ignivault.Data.Models.Auth
{
    public class TFALoginModel
    {
        public bool Success { get; set; }

        public LoginUser User { get; set; }

        public string Message { get; set; }

        public string token { get; set; }

        public IEnumerable<string> Codes { get; set; }
    }
}
