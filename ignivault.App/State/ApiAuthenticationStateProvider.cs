using ignivault.ApiClient;
using ignivault.ApiClient.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;
namespace ignivault.App.State
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ITokenManager _tokenManager;
        private readonly IAuthApiClient _authApiClient;

        public ApiAuthenticationStateProvider(ITokenManager tokenManager, IAuthApiClient authApiClient)
        {
            _tokenManager = tokenManager;
            _authApiClient = authApiClient;

            _authApiClient.OnLoginSuccess += NotifyUserAuthentication;
            _authApiClient.OnLogout += NotifyUserLogout;
        }

        /// <summary>
        /// Gets the current authentication state by checking for a valid JWT token.
        /// </summary>
        /// <returns></returns>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _tokenManager.GetTokenAsync();
            var identity = new ClaimsIdentity();

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
                }
                catch
                {
                    await _tokenManager.SetTokenAsync(null);
                    identity = new ClaimsIdentity();
                }
            }

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        /// <summary>
        /// Notifies the application that the user has been authenticated.
        /// </summary>
        /// <returns></returns>
        public Task NotifyUserAuthentication()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            return Task.CompletedTask;
        }

        /// <summary>
        /// Notifies the application that the user has logged out.
        /// </summary>
        /// <returns></returns>
        public Task NotifyUserLogout()
        {
            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Parses claims from a JWT token.
        /// </summary>
        /// <param name="jwt"></param>
        /// <returns></returns>
        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs!.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!));
        }

        /// <summary>
        /// Parses a base64 string that may be missing padding.
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
