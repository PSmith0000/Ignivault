namespace ignivault.ApiClient.Vault
{
    public interface IVaultApiClient
    {
        Task<ApiResponse<IEnumerable<VaultItemSummaryDto>>> GetItemsAsync();
        Task<ApiResponse<VaultItemDetailDto>> GetItemByIdAsync(int id);
        Task<ApiResponse<VaultItemSummaryDto>> CreateItemAsync(CreateVaultItemDto request);
        Task<ApiResponse> DeleteItemAsync(int id);
        Task<ApiResponse<VaultItemSummaryDto>> UploadFileAsync(Stream fileStream, string fileName, byte[] iv);
        Task<ApiResponse<Stream>> DownloadFileAsync(int id);

        Task<ApiResponse> UpdateItemAsync(int id, UpdateVaultItemDto request);
    }
}
