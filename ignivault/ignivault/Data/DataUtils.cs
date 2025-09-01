using Microsoft.JSInterop;
using System.Text.Json;


namespace ignivault.Data
{
    public class DataUtils
    {
        public static bool IsTokenExpired(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return true;

            var parts = token.Split('.');
            if (parts.Length != 3) return true;

            var payload = parts[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var payloadData = JsonSerializer.Deserialize<JwtPayload>(jsonBytes);

            var exp = DateTimeOffset.FromUnixTimeSeconds(payloadData.Exp);
            return exp <= DateTimeOffset.UtcNow;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            base64 = base64.Replace('-', '+').Replace('_', '/');
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        private class JwtPayload
        {
            public long Exp { get; set; }
        }
    }
}
