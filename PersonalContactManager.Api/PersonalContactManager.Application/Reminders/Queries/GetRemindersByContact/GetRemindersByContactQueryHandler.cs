using Mapster;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.DTOs;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Reminders.Queries.GetRemindersByContact;

public sealed class GetRemindersByContactQueryHandler : IRequestHandler<GetRemindersByContactQuery, List<ReminderDto>>
{
    private readonly IReminderRepository _reminderRepository;
    private readonly HybridCache _cache;

    public GetRemindersByContactQueryHandler(IReminderRepository reminderRepository, HybridCache cache)
    {
        _reminderRepository = reminderRepository;
        _cache = cache;
    }

    public async Task<List<ReminderDto>> Handle(GetRemindersByContactQuery request, CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            $"reminders:contact:{request.ContactId}",
            async ct =>
            {
                var reminders = await _reminderRepository.GetByContactIdAsync(request.ContactId, ct);
                return reminders.Adapt<List<ReminderDto>>();
            },
            tags: ["reminders"],
            cancellationToken: cancellationToken);
    }
}
