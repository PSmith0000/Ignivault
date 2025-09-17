namespace ignivault.Data.Models.Auth
{
    public class TwoFA
    {
        public class TwoFactorSetupResponse
        {
            public string SecretKey { get; set; } = string.Empty;
            public string QrCodeUri { get; set; } = string.Empty;
        }

        public class TwoFactorVerifyModel
        {
            public string Code { get; set; } = string.Empty;
        }
    }
}
