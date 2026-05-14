namespace PersonalContactManager.Application.DTOs;

public sealed record AddressInput(
    string Street,
    string City,
    string? State,
    string PostalCode,
    string Country);
