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
            if (LoginUser == null) return false;

            if (DataUtils.IsTokenExpired(LoginUser.Token)) return false;

            return true;
        }

        public async Task<bool> IsTokenSetAsync()
        {
            var storedToken = await _localStorage.GetItemAsync<string>("authToken");
            bool nullToken = string.IsNullOrEmpty(storedToken);

            bool loggedIn = IsLoggedIn();

            Console.WriteLine($"Stored Token: {storedToken}, IsLoggedIn: {loggedIn}, IsNullOrEmpty: {nullToken}");

            return !nullToken && loggedIn;
        }
    }
}
