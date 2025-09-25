namespace ignivault.WebAPI.Data.Repositories
{
    public interface IUserActivityRepository : IRepository<UserActivity>
    {
        /// <summary>
        /// Gets recent activities for a specific user, limited to a certain number of entries.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IEnumerable<UserActivity>> GetRecentActivitiesForUserAsync(string userId, int limit = 20);

        /// <summary>
        /// Gets all user activities, with pagination support via limit and offset.
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Task<IEnumerable<UserActivity>> GetAllAsync(int limit, int offset = 0);
    }
}
