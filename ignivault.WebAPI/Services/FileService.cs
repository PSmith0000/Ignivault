namespace ignivault.WebAPI.Services
{
    public interface IFileService
    {
        /// <summary>
        /// Creates a new file vault item, including its metadata and encrypted blob, within a single transaction.
        /// </summary>
        /// <param name="fileName">The original name of the file being uploaded.</param>
        /// <param name="encryptedStream">The stream containing the client-side encrypted file data.</param>
        /// <param name="iv">The unique Initialization Vector (IV) used to encrypt the file.</param>
        /// <param name="userId">The ID of the user uploading the file.</param>
        /// <returns>A summary DTO of the newly created vault item record.</returns>
        Task<VaultItemSummaryDto> CreateFileAsync(string fileName, Stream encryptedStream, byte[] iv, string userId);

        /// <summary>
        /// Retrieves an encrypted file's data for a user to download.
        /// </summary>
        /// <param name="itemId">The ID of the vault item representing the file.</param>
        /// <param name="userId">The ID of the user requesting the download, for authorization.</param>
        /// <returns>A DTO containing the file's name, content type, and the encrypted data payload, or null if not found or not authorized.</returns>
        Task<FileDownloadDto?> GetFileForDownloadAsync(int itemId, string userId);
    }

    public class FileService : IFileService
    {
        private readonly IVaultItemRepository _vaultItemRepository;
        private readonly IStoredBlobRepository _blobRepository;
        private readonly AppDbContext _context;

        public FileService(IVaultItemRepository vaultItemRepository, IStoredBlobRepository blobRepository, AppDbContext context)
        {
            _vaultItemRepository = vaultItemRepository;
            _blobRepository = blobRepository;
            _context = context;
        }

        public async Task<VaultItemSummaryDto> CreateFileAsync(string fileName, Stream encryptedStream, byte[] iv, string userId)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                await encryptedStream.CopyToAsync(memoryStream);
                var encryptedData = memoryStream.ToArray();

                var newBlob = new StoredBlob
                {
                    BlobId = Guid.NewGuid(),
                    Data = encryptedData
                };

                var newVaultItem = new VaultEntity
                {
                    Name = fileName,
                    ItemType = VaultItemType.File,
                    UserId = userId,
                    Iv = iv,
                    StoredBlob = newBlob,
                    BlobId = newBlob.BlobId,
                    EncryptedData = Array.Empty<byte>()
                };

                await _vaultItemRepository.AddItemAsync(newVaultItem);

                return new VaultItemSummaryDto
                {
                    Id = newVaultItem.ItemId,
                    Name = newVaultItem.Name,
                    ItemType = newVaultItem.ItemType,
                    UpdatedAt = newVaultItem.UpdatedAt
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<FileDownloadDto?> GetFileForDownloadAsync(int itemId, string userId)
        {
            var vaultItem = await _vaultItemRepository.GetItemByIdAndUserIdAsync(itemId, userId);
            if (vaultItem == null || vaultItem.ItemType != VaultItemType.File || vaultItem.BlobId == null)
            {
                return null;
            }

            var blob = await _blobRepository.GetByGuidAsync(vaultItem.BlobId.Value);
            if (blob == null) return null;

            var iv = vaultItem.Iv;
            var ciphertext = blob.Data;
            var payload = new byte[iv.Length + ciphertext.Length];

            Buffer.BlockCopy(iv, 0, payload, 0, iv.Length);
            Buffer.BlockCopy(ciphertext, 0, payload, iv.Length, ciphertext.Length);

            return new FileDownloadDto
            {
                FileName = $"{vaultItem.Name}.enc",
                ContentType = "application/octet-stream",
                EncryptedData = payload
            };
        }

        private static string GetContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".txt" => "text/plain",
                ".pdf" => "application/pdf",
                ".doc" => "application/vnd.ms-word",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".csv" => "text/csv",
                _ => "application/octet-stream",
            };
        }
    }
}
