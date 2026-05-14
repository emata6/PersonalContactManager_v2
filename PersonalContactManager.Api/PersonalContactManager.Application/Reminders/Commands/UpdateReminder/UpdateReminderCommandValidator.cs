using FluentValidation;

namespace PersonalContactManager.Application.Reminders.Commands.UpdateReminder;

public sealed class UpdateReminderCommandValidator : AbstractValidator<UpdateReminderCommand>
{
    public UpdateReminderCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Reminder ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Reminder title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.DueAt)
            .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future.");

        RuleFor(x => x.Channel)
            .IsInEnum().WithMessage("Invalid reminder channel.");
    }
}
