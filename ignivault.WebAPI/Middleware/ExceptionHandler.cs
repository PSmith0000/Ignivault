namespace ignivault.WebAPI.Middleware
{
    public class ExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<ExceptionHandler> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandler(ILogger<ExceptionHandler> logger, IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An unhandled exception occurred.");

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";

            var problemDetails = new
            {
                StatusCode = httpContext.Response.StatusCode,
                Title = "An unexpected error occurred.",
                Detail = _env.IsDevelopment() ? exception.Message : "An internal server error has occurred. Please try again later."
            };

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }
    }
}
