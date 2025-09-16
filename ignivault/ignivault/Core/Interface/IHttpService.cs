using ignivault.Data;
using ignivault.Data.Models.Auth;
using ignivault.Data.Models.Data;

namespace ignivault.Core.Interface
{
    public interface IHttpService
    {
        public abstract Task<(bool Success, string? Message)> ResetVaultKeyAsync(string? currentKey, string newKey, string? token = null);
        public abstract Task<(bool Success, string? Message)> RequestPasswordResetAsync(string email);
        public abstract Task<(bool Success, string? Message)> ResetPasswordAsync(ResetPasswordModel resetPasswordModel);
        public abstract Task<(bool Success, string? Message)> ChangePasswordAsync(string currentPassword, string newPassword);
        public abstract Task<LoginUser?> FetchUserProfileAsync();
        public abstract Task<LoginUser?> LoginAsync(string email, string password);
        public abstract Task<(bool Success, string? Message)> RegisterAsync(RegistrationModel model);
        public abstract Task<RecordTypes.VaultResponse?> GetVaultItemsAsync();

        public abstract Task<List<VaultStorageMonthly>?> GetVaultStorageMonthlyAsync();

        public abstract Task<bool> AddVaultItemAsync(VaultItem item);

        public abstract Task<bool> UpdateVaultItem(VaultItem item);

        public abstract Task<bool> DeleteVaultItem(int itemId);

        public abstract Task<byte[]?> GetFileData(int itemId);

        public abstract Task<List<UserActivity>?> GetUserActivitiesAsync(int limit = 5);

        public abstract string ApiBaseUrl { get; }
    }
}
