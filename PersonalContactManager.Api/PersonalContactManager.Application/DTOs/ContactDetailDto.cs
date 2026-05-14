namespace PersonalContactManager.Application.DTOs;

public sealed class ContactDetailDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Iban { get; set; }
    public DateOnly? Birthday { get; set; }
    public string? Notes { get; set; }
    public bool IsFavorite { get; set; }
    public AddressDto? Address { get; set; }
    public List<PhoneNumberDto> PhoneNumbers { get; set; } = [];
    public List<TagDto> Tags { get; set; } = [];
    public List<GroupDto> Groups { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
