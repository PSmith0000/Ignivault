using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
namespace ignivault.Services
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly JwtSecurityTokenHandler _tokenHandler = new();

        public JwtAuthenticationStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var identity = GetClaimsPrincipalFromToken(token);
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public void NotifyUserAuthentication(string token)
        {
            var identity = GetClaimsPrincipalFromToken(token);
            var user = new ClaimsPrincipal(identity);

            var authState = Task.FromResult(new AuthenticationState(user));
            NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyUserLogout()
        {
            _localStorage.RemoveItemAsync("authToken");
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymous));
            NotifyAuthenticationStateChanged(authState);
        }

        private ClaimsIdentity GetClaimsPrincipalFromToken(string token)
        {
            try
            {
                var jwt = _tokenHandler.ReadJwtToken(token);
                var claims = jwt.Claims;

                return new ClaimsIdentity(claims, "authToken");
            }
            catch
            {
                return new ClaimsIdentity();
            }
        }
    }

}
