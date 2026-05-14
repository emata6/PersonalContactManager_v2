namespace PersonalContactManager.Application.Interfaces;

public sealed record ReminderEmailDto(string ToEmail, string ToName, string Title, string? Note, DateTime DueAt);
public sealed record BirthdayEmailDto(string ToEmail, string ToName, DateOnly Birthday);
public sealed record ContactEmailDto(string ToEmail, string Subject, string Body, string? ReplyTo);

public interface IEmailService
{
    Task SendReminderAsync(ReminderEmailDto dto, CancellationToken cancellationToken = default);
    Task SendBirthdayReminderAsync(BirthdayEmailDto dto, CancellationToken cancellationToken = default);
    Task SendContactEmailAsync(ContactEmailDto dto, CancellationToken cancellationToken = default);
}
