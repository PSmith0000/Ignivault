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
    }
}
