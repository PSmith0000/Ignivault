using Blazored.LocalStorage;
using ignivault.Core.Interface;
using ignivault.Data.Models.Auth;
using ignivault.Data.Models.Data;
using System.Net.Http.Json;
using System.Text.Json;
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

        public async Task<(bool Success, LoginUser? Response)> LoginAsync(string email, string password)
        {
            var response = await _http.PostAsJsonAsync("api/Authentication/login", new { Email = email, Password = password });
            if (response.IsSuccessStatusCode)
            {
                var loginData = await response.Content.ReadAsStringAsync();

                //flatten json
                var doc = JsonDocument.Parse(loginData);
                var loginUserJson = doc.RootElement.GetProperty("loginUser").GetRawText();
                var loginUser = JsonSerializer.Deserialize<LoginUser>(loginUserJson);

                if (loginUser != null)
                {
                    return (true, loginUser);
                }
            }
            return (false, null);
        }

        public async Task<(bool Success, string? Message)> RegisterAsync(RegistrationModel model)
        {
            var response = await _http.PostAsJsonAsync("api/Authentication/register", model);
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
            var response = await _http.GetAsync("api/Vault/myvault");
            if (response.IsSuccessStatusCode)
            {
                var vaultItems = await response.Content.ReadFromJsonAsync<List<VaultItem>>();
                return vaultItems;
            }
            return null;
        }

        public async Task<bool> AddVaultItemAsync(VaultItem item)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrEmpty(token))
                return false;

            _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _http.PostAsJsonAsync("api/vault/add", item);

            return response.IsSuccessStatusCode;
        }


        public async Task<bool> UpdateVaultItem(VaultItem item)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrEmpty(token))
                return false;
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _http.PutAsJsonAsync("api/vault/update", item);
            return response.IsSuccessStatusCode;
        }


        public async Task<bool> DeleteVaultItem(int itemId)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrEmpty(token))
                return false;
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _http.DeleteAsync($"api/vault/delete/?itemId={itemId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<byte[]?> GetFileData(int itemId)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrEmpty(token))
                return null;
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _http.GetAsync($"api/vault/getfile?itemId={itemId}");
            if (response.IsSuccessStatusCode)
            {
                var fileData = await response.Content.ReadAsStringAsync();
                return Convert.FromBase64String(fileData);
            }

            return null;
        } 
    }
}
