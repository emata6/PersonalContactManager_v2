using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonalContactManager.Infrastructure.Persistence;

namespace PersonalContactManager.Infrastructure.BackgroundJobs;

public sealed class SoftDeleteCleanupJob
{
    private readonly AppDbContext _context;
    private readonly ILogger<SoftDeleteCleanupJob> _logger;

    public SoftDeleteCleanupJob(AppDbContext context, ILogger<SoftDeleteCleanupJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var cutoff = DateTime.UtcNow.AddDays(-30);

        // IgnoreQueryFilters bypasses the global soft-delete filter to find deleted contacts
        var stale = await _context.Contacts
            .IgnoreQueryFilters()
            .Where(c => c.IsDeleted && c.DeletedAt < cutoff)
            .ToListAsync();

        if (stale.Count == 0) return;

        _context.Contacts.RemoveRange(stale);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Hard-deleted {Count} contact(s) soft-deleted before {Cutoff}", stale.Count, cutoff);
    }
}
