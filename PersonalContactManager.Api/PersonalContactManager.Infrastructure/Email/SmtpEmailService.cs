using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using PersonalContactManager.Application.Interfaces;

namespace PersonalContactManager.Infrastructure.Email;

public sealed class SmtpEmailService : IEmailService
{
    private readonly EmailOptions _options;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IOptions<EmailOptions> options, ILogger<SmtpEmailService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendReminderAsync(ReminderEmailDto dto, CancellationToken cancellationToken = default)
    {
        var body = LoadTemplate("ReminderEmail.html")
            .Replace("{{Title}}", dto.Title)
            .Replace("{{DueAt}}", dto.DueAt.ToString("dddd, MMMM d yyyy 'at' HH:mm"))
            .Replace("{{ContactName}}", dto.ToName)
            .Replace("{{#Note}}", dto.Note is not null ? string.Empty : "<!--")
            .Replace("{{/Note}}", dto.Note is not null ? string.Empty : "-->")
            .Replace("{{Note}}", dto.Note ?? string.Empty);

        await SendAsync(dto.ToEmail, dto.ToName, $"Reminder: {dto.Title}", body, cancellationToken);
    }

    public async Task SendBirthdayReminderAsync(BirthdayEmailDto dto, CancellationToken cancellationToken = default)
    {
        var body = LoadTemplate("BirthdayReminderEmail.html")
            .Replace("{{ContactName}}", dto.ToName)
            .Replace("{{Birthday}}", dto.Birthday.ToString("MMMM d"));

        await SendAsync(dto.ToEmail, dto.ToName, $"Birthday Reminder: {dto.ToName}", body, cancellationToken);
    }

    public async Task SendContactEmailAsync(ContactEmailDto dto, CancellationToken cancellationToken = default)
    {
        var body = LoadTemplate("ContactEmail.html")
            .Replace("{{Subject}}", dto.Subject)
            .Replace("{{Body}}", dto.Body);

        await SendAsync(dto.ToEmail, string.Empty, dto.Subject, body, cancellationToken);
    }

    private async Task SendAsync(string toEmail, string toName, string subject, string htmlBody, CancellationToken cancellationToken)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_options.SmtpHost, _options.SmtpPort,
                _options.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None,
                cancellationToken);

            if (!string.IsNullOrEmpty(_options.Username))
                await client.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("Email sent to {Email} — subject: {Subject}", toEmail, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email} — subject: {Subject}", toEmail, subject);
            throw;
        }
    }

    private static string LoadTemplate(string fileName)
    {
        var assembly = typeof(SmtpEmailService).Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith(fileName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"Email template '{fileName}' not found in assembly resources.");

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
