using Microsoft.Extensions.Logging;
using PersonalContactManager.Domain.Interfaces;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Infrastructure.BackgroundJobs;

public sealed class ReminderPollingJob
{
    private readonly IReminderRepository _reminderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReminderPollingJob> _logger;

    public ReminderPollingJob(
        IReminderRepository reminderRepository,
        IUnitOfWork unitOfWork,
        ILogger<ReminderPollingJob> logger)
    {
        _reminderRepository = reminderRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var due = await _reminderRepository.GetPendingDueAsync(DateTime.UtcNow);

        if (due.Count == 0) return;

        _logger.LogInformation("Firing {Count} due reminder(s)", due.Count);

        foreach (var reminder in due)
        {
            try
            {
                reminder.Fire();
                _reminderRepository.Update(reminder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fire reminder {ReminderId}", reminder.Id);
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }
}
