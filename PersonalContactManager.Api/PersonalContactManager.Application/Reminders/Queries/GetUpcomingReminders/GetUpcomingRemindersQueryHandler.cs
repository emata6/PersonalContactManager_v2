using Mapster;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.DTOs;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Reminders.Queries.GetUpcomingReminders;

public sealed class GetUpcomingRemindersQueryHandler : IRequestHandler<GetUpcomingRemindersQuery, List<ReminderDto>>
{
    private readonly IReminderRepository _reminderRepository;
    private readonly HybridCache _cache;

    public GetUpcomingRemindersQueryHandler(IReminderRepository reminderRepository, HybridCache cache)
    {
        _reminderRepository = reminderRepository;
        _cache = cache;
    }

    public async Task<List<ReminderDto>> Handle(GetUpcomingRemindersQuery request, CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            $"reminders:upcoming:{request.DaysAhead}",
            async ct =>
            {
                var reminders = await _reminderRepository.GetUpcomingAsync(request.DaysAhead, ct);
                return reminders.Adapt<List<ReminderDto>>();
            },
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5) },
            tags: ["reminders"],
            cancellationToken: cancellationToken);
    }
}
