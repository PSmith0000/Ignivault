using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace ignivault.Services
{
    public class AuthService
    {
        private readonly JwtAuthenticationStateProvider _authProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthService(AuthenticationStateProvider authProvider, ILocalStorageService localStorage)
        {
            _authProvider = (JwtAuthenticationStateProvider)authProvider;
            _localStorage = localStorage;
        }

        public async Task LoginAsync(string token)
        {
            await _localStorage.SetItemAsync("authToken", token);
            _authProvider.NotifyUserAuthentication(token);
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            _authProvider.NotifyUserLogout();
        }
    }
}
