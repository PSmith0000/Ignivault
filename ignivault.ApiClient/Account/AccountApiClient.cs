namespace ignivault.ApiClient.Account
{

    public interface IAccountApiClient
    {
        /// <summary>
        /// Gets the non-sensitive profile information for the currently authenticated user.
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse<UserProfileDto>> GetProfileAsync();

        /// <summary>
        /// Updates the user's login password after verifying their current password.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ApiResponse> UpdatePasswordAsync(UpdatePasswordRequestDto request);

        /// <summary>
        /// Updates the user's master password after verifying their current password and receiving re-encrypted vault items.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ApiResponse> UpdateMasterPasswordAsync(UpdateMasterPasswordRequestDto request);

        /// <summary>
        /// Gets the most recent user activities, limited by the specified number.
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<UserActivityDto>>> GetRecentActivityAsync(int limit = 10);

        /// <summary>
        /// Disables Two-Factor Authentication (2FA) for the currently authenticated user.
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse> Disable2faAsync();

        /// <summary>
        /// Regenerates a new set of 2FA recovery codes for the currently authenticated user, invalidating any old ones.
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<string>>> RegenerateRecoveryCodesAsync();
    }
    public class AccountApiClient : IAccountApiClient
    {
        private readonly HttpClient _httpClient;
        public AccountApiClient(HttpClient httpClient) { _httpClient = httpClient; }

        public async Task<ApiResponse<UserProfileDto>> GetProfileAsync()
        {
            try
            {
                var profile = await _httpClient.GetFromJsonAsync<UserProfileDto>(ApiEndpoints.Account.ProfileEndpoint);
                return new ApiResponse<UserProfileDto> { IsSuccess = true, Data = profile };
            }
            catch (HttpRequestException ex)
            {
                return new ApiResponse<UserProfileDto> { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse> UpdatePasswordAsync(UpdatePasswordRequestDto request)
        {
            var response = await _httpClient.PutAsJsonAsync(ApiEndpoints.Account.PasswordEndpoint, request);
            return new ApiResponse { IsSuccess = response.IsSuccessStatusCode, Message = response.IsSuccessStatusCode ? "Success" : "Failed to update password." };
        }

        public async Task<ApiResponse> UpdateMasterPasswordAsync(UpdateMasterPasswordRequestDto request)
        {
            var response = await _httpClient.PutAsJsonAsync(ApiEndpoints.Account.MasterPasswordEndpoint, request);
            return new ApiResponse { IsSuccess = response.IsSuccessStatusCode, Message = response.IsSuccessStatusCode ? "Success" : "Failed to update master password." };
        }

        public async Task<ApiResponse<IEnumerable<UserActivityDto>>> GetRecentActivityAsync(int limit = 10)
        {
            var url = ApiEndpoints.Account.ActivityEndpoint;
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<UserActivityDto>>($"{url}?limit={limit}");
            return new ApiResponse<IEnumerable<UserActivityDto>> { IsSuccess = true, Data = response };
        }

        public async Task<ApiResponse> Disable2faAsync()
        {
            var response = await _httpClient.PostAsync(ApiEndpoints.Account.Disable2faEndpoint, null);
            return new ApiResponse { IsSuccess = response.IsSuccessStatusCode, Message = await response.Content.ReadAsStringAsync() };
        }

        public async Task<ApiResponse<IEnumerable<string>>> RegenerateRecoveryCodesAsync()
        {
            var response = await _httpClient.PostAsync(ApiEndpoints.Account.RegenerateRecoveryCodesEndpoint, null);
            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<IEnumerable<string>> { IsSuccess = false, Message = "Failed to regenerate codes." };
            }
            var codes = await response.Content.ReadFromJsonAsync<IEnumerable<string>>();
            return new ApiResponse<IEnumerable<string>> { IsSuccess = true, Data = codes };
        }
    }
}
