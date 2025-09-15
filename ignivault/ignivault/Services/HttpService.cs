using Blazored.SessionStorage;
using ignivault.Core.Interface;
using ignivault.Data;
using ignivault.Data.Models.Auth;
using ignivault.Data.Models.Data;
using ignivault.Pages;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ignivault.Services
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _http;
        private readonly ISessionStorageService _sessionStorage;

        public HttpService(HttpClient http, ISessionStorageService sessionStorage)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _sessionStorage = sessionStorage ?? throw new ArgumentNullException(nameof(sessionStorage));
        }

        public string ApiBaseUrl => _http.BaseAddress?.ToString() ?? string.Empty;

        // ---------- Authentication ----------
        public async Task<LoginUser?> LoginAsync(string email, string password)
        {
            var loginRequest = new { Email = email, Password = password };
            return await SendAsync<LoginUser>(HttpMethod.Post, "api/Authentication/login", loginRequest);
        }

        public async Task<(bool Success, string? Message)> RegisterAsync(RegistrationModel model)
        {
            try
            {
                var resp = await _http.PostAsJsonAsync("api/Authentication/register", model);
                return resp.IsSuccessStatusCode
                    ? (true, null)
                    : (false, await resp.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<LoginUser?> FetchUserProfileAsync()
        {
            return await SendAsync<LoginUser>(HttpMethod.Get, "api/Authentication/userdata");
        }

        // ---------- Vault ----------
        public Task<RecordTypes.VaultResponse?> GetVaultItemsAsync() =>
            SendAsync<RecordTypes.VaultResponse>(HttpMethod.Get, "api/vault/myvault");

        public Task<bool> AddVaultItemAsync(VaultItem item) =>
            SendNoContentAsync(HttpMethod.Post, "api/vault/add", item);

        public Task<bool> UpdateVaultItem(VaultItem item) =>
            SendNoContentAsync(HttpMethod.Put, "api/vault/update", item);

        public Task<bool> DeleteVaultItem(int itemId) =>
            SendNoContentAsync(HttpMethod.Delete, $"api/vault/delete/?itemId={itemId}");

        public async Task<byte[]?> GetFileData(int itemId)
        {
            var request = await CreateRequestAsync(HttpMethod.Get, $"api/vault/getfile?itemId={itemId}");
            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsByteArrayAsync() : null;
        }

        // ---------- Helpers ----------
        private async Task<T?> SendAsync<T>(HttpMethod method, string url, object? content = null)
        {
            var request = await CreateRequestAsync(method, url, content);
            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<T>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                : default;
        }

        private async Task<bool> SendNoContentAsync(HttpMethod method, string url, object? content = null)
        {
            var request = await CreateRequestAsync(method, url, content);
            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        private async Task<HttpRequestMessage> CreateRequestAsync(HttpMethod method, string url, object? content = null)
        {
            var request = new HttpRequestMessage(method, url);

            var token = await _sessionStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (content != null)
                request.Content = JsonContent.Create(content);

            return request;
        }

        public async Task<List<UserActivity>?> GetUserActivitiesAsync()
        {
            return await SendAsync<List<UserActivity>>(HttpMethod.Get, "api/user/activities");
        }

        public async Task<(bool Success, string? Message)> ResetPasswordAsync(ResetPasswordModel resetPasswordModel)
        {
            try
            {
                var request = await CreateRequestAsync(HttpMethod.Post, "api/Authentication/reset-password", resetPasswordModel);
                var response = await _http.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Password reset successfully.");
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return (false, content);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Message)> RequestPasswordResetAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return (false, "Email cannot be empty.");

                var model = new { Email = email };
                var request = await CreateRequestAsync(HttpMethod.Post, "api/Authentication/request-reset", model);
                var response = await _http.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Password reset successfully.");
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return (false, content);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Message)> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            try
            {
                var model = new { CurrentPassword = currentPassword, NewPassword = newPassword };
                var request = await CreateRequestAsync(HttpMethod.Post, "api/user/change-password", model);

                var response = await _http.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Password changed successfully.");
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return (false, content);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
