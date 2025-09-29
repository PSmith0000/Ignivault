namespace ignivault.WebAPI.Data.Repositories
{
    public class UserActivityRepository : Repository<UserActivity>, IUserActivityRepository
    {
        public UserActivityRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UserActivity>> GetRecentActivitiesForUserAsync(string userId, int limit = 20)
        {
            return await _context.Set<UserActivity>()
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserActivity>> GetAllAsync(int limit, int offset = 0)
        {
            return await _context.Set<UserActivity>()
                .OrderByDescending(a => a.Timestamp)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }
    }
}
