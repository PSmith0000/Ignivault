using Blazored.LocalStorage;
using ignivault.Core.Interface;
using ignivault.Data.Models.Auth;
using ignivault.Data.Models.Data;
using System.Net.Http.Json;
namespace ignivault.Services
{
    public class HttpService : HttpClient, IHttpService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;
        public HttpService(HttpClient http, ILocalStorageService localStorage)
        {
            _http = http;
            _localStorage = localStorage;
           
        }

        public async Task<(bool Success, LoginResponse? Response)> LoginAsync(string email, string password)
        {
            var response = await _http.PostAsJsonAsync("https://localhost:7158/api/Authentication/login", new { Email = email, Password = password });
            if (response.IsSuccessStatusCode)
            {
                var loginData = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (loginData != null)
                {
                    return (true, loginData);
                }
            }
            return (false, null);
        }

        public async Task<(bool Success, string? Message)> RegisterAsync(RegistrationModel model)
        {
            var response = await _http.PostAsJsonAsync("https://localhost:7158/api/Authentication/register", model);
            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return (false, error);
            }
        }

        public async Task<List<VaultItem>?> GetVaultItemsAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }
            _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _http.GetAsync("https://localhost:7158/api/Vault/myvault");
            if (response.IsSuccessStatusCode)
            {
                var vaultItems = await response.Content.ReadFromJsonAsync<List<VaultItem>>();
                return vaultItems;
            }
            return null;
        }
    }
}
