using Microsoft.EntityFrameworkCore;
using PersonalContactManager.Domain.Entities;
using PersonalContactManager.Domain.Enums;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Infrastructure.Persistence.Repositories;

public sealed class ReminderRepository : IReminderRepository
{
    private readonly AppDbContext _context;

    public ReminderRepository(AppDbContext context) => _context = context;

    public async Task<Reminder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Reminders
            .Include(r => r.Contact)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<List<Reminder>> GetByContactIdAsync(Guid contactId, CancellationToken cancellationToken = default)
        => await _context.Reminders
            .Include(r => r.Contact)
            .Where(r => r.ContactId == contactId)
            .OrderBy(r => r.DueAt)
            .ToListAsync(cancellationToken);

    public async Task<List<Reminder>> GetPendingDueAsync(DateTime asOf, CancellationToken cancellationToken = default)
        => await _context.Reminders
            .Include(r => r.Contact)
            .Where(r => r.Status == ReminderStatus.Pending && r.DueAt <= asOf)
            .ToListAsync(cancellationToken);

    public async Task<List<Reminder>> GetUpcomingAsync(int daysAhead, CancellationToken cancellationToken = default)
    {
        var cutoff = DateTime.UtcNow.AddDays(daysAhead);
        return await _context.Reminders
            .Include(r => r.Contact)
            .Where(r => r.Status == ReminderStatus.Pending && r.DueAt >= DateTime.UtcNow && r.DueAt <= cutoff)
            .OrderBy(r => r.DueAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountPendingAsync(CancellationToken cancellationToken = default)
        => await _context.Reminders.CountAsync(r => r.Status == ReminderStatus.Pending, cancellationToken);

    public async Task AddAsync(Reminder reminder, CancellationToken cancellationToken = default)
        => await _context.Reminders.AddAsync(reminder, cancellationToken);

    public void Update(Reminder reminder) => _context.Reminders.Update(reminder);

    public void Remove(Reminder reminder) => _context.Reminders.Remove(reminder);
}
