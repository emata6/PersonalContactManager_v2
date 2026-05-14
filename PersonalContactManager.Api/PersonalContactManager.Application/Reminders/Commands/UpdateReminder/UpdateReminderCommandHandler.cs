using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.Common.Exceptions;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Reminders.Commands.UpdateReminder;

public sealed class UpdateReminderCommandHandler : IRequestHandler<UpdateReminderCommand>
{
    private readonly IReminderRepository _reminderRepository;
    private readonly HybridCache _cache;

    public UpdateReminderCommandHandler(IReminderRepository reminderRepository, HybridCache cache)
    {
        _reminderRepository = reminderRepository;
        _cache = cache;
    }

    public async Task Handle(UpdateReminderCommand request, CancellationToken cancellationToken)
    {
        var reminder = await _reminderRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Reminder", request.Id);

        reminder.Update(request.Title, request.Note, request.DueAt, request.RecurrenceRule, request.Channel);
        _reminderRepository.Update(reminder);

        await _cache.RemoveByTagAsync("reminders", cancellationToken);
    }
}
