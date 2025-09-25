namespace ignivault.ApiClient.Vault
{
    public interface IVaultApiClient
    {
        /// <summary>
        /// Gets all vault items for the authenticated user.
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<VaultItemSummaryDto>>> GetItemsAsync();

        /// <summary>
        /// Gets a specific vault item by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ApiResponse<VaultItemDetailDto>> GetItemByIdAsync(int id);

        /// <summary>
        /// Creates a new vault item.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ApiResponse<VaultItemSummaryDto>> CreateItemAsync(CreateVaultItemDto request);

        /// <summary>
        /// Deletes a vault item by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ApiResponse> DeleteItemAsync(int id);

        /// <summary>
        /// Uploads a file to the vault.
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="fileName"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        Task<ApiResponse<VaultItemSummaryDto>> UploadFileAsync(Stream fileStream, string fileName, byte[] iv);

        /// <summary>
        /// Downloads a file from the vault by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ApiResponse<Stream>> DownloadFileAsync(int id);

        /// <summary>
        /// Updates an existing vault item.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>

        Task<ApiResponse> UpdateItemAsync(int id, UpdateVaultItemDto request);
    }
    public class VaultApiClient : IVaultApiClient
    {
        private readonly HttpClient _httpClient;
        public VaultApiClient(HttpClient httpClient) { _httpClient = httpClient; }

        public async Task<ApiResponse<IEnumerable<VaultItemSummaryDto>>> GetItemsAsync()
        {
            var items = await _httpClient.GetFromJsonAsync<IEnumerable<VaultItemSummaryDto>>(ApiEndpoints.Vault.ItemsEndpoint);
            return new ApiResponse<IEnumerable<VaultItemSummaryDto>> { IsSuccess = true, Data = items };
        }

        public async Task<ApiResponse<VaultItemDetailDto>> GetItemByIdAsync(int id)
        {
            var url = ApiEndpoints.Vault.ItemByIdEndpoint.Replace("{id}", id.ToString());
            var item = await _httpClient.GetFromJsonAsync<VaultItemDetailDto>(url);
            return new ApiResponse<VaultItemDetailDto> { IsSuccess = true, Data = item };
        }

        public async Task<ApiResponse<VaultItemSummaryDto>> CreateItemAsync(CreateVaultItemDto request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Vault.ItemsEndpoint, request);
            if (!response.IsSuccessStatusCode) return new ApiResponse<VaultItemSummaryDto> { IsSuccess = false, Message = "Failed to create item." };

            var newItem = await response.Content.ReadFromJsonAsync<VaultItemSummaryDto>();
            return new ApiResponse<VaultItemSummaryDto> { IsSuccess = true, Data = newItem };
        }

        public async Task<ApiResponse> DeleteItemAsync(int id)
        {
            var url = ApiEndpoints.Vault.ItemByIdEndpoint.Replace("{id}", id.ToString());
            var response = await _httpClient.DeleteAsync(url);
            return new ApiResponse { IsSuccess = response.IsSuccessStatusCode };
        }

        public async Task<ApiResponse<VaultItemSummaryDto>> UploadFileAsync(Stream fileStream, string fileName, byte[] iv)
        {
            using var content = new MultipartFormDataContent();

            content.Add(new StreamContent(fileStream), "File", fileName);

            content.Add(new StringContent(Convert.ToBase64String(iv)), "IvBase64");

            var response = await _httpClient.PostAsync(ApiEndpoints.Vault.FilesEndpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<VaultItemSummaryDto> { IsSuccess = false, Message = "File upload failed." };
            }

            var newFileItem = await response.Content.ReadFromJsonAsync<VaultItemSummaryDto>();
            return new ApiResponse<VaultItemSummaryDto> { IsSuccess = true, Data = newFileItem };
        }

        public async Task<ApiResponse<Stream>> DownloadFileAsync(int id)
        {
            var url = ApiEndpoints.Vault.DownloadFileEndpoint.Replace("{id}", id.ToString());
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return new ApiResponse<Stream> { IsSuccess = false, Message = "File download failed." };

            var stream = await response.Content.ReadAsStreamAsync();
            return new ApiResponse<Stream> { IsSuccess = true, Data = stream };
        }

        public async Task<ApiResponse> UpdateItemAsync(int id, UpdateVaultItemDto request)
        {
            var url = ApiEndpoints.Vault.ItemByIdEndpoint.Replace("{id}", id.ToString());
            var response = await _httpClient.PutAsJsonAsync(url, request);
            return new ApiResponse { IsSuccess = response.IsSuccessStatusCode, Message = response.IsSuccessStatusCode ? "Success" : "Failed to update item." };
        }
    }
}
