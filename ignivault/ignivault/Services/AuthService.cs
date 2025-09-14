using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace ignivault.Services
{
    /// <summary>
    /// Handles client-side login/logout operations and propagates authentication state.
    /// </summary>
    public class AuthService
    {
        private readonly JwtAuthenticationStateProvider _authProvider;
        private readonly ISessionStorageService _SessionStorage;

        public AuthService(AuthenticationStateProvider authProvider, ISessionStorageService sessionStorage)
        {
            _authProvider = authProvider as JwtAuthenticationStateProvider
                ?? throw new ArgumentException("AuthenticationStateProvider must be JwtAuthenticationStateProvider");
            _SessionStorage = sessionStorage ?? throw new ArgumentNullException(nameof(sessionStorage));
        }

        /// <summary>
        /// Logs in the user by storing the JWT token and notifying the auth state provider.
        /// </summary>
        public async Task LoginAsync(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    throw new ArgumentException("Token cannot be null or empty.", nameof(token));

                await _SessionStorage.SetItemAsync("authToken", token);
                _authProvider.NotifyUserAuthentication(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoginAsync failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Logs out the user by removing the JWT token and notifying the auth state provider.
        /// </summary>
        public async Task LogoutAsync()
        {
            try
            {
                await _SessionStorage.RemoveItemAsync("authToken");
                _authProvider.NotifyUserLogout();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LogoutAsync failed: {ex.Message}");
                throw;
            }
        }
    }
}
