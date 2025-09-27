using ignivault.Shared.DTOs.Vault;
using ignivault.WebAPI.Data.Entities;
using ignivault.WebAPI.Data.Repositories;
using ignivault.WebAPI.Data.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace ignivault.WebAPI.Services
{
    /// <summary>
    /// Defines the business logic operations for managing a user's vault items.
    /// </summary>
    public interface IVaultService
    {
        /// <summary>
        /// Retrieves a single, detailed vault item for a specific user.
        /// </summary>
        /// <param name="itemId">The ID of the item to retrieve.</param>
        /// <param name="userId">The ID of the user making the request, for authorization.</param>
        /// <returns>A detailed DTO with the item's encrypted data, or null if not found or not authorized.</returns>
        Task<VaultItemDetailDto?> GetItemAsync(int itemId, string userId);

        /// <summary>
        /// Retrieves a summary list of all vault items for a specific user, with optional filtering.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="searchText">Optional text to filter items by name.</param>
        /// <param name="startDate">Optional start date to filter items by modification date.</param>
        /// <param name="endDate">Optional end date to filter items by modification date.</param>
        /// <returns>A collection of summary DTOs for the user's vault items.</returns>
        Task<IEnumerable<VaultItemSummaryDto>> GetItemsAsync(string userId, string? searchText = null, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Creates a new vault item with a pre-encrypted payload.
        /// </summary>
        /// <param name="itemDto">The DTO containing the new item's details and encrypted data.</param>
        /// <param name="userId">The ID of the user creating the item.</param>
        /// <returns>A summary DTO of the newly created item.</returns>
        Task<VaultItemSummaryDto> CreateItemAsync(CreateVaultItemDto itemDto, string userId);

        /// <summary>
        /// Updates an existing vault item's name and optionally its encrypted data.
        /// </summary>
        /// <param name="itemId">The ID of the item to update.</param>
        /// <param name="itemDto">The DTO containing the updated information.</param>
        /// <param name="userId">The ID of the user making the request.</param>
        /// <returns>True if the update was successful, otherwise false.</returns>
        Task<bool> UpdateItemAsync(int itemId, UpdateVaultItemDto itemDto, string userId);

        /// <summary>
        /// Deletes a vault item owned by the specified user.
        /// </summary>
        /// <param name="itemId">The ID of the item to delete.</param>
        /// <param name="userId">The ID of the user making the request.</param>
        /// <returns>True if the deletion was successful, otherwise false.</returns>
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

            // Log that a specific item was viewed, as this is a sensitive action.
            await _userActivityLogger.LogActivityAsync(userId, ActivityType.VaultItemViewed, true, $"Viewed item: {entity.Name} (ID: {entity.ItemId})");

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

        public async Task<IEnumerable<VaultItemSummaryDto>> GetItemsAsync(string userId, string? searchText = null, DateTime? startDate = null, DateTime? endDate = null)
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

            await _vaultItemRepository.AddAsync(newEntity);
            await _vaultItemRepository.SaveChangesAsync(); // Commit the change to the database.

            await _userActivityLogger.LogActivityAsync(userId, ActivityType.VaultItemCreated, true, $"Created item: {newEntity.Name} (ID: {newEntity.ItemId})");

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

            // Manually update the timestamp to reflect the change.
            existingEntity.UpdatedAt = DateTime.UtcNow;

            var success = await _vaultItemRepository.UpdateItemAsync(existingEntity);

            if (success)
            {
                await _userActivityLogger.LogActivityAsync(userId, ActivityType.VaultItemUpdated, true, $"Updated item: {existingEntity.Name} (ID: {existingEntity.ItemId})");
            }

            return success;
        }

        public async Task<bool> DeleteItemAsync(int itemId, string userId)
        {
            // Fetch the item first to get its name for logging before it's deleted.
            var itemToDelete = await _vaultItemRepository.GetItemByIdAndUserIdAsync(itemId, userId);
            if (itemToDelete == null)
            {
                return false;
            }

            var itemName = itemToDelete.Name; // Store the name for the log message.
            var success = await _vaultItemRepository.DeleteItemAsync(itemId, userId);

            if (success)
            {
                await _userActivityLogger.LogActivityAsync(userId, ActivityType.VaultItemDeleted, true, $"Deleted item: {itemName} (ID: {itemId})");
            }

            return success;
        }
    }
}
