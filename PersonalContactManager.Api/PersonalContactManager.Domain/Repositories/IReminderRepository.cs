using PersonalContactManager.Domain.Entities;

namespace PersonalContactManager.Domain.Repositories;

public interface IReminderRepository
{
    Task<Reminder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Reminder>> GetByContactIdAsync(Guid contactId, CancellationToken cancellationToken = default);
    Task<List<Reminder>> GetPendingDueAsync(DateTime asOf, CancellationToken cancellationToken = default);
    Task<List<Reminder>> GetUpcomingAsync(int daysAhead, CancellationToken cancellationToken = default);
    Task<int> CountPendingAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Reminder reminder, CancellationToken cancellationToken = default);
    void Update(Reminder reminder);
    void Remove(Reminder reminder);
}
