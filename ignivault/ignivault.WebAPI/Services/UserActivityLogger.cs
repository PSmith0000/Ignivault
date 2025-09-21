namespace ignivault.WebAPI.Services
{
    public interface IUserActivityLogger
    {
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