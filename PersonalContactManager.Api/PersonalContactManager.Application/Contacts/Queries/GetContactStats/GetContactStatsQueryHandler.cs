using MediatR;
using PersonalContactManager.Application.DTOs;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Contacts.Queries.GetContactStats;

public sealed class GetContactStatsQueryHandler(
    IContactRepository contactRepository,
    IReminderRepository reminderRepository)
    : IRequestHandler<GetContactStatsQuery, ContactStatsDto>
{
    public async Task<ContactStatsDto> Handle(GetContactStatsQuery request, CancellationToken cancellationToken)
    {
        var (total, favorites) = await contactRepository.GetCountsAsync(cancellationToken);
        var birthdays = await contactRepository.GetUpcomingBirthdaysAsync(7, cancellationToken);
        var pendingReminders = await reminderRepository.CountPendingAsync(cancellationToken);

        return new ContactStatsDto(total, favorites, birthdays.Count, pendingReminders);
    }
}
