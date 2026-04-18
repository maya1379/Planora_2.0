using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Planora.Services.Services.Interfaces;

namespace Planora.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        // Email sending is disabled in production for now
        // Will be enabled when SMTP is configured
        _logger.LogWarning("SMTP не налаштований. Лист НЕ відправлено.");
        _logger.LogInformation("========== EMAIL ==========");
        _logger.LogInformation("To: {To}", to);
        _logger.LogInformation("Subject: {Subject}", subject);
        _logger.LogInformation("===========================");
        return Task.CompletedTask;
    }
}
