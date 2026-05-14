using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonalContactManager.Domain.Enums;
using PersonalContactManager.Infrastructure.Persistence;

namespace PersonalContactManager.Infrastructure.BackgroundJobs;

public sealed class StaleReminderCleanupJob
{
    private readonly AppDbContext _context;
    private readonly ILogger<StaleReminderCleanupJob> _logger;

    public StaleReminderCleanupJob(AppDbContext context, ILogger<StaleReminderCleanupJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var cutoff = DateTime.UtcNow.AddDays(-90);

        var stale = await _context.Reminders
            .Where(r =>
                (r.Status == ReminderStatus.Fired || r.Status == ReminderStatus.Dismissed) &&
                r.UpdatedAt < cutoff)
            .ToListAsync();

        if (stale.Count == 0) return;

        _context.Reminders.RemoveRange(stale);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Hard-deleted {Count} stale reminder(s) older than {Cutoff}", stale.Count, cutoff);
    }
}
