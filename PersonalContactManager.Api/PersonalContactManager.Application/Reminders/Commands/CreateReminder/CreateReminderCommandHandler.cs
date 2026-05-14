using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.Common.Exceptions;
using PersonalContactManager.Domain.Entities;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Reminders.Commands.CreateReminder;

public sealed class CreateReminderCommandHandler : IRequestHandler<CreateReminderCommand, Guid>
{
    private readonly IReminderRepository _reminderRepository;
    private readonly IContactRepository _contactRepository;
    private readonly HybridCache _cache;

    public CreateReminderCommandHandler(
        IReminderRepository reminderRepository,
        IContactRepository contactRepository,
        HybridCache cache)
    {
        _reminderRepository = reminderRepository;
        _contactRepository = contactRepository;
        _cache = cache;
    }

    public async Task<Guid> Handle(CreateReminderCommand request, CancellationToken cancellationToken)
    {
        var contactExists = await _contactRepository.GetByIdAsync(request.ContactId, cancellationToken);
        if (contactExists is null)
            throw new NotFoundException("Contact", request.ContactId);

        var reminder = Reminder.Create(
            request.ContactId,
            request.Title,
            request.Note,
            request.DueAt,
            request.RecurrenceRule,
            request.Channel);

        await _reminderRepository.AddAsync(reminder, cancellationToken);

        await _cache.RemoveByTagAsync("reminders", cancellationToken);

        return reminder.Id;
    }
}
