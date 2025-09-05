using ignivault.Data.Models.Auth;
using ignivault.Data.Models.Data;

namespace ignivault.Core.Interface
{
    public interface IHttpService
    {
        abstract Task<(bool Success, LoginResponse? Response)> LoginAsync(string email, string password);
        public abstract Task<(bool Success, string? Message)> RegisterAsync(RegistrationModel model);
        public abstract Task<List<VaultItem>?> GetVaultItemsAsync();
    }
}
