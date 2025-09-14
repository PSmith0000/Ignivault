using Blazored.SessionStorage;
using ignivault.Data.Models.Auth;
using ignivault.Data;

namespace ignivault.Services
{
    /// <summary>
    /// Tracks current user session and validates login state.
    /// </summary>
    public class AccountService
    {
        private readonly ISessionStorageService _localStorage;
        public LoginUser? LoginUser { get; private set; }

        public AccountService(ISessionStorageService localStorage)
        {
            _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
        }

        public void SetAccount(LoginUser user)
        {
            LoginUser = user ?? throw new ArgumentNullException(nameof(user));
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
