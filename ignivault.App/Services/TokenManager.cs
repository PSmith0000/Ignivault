using Blazored.SessionStorage;
using ignivault.ApiClient;
using Microsoft.JSInterop;

namespace ignivault.App.Services
{
    public class TokenManager : ITokenManager
    {
        private readonly ISessionStorageService _sessionStorage;
        private const string TokenKey = "authToken";

        public TokenManager(ISessionStorageService sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _sessionStorage.GetItemAsStringAsync(TokenKey);
        }

        public async Task SetTokenAsync(string? token)
        {
            if (string.IsNullOrEmpty(token))
            {
                await _sessionStorage.RemoveItemAsync(TokenKey);
            }
            else
            {
                await _sessionStorage.SetItemAsStringAsync(TokenKey, token);
            }
        }

        public async Task ClearTokenAsync()
        {
            await _sessionStorage.RemoveItemAsync(TokenKey);
        }
    }
}

