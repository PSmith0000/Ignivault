using Blazored.LocalStorage;
using ignivault.Data;
using ignivault.Data.Models.Auth;
using Syncfusion.Blazor.Data;

namespace ignivault.Services
{
    public class AccountService
    {
        public AccountService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        private readonly ILocalStorageService _localStorage;
        public LoginUser LoginUser { get; set; }

        public void SetAccount(LoginUser user)
        {
           LoginUser = user;
        }

        public bool IsLoggedIn()
        {
            return LoginUser != null && DataUtils.IsTokenExpired(LoginUser.Token);
        }

        public async Task<bool> IsTokenSetAsync()
        {
            var storedToken = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(storedToken) && IsLoggedIn())
            {
                return true;
            }

            if (LoginUser != null && !string.IsNullOrEmpty(LoginUser.Token) && IsLoggedIn())
            {
                return true;
            }


            return false;
        }
    }
}
