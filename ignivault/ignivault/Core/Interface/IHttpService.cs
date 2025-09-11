using ignivault.Data.Models.Auth;
using ignivault.Data.Models.Data;

namespace ignivault.Core.Interface
{
    public interface IHttpService
    {
        public abstract Task<(bool Success, LoginUser? Response)> LoginAsync(string email, string password);
        public abstract Task<(bool Success, string? Message)> RegisterAsync(RegistrationModel model);
        public abstract Task<List<VaultItem>?> GetVaultItemsAsync();

        public abstract Task<bool> AddVaultItemAsync(VaultItem item);

        public abstract Task<bool> UpdateVaultItem(VaultItem item);

        public abstract Task<bool> DeleteVaultItem(int itemId);
    }
}
