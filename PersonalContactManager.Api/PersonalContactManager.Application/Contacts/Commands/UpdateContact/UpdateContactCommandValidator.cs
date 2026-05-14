using FluentValidation;

namespace PersonalContactManager.Application.Contacts.Commands.UpdateContact;

public sealed class UpdateContactCommandValidator : AbstractValidator<UpdateContactCommand>
{
    public UpdateContactCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Contact ID is required.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters.");

        RuleFor(x => x.Birthday)
            .LessThan(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Date of birth must be in the past.");

        RuleFor(x => x.Iban)
            .NotEmpty().WithMessage("IBAN is required.")
            .Matches(@"^[A-Z]{2}[0-9]{2}[A-Za-z0-9]{1,30}$")
            .WithMessage("IBAN must be in a valid format (e.g. GB29NWBK60161331926819).");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters.")
            .When(x => x.Email is not null);

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes must not exceed 2000 characters.")
            .When(x => x.Notes is not null);

        RuleFor(x => x.Address.Street).NotEmpty().WithMessage("Street is required.");
        RuleFor(x => x.Address.City).NotEmpty().WithMessage("City is required.");
        RuleFor(x => x.Address.PostalCode).NotEmpty().WithMessage("Postal code is required.");
        RuleFor(x => x.Address.Country).NotEmpty().WithMessage("Country is required.");
    }
}
