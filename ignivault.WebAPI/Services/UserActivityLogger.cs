namespace ignivault.WebAPI.Services
{
    /// <summary>
    /// Provides a service for logging user activities for auditing purposes.
    /// </summary>
    public interface IUserActivityLogger
    {
        /// <summary>
        /// Creates and saves a log entry for a user activity.
        /// </summary>
        /// <param name="userId">The ID of the user performing the action. Use "N/A" for anonymous actions like failed logins.</param>
        /// <param name="activityType">The type of activity being logged.</param>
        /// <param name="success">A boolean indicating whether the action was successful.</param>
        /// <param name="details">Optional, user-facing details about the event (e.g., the name of an affected item).</param>
        Task LogActivityAsync(string userId, ActivityType activityType, bool success, string? details = null);
    }

    public class UserActivityLogger : IUserActivityLogger
    {
        private readonly IUserActivityRepository _activityRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserActivityLogger(IUserActivityRepository activityRepository, IHttpContextAccessor httpContextAccessor)
        {
            _activityRepository = activityRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogActivityAsync(string userId, ActivityType activityType, bool success, string? details = null)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var activity = new UserActivity
            {
                UserId = userId,
                ActivityType = activityType,
                Success = success,
                Details = details,
                Timestamp = DateTime.UtcNow,
                IpAddress = httpContext?.Connection?.RemoteIpAddress?.ToString() ?? "N/A",
                UserAgent = httpContext?.Request?.Headers["User-Agent"].ToString() ?? "N/A"
            };

            await _activityRepository.AddAsync(activity);
            await _activityRepository.SaveChangesAsync();
        }
    }
}