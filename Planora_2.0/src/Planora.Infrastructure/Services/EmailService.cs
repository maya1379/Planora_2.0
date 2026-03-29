using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
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

    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");
        var smtpHost = emailSettings["SmtpHost"];
        var smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");
        var smtpUser = emailSettings["SmtpUser"];
        var smtpPassword = emailSettings["SmtpPass"];
        var senderEmail = emailSettings["SenderEmail"] ?? smtpUser ?? "noreply@planora.com";
        var senderName = emailSettings["SenderName"] ?? "Planora";

        if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUser))
        {
            _logger.LogWarning("SMTP не налаштований. Лист НЕ відправлено.");
            _logger.LogInformation("========== EMAIL ==========");
            _logger.LogInformation("To: {To}", to);
            _logger.LogInformation("Subject: {Subject}", subject);
            _logger.LogInformation("Body: {Body}", htmlBody);
            _logger.LogInformation("===========================");
            return;
        }

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(smtpUser, smtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email успішно відправлено на {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Не вдалося відправити email на {To}. Помилка: {Message}", to, ex.Message);
            throw;
        }
    }
}
