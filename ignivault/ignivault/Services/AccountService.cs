using Blazored.SessionStorage;
using ignivault.Core.Interface;
using ignivault.Data;
using ignivault.Data.Models.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
namespace ignivault.Services
{
    /// <summary>
    /// Tracks current user session and validates login state.
    /// </summary>
    public class AccountService
    {
        private readonly ISessionStorageService _localStorage;
        private readonly IHttpService _http;
        public LoginUser? LoginUser { get; private set; }

        private readonly AuthService _authService;
        private readonly NavigationManager _nav;

        public AccountService(ISessionStorageService localStorage, IHttpService http, NavigationManager nav, AuthService authService)
        {
            _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
            _http = http;
            _authService = authService;
        }

        public void SetAccount(LoginUser user)
        {
            LoginUser = user ?? throw new ArgumentNullException(nameof(user));
        }

        /// <summary>
        /// Reads the stored token and, if valid, fetches the profile.
        /// </summary>
        public async Task LoadUserAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
            {
                LoginUser = null;
                return;
            }

            var user = await _http.FetchUserProfileAsync();

            if(user == null)
            {
                _nav.NavigateTo("/login", forceLoad: true);
                return;
            }

            LoginUser = user;
            LoginUser.Token = token;
        }

        /// <summary>
        /// Determines whether the user is currently logged in (token exists and is valid).
        /// </summary>
        public async Task<bool> IsLoggedInAsync()
        {
            if (LoginUser != null && !DataUtils.IsTokenExpired(LoginUser.Token))
                return true;

            var storedToken = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(storedToken))
            {
                // Optional: parse token for validation
                return !DataUtils.IsTokenExpired(storedToken);
            }

            return false;
        }
    }
}
