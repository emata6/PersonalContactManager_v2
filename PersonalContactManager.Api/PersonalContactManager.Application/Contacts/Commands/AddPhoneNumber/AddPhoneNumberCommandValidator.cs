using FluentValidation;

namespace PersonalContactManager.Application.Contacts.Commands.AddPhoneNumber;

public sealed class AddPhoneNumberCommandValidator : AbstractValidator<AddPhoneNumberCommand>
{
    public AddPhoneNumberCommandValidator()
    {
        RuleFor(x => x.ContactId).NotEmpty().WithMessage("Contact ID is required.");

        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters.");

        RuleFor(x => x.Label)
            .NotEmpty().WithMessage("Phone number label is required.")
            .MaximumLength(50).WithMessage("Label must not exceed 50 characters.");
    }
}
