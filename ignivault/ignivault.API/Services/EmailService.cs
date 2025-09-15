using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ignivault.API.Services;

public interface IEmailService
{
    Task SendPasswordResetAsync(string recipientEmail, string resetLink);
    Task SendAsync(string toEmail, string subject, string plainText, string? html = null);
}

public class EmailService : IEmailService
{
    private readonly SendGridClient _client;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public EmailService(IConfiguration configuration)
    {
        var apiKey = configuration["SendGrid:ApiKey"];
        _fromEmail = configuration["SendGrid:FromEmail"] ?? throw new ArgumentNullException("FromEmail");
        _fromName = configuration["SendGrid:FromName"] ?? "Support";

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("SendGrid ApiKey is missing in configuration.");

        _client = new SendGridClient(apiKey);
    }

    public async Task SendAsync(string toEmail, string subject, string plainText, string? html = null)
    {
        if (string.IsNullOrWhiteSpace(toEmail))
            throw new ArgumentNullException(nameof(toEmail));
        if (string.IsNullOrWhiteSpace(subject))
            throw new ArgumentNullException(nameof(subject));

        var from = new EmailAddress(_fromEmail, _fromName);
        var to = new EmailAddress(toEmail);

        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainText, html ?? plainText);
        var response = await _client.SendEmailAsync(msg);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Body.ReadAsStringAsync();
            Console.WriteLine($"Failed to send email to {toEmail}. Status: {response.StatusCode}, Body: {body}");
        }
        else
        {
            Console.WriteLine($"Email sent to {toEmail} with subject '{subject}'.");
        }
    }

    public Task SendPasswordResetAsync(string recipientEmail, string resetLink)
    {
        var subject = "Reset your password";
        var plain = $"Click the link below to reset your password:\n{resetLink}";
        var html = $@"
            <p>Hello,</p>
            <p>Click <a href=""{resetLink}"">here</a> to reset your password.</p>
            <p>If you did not request this, please ignore this email.</p>";

        return SendAsync(recipientEmail, subject, plain, html);
    }
}