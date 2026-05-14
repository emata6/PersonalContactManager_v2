using FluentValidation;
using PersonalContactManager.Domain.Enums;

namespace PersonalContactManager.Application.Reminders.Commands.CreateReminder;

public sealed class CreateReminderCommandValidator : AbstractValidator<CreateReminderCommand>
{
    public CreateReminderCommandValidator()
    {
        RuleFor(x => x.ContactId).NotEmpty().WithMessage("Contact ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Reminder title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Note)
            .MaximumLength(1000).WithMessage("Note must not exceed 1000 characters.")
            .When(x => x.Note is not null);

        RuleFor(x => x.DueAt)
            .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future.");

        RuleFor(x => x.Channel)
            .IsInEnum().WithMessage("Invalid reminder channel.");
    }
}
