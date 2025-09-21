namespace ignivault.ApiClient.Account
{
    public interface IAccountApiClient
    {
        Task<ApiResponse<UserProfileDto>> GetProfileAsync();
        Task<ApiResponse> UpdatePasswordAsync(UpdatePasswordRequestDto request);
        Task<ApiResponse> UpdateMasterPasswordAsync(UpdateMasterPasswordRequestDto request);
        Task<ApiResponse<IEnumerable<UserActivityDto>>> GetRecentActivityAsync(int limit = 10);
        Task<ApiResponse> Disable2faAsync();
        Task<ApiResponse<IEnumerable<string>>> RegenerateRecoveryCodesAsync();
    }
}
