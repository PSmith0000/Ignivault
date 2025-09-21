namespace ignivault.WebAPI.Services
{
    public interface IVaultService
    {
        Task<VaultItemDetailDto?> GetItemAsync(int itemId, string userId);
        Task<IEnumerable<VaultItemSummaryDto>> GetItemsAsync(string userId);
        Task<VaultItemSummaryDto> CreateItemAsync(CreateVaultItemDto itemDto, string userId);
        Task<bool> UpdateItemAsync(int itemId, UpdateVaultItemDto itemDto, string userId);
        Task<bool> DeleteItemAsync(int itemId, string userId);
    }
    public class VaultService : IVaultService
    {
        private readonly IVaultItemRepository _vaultItemRepository;
        private readonly IUserActivityLogger _userActivityLogger;

        public VaultService(IVaultItemRepository vaultItemRepository, IUserActivityLogger userActivityLogger)
        {
            _vaultItemRepository = vaultItemRepository;
            _userActivityLogger = userActivityLogger;
        }

        public async Task<VaultItemDetailDto?> GetItemAsync(int itemId, string userId)
        {
            var entity = await _vaultItemRepository.GetItemByIdAndUserIdAsync(itemId, userId);
            if (entity == null)
            {
                return null;
            }

            return new VaultItemDetailDto
            {
                Id = entity.ItemId,
                Name = entity.Name,
                ItemType = entity.ItemType,
                EncryptedData = entity.EncryptedData,
                Iv = entity.Iv,
                BlobId = entity.BlobId,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        public async Task<IEnumerable<VaultItemSummaryDto>> GetItemsAsync(string userId)
        {
            var entities = await _vaultItemRepository.GetItemsByUserIdAsync(userId);

            return entities.Select(e => new VaultItemSummaryDto
            {
                Id = e.ItemId,
                Name = e.Name,
                ItemType = e.ItemType,
                UpdatedAt = e.UpdatedAt
            }).ToList();
        }

        public async Task<VaultItemSummaryDto> CreateItemAsync(CreateVaultItemDto itemDto, string userId)
        {
            var newEntity = new VaultEntity
            {
                Name = itemDto.Name,
                ItemType = itemDto.ItemType,
                UserId = userId,
                EncryptedData = itemDto.EncryptedData,
                Iv = itemDto.Iv,
            };

            if(await _vaultItemRepository.AddItemAsync(newEntity))
            {
               await _userActivityLogger.LogActivityAsync(userId, ActivityType.VaultItemCreated, true, $"{newEntity.Name}:{newEntity.ItemType.ToString()} - Added.").ConfigureAwait(false);
            }

            return new VaultItemSummaryDto
            {
                Id = newEntity.ItemId,
                Name = newEntity.Name,
                ItemType = newEntity.ItemType,
                UpdatedAt = newEntity.UpdatedAt
            };
        }

        public async Task<bool> UpdateItemAsync(int itemId, UpdateVaultItemDto itemDto, string userId)
        {
            var existingEntity = await _vaultItemRepository.GetItemByIdAndUserIdAsync(itemId, userId);
            if (existingEntity == null)
            {
                return false;
            }

            existingEntity.Name = itemDto.Name;

            if (itemDto.EncryptedData != null && itemDto.Iv != null)
            {
                existingEntity.EncryptedData = itemDto.EncryptedData;
                existingEntity.Iv = itemDto.Iv;
            }

            existingEntity.UpdatedAt = DateTime.UtcNow;

            bool updated = await _vaultItemRepository.UpdateItemAsync(existingEntity);

            if (updated)
            {
                await _userActivityLogger.LogActivityAsync(userId, ActivityType.VaultItemUpdated, true, $"{existingEntity.Name}:{existingEntity.ItemType.ToString()} - Updated.").ConfigureAwait(false);
            }

            return updated;
        }

        public async Task<bool> DeleteItemAsync(int itemId, string userId)
        {
            bool deleted = await _vaultItemRepository.DeleteItemAsync(itemId, userId);

            if (deleted)
            {
                await _userActivityLogger.LogActivityAsync(userId, ActivityType.VaultItemDeleted, true, $"ItemId:{itemId} - Deleted.").ConfigureAwait(false);
            }

            return deleted;
        }
    }
}
