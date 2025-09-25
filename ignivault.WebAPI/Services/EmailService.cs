namespace ignivault.WebAPI.Services
{
    public interface IEmailService
    {
        /// <summary>
        /// Sends a password reset email to the specified user.
        /// </summary>
        /// <param name="toEmail">The recipient's email address.</param>
        /// <param name="resetLink">The unique, single-use link the user will click to reset their password.</param>
        Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
    }

    public class SendGridEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SendGridEmailService> _logger;

        public SendGridEmailService(IConfiguration configuration, ILogger<SendGridEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            var apiKey = _configuration["SendGrid:ApiKey"];
            var fromEmail = _configuration["SendGrid:FromEmail"];
            var fromName = _configuration["SendGrid:FromName"];

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(fromEmail))
            {
                _logger.LogError("SendGrid API Key or FromEmail is not configured.");
                return;
            }

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, fromName);
            var to = new EmailAddress(toEmail);
            var subject = "Reset Your Password";
            var plainTextContent = $"Please reset your password by clicking here: {resetLink}";
            var htmlContent = $"<p>Please reset your password by clicking the link below:</p><p><a href='{resetLink}'>Reset Password</a></p>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to send password reset email to {Email}. Status code: {StatusCode}", toEmail, response.StatusCode);
            }
        }
    }
}