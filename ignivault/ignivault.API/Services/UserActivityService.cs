using ignivault.API.Models.Records;
using ignivault.API.SQL;
using Microsoft.EntityFrameworkCore;

namespace ignivault.API.Services
{
    public class UserActivityService
    {
        private readonly AppDbContext _dbContext;

        public UserActivityService(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Logs a user activity.
        /// </summary>
        /// <param name="userId">The ID of the user performing the activity.</param>
        /// <param name="activityType">The type of activity (e.g., Login, VaultAccess).</param>
        /// <param name="details">Optional details about the activity.</param>
        public async Task LogActivityAsync(string userId, string activityType, string? details = null)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId cannot be null or empty.", nameof(userId));

            if (string.IsNullOrWhiteSpace(activityType))
                throw new ArgumentException("ActivityType cannot be null or empty.", nameof(activityType));

            var activity = new UserActivity
            {
                UserId = userId,
                ActivityType = activityType,
                Details = details,
                ActivityTime = DateTime.UtcNow
            };

            _dbContext.UserActivities.Add(activity);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves the recent activities for a given user.
        /// </summary>
        public async Task<List<UserActivity>> GetRecentActivitiesAsync(string userId, int limit = 10)
        {
            return await _dbContext.UserActivities
                                   .AsNoTracking()
                                   .Where(a => a.UserId == userId)
                                   .OrderByDescending(a => a.ActivityTime)
                                   .Take(limit)
                                   .ToListAsync();
        }
    }
}
