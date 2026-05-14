namespace PersonalContactManager.Api.Requests;

public sealed record CreateContactRequest(
    string FirstName,
    string LastName,
    string? Email,
    DateOnly Birthday,
    string? Notes,
    string Iban,
    PhoneRequest Phone,
    AddressRequest Address);

public sealed record UpdateContactRequest(
    string FirstName,
    string LastName,
    string? Email,
    DateOnly Birthday,
    string? Notes,
    string Iban,
    AddressRequest Address);

public sealed record PhoneRequest(string Number, string Label);

public sealed record AddressRequest(
    string Street,
    string City,
    string? State,
    string PostalCode,
    string Country);

public sealed record PhoneNumberRequest(string Number, string Label);

public sealed record SendEmailRequest(string Subject, string Body, string? ReplyTo);

public sealed record ImportContactsRequest(string CsvContent);
