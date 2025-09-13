using Microsoft.JSInterop;
using System.Text.Json;


namespace ignivault.Data
{
    public class DataUtils
    {
        public static bool IsTokenExpired(string token)
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            return jwt.ValidTo <= DateTime.UtcNow;
        }

        public static bool AreEqual(byte[]? a1, byte[]? a2)
        {
            if (a1 == null || a2 == null || a1.Length != a2.Length) return false;
            for (int i = 0; i < a1.Length; i++)
                if (a1[i] != a2[i]) return false;
            return true;
        }
    }
}
