using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.Common.Exceptions;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Reminders.Commands.CancelReminder;

public sealed class CancelReminderCommandHandler : IRequestHandler<CancelReminderCommand>
{
    private readonly IReminderRepository _reminderRepository;
    private readonly HybridCache _cache;

    public CancelReminderCommandHandler(IReminderRepository reminderRepository, HybridCache cache)
    {
        _reminderRepository = reminderRepository;
        _cache = cache;
    }

    public async Task Handle(CancelReminderCommand request, CancellationToken cancellationToken)
    {
        var reminder = await _reminderRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Reminder", request.Id);

        reminder.Cancel();
        _reminderRepository.Update(reminder);

        await _cache.RemoveByTagAsync("reminders", cancellationToken);
    }
}
