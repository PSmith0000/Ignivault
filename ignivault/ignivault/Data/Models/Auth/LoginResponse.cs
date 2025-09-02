namespace ignivault.Data.Models.Auth
{
    public class LoginResponse
    {
        public string Token { get; set; }

        public string MasterKey { get; set; }

        public string keySalt { get; set; }

        public string MasterIV { get; set; }
    }
}
