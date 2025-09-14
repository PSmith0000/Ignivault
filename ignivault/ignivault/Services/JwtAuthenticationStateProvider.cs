using Blazored.SessionStorage;
using ignivault.Data;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ignivault.Services
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ISessionStorageService _localStorage;
        private readonly JwtSecurityTokenHandler _tokenHandler = new();

        public JwtAuthenticationStateProvider(ISessionStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token) || DataUtils.IsTokenExpired(token))
            {
                await _localStorage.RemoveItemAsync("authToken");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var identity = GetClaimsPrincipalFromToken(token);
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public void NotifyUserAuthentication(string token)
        {
            var identity = GetClaimsPrincipalFromToken(token);
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void NotifyUserLogout()
        {
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
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
