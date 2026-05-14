using FluentValidation;

namespace PersonalContactManager.Application.Contacts.Commands.SendContactEmail;

public sealed class SendContactEmailCommandValidator : AbstractValidator<SendContactEmailCommand>
{
    public SendContactEmailCommandValidator()
    {
        RuleFor(x => x.ContactId).NotEmpty().WithMessage("Contact ID is required.");

        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("Subject is required.")
            .MaximumLength(255).WithMessage("Subject must not exceed 255 characters.");

        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("Email body is required.")
            .MaximumLength(10000).WithMessage("Email body must not exceed 10000 characters.");

        RuleFor(x => x.ReplyTo)
            .EmailAddress().WithMessage("Reply-to must be a valid email address.")
            .When(x => x.ReplyTo is not null);
    }
}
