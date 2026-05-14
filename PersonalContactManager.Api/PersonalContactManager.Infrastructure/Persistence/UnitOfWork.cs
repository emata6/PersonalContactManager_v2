using Microsoft.EntityFrameworkCore;
using PersonalContactManager.Application.Interfaces;
using PersonalContactManager.Domain.Common;
using PersonalContactManager.Domain.Interfaces;

namespace PersonalContactManager.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly IEventPublisher _eventPublisher;

    public UnitOfWork(AppDbContext context, IEventPublisher eventPublisher)
    {
        _context = context;
        _eventPublisher = eventPublisher;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Snapshot aggregate roots with pending events before saving
        var aggregates = _context.ChangeTracker
            .Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var result = await _context.SaveChangesAsync(cancellationToken);

        // Dispatch events only after successful DB commit
        foreach (var aggregate in aggregates)
        {
            foreach (var domainEvent in aggregate.DomainEvents)
                await _eventPublisher.PublishAsync(domainEvent, cancellationToken);

            aggregate.ClearDomainEvents();
        }

        return result;
    }
}
