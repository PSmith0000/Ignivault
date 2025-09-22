namespace ignivault.WebAPI.Data.Repositories
{
    public interface IStoredBlobRepository : IRepository<StoredBlob>
    {
        Task AddBlobAsync(StoredBlob blob);
        Task<StoredBlob?> GetByGuidAsync(Guid guid);
        Task<IEnumerable<StoredBlob>> GetAllByUserId(string userId);
        Task<bool> UpdateBlobAsync(StoredBlob blob);
        Task<bool> DeleteByGuidAsync(Guid guid);
        Task<bool> BlobExistsAsync(Guid guid);
    }

    public class BlobRepository : Repository<StoredBlob>, IStoredBlobRepository
    {
        public BlobRepository(AppDbContext context) : base(context)
        {
        }
        public async Task AddBlobAsync(StoredBlob blob)
        {
            await _context.Set<StoredBlob>().AddAsync(blob);
            await _context.SaveChangesAsync();
        }

        public async Task<StoredBlob?> GetByGuidAsync(Guid guid)
        {
            return await _context.Set<StoredBlob>().FirstOrDefaultAsync(item => item.BlobId == guid);
        }

        public async Task<IEnumerable<StoredBlob>> GetAllByUserId(string userId)
        {
            return await _context.Set<StoredBlob>().Include(blob => blob.VaultItem).Where(blob => blob.VaultItem.UserId == userId).ToListAsync();
        }

        public async Task<bool> UpdateBlobAsync(StoredBlob blob)
        {
            var exists = await BlobExistsAsync(blob.BlobId);
            if (!exists)
            {
                return false;
            }

            _context.Set<StoredBlob>().Update(blob);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteByGuidAsync(Guid guid)
        {
            var blob = await GetByGuidAsync(guid);
            if (blob == null)
            {
                return false;
            }

            _context.Set<StoredBlob>().Remove(blob);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> BlobExistsAsync(Guid guid)
        {
            return await _context.Set<StoredBlob>().AnyAsync(b => b.BlobId == guid);
        }
    }
}
