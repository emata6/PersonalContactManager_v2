using Microsoft.Extensions.Logging;
using PersonalContactManager.Application.Interfaces;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Infrastructure.BackgroundJobs;

public sealed class BirthdayReminderJob
{
    private readonly IContactRepository _contactRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<BirthdayReminderJob> _logger;

    public BirthdayReminderJob(
        IContactRepository contactRepository,
        IEmailService emailService,
        ILogger<BirthdayReminderJob> logger)
    {
        _contactRepository = contactRepository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var contacts = await _contactRepository.GetUpcomingBirthdaysAsync(daysAhead: 7);

        if (contacts.Count == 0) return;

        _logger.LogInformation("Sending birthday reminders for {Count} contact(s)", contacts.Count);

        foreach (var contact in contacts)
        {
            if (contact.Email is null) continue;

            try
            {
                await _emailService.SendBirthdayReminderAsync(
                    new BirthdayEmailDto(contact.Email, contact.FullName, contact.Birthday));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send birthday reminder for contact {ContactId}", contact.Id);
            }
        }
    }
}
