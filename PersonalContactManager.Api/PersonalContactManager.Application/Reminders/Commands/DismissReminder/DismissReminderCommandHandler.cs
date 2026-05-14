using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.Common.Exceptions;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Reminders.Commands.DismissReminder;

public sealed class DismissReminderCommandHandler : IRequestHandler<DismissReminderCommand>
{
    private readonly IReminderRepository _reminderRepository;
    private readonly HybridCache _cache;

    public DismissReminderCommandHandler(IReminderRepository reminderRepository, HybridCache cache)
    {
        _reminderRepository = reminderRepository;
        _cache = cache;
    }

    public async Task Handle(DismissReminderCommand request, CancellationToken cancellationToken)
    {
        var reminder = await _reminderRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Reminder", request.Id);

        reminder.Dismiss();
        _reminderRepository.Update(reminder);

        await _cache.RemoveByTagAsync("reminders", cancellationToken);
    }
}
