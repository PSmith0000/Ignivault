namespace ignivault.WebAPI.Data.Repositories
{
    public interface IVaultItemRepository : IRepository<VaultEntity>
    {
        /// <summary>
        /// Gets all vault items for a specific user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<VaultEntity>> GetItemsByUserIdAsync(string userId);

        /// <summary>
        /// Adds a new vault item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<bool> AddItemAsync(VaultEntity item);
        Task<VaultEntity?> GetItemByIdAndUserIdAsync(int itemId, string userId);

        /// <summary>
        /// Updates an existing vault item.
        /// </summary>
        /// <param name="itemToUpdate"></param>
        /// <returns></returns>
        Task<bool> UpdateItemAsync(VaultEntity itemToUpdate);

        /// <summary>
        /// Deletes a vault item by its ID and user ID.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> DeleteItemAsync(int itemId, string userId);
    }

    public class VaultItemRepository : Repository<VaultEntity>, IVaultItemRepository
    {
        public VaultItemRepository(AppDbContext context) : base(context)
        {
          
        }

        public async Task<bool> AddItemAsync(VaultEntity item)
        {
            await _context.Set<VaultEntity>().AddAsync(item);
            int count = await _context.SaveChangesAsync();

            if(count > 0)
            {
                return true;
            }

            return false;
        }
        public async Task<VaultEntity?> GetItemByIdAndUserIdAsync(int itemId, string userId)
        {
            return await _context.Set<VaultEntity>()
                .FirstOrDefaultAsync(item => item.ItemId == itemId && item.UserId == userId);
        }

        public async Task<IEnumerable<VaultEntity>> GetItemsByUserIdAsync(string userId)
        {
            return await _context.Set<VaultEntity>()
                .Where(item => item.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> UpdateItemAsync(VaultEntity itemToUpdate)
        {
            _context.Set<VaultEntity>().Update(itemToUpdate);
            var changed = await _context.SaveChangesAsync();
            return changed > 0;
        }

        public async Task<bool> DeleteItemAsync(int itemId, string userId)
        {
            var itemToDelete = await GetItemByIdAndUserIdAsync(itemId, userId);

            if (itemToDelete == null)
            {
                return false;
            }

            _context.Set<VaultEntity>().Remove(itemToDelete);
            var changed = await _context.SaveChangesAsync();
            return changed > 0;
        }
    }
}