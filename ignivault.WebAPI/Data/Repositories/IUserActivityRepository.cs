namespace ignivault.WebAPI.Data.Repositories
{
    public interface IUserActivityRepository : IRepository<UserActivity>
    {
        Task<IEnumerable<UserActivity>> GetRecentActivitiesForUserAsync(string userId, int limit = 20);
        Task<IEnumerable<UserActivity>> GetAllAsync(int limit, int offset = 0);
    }
}
