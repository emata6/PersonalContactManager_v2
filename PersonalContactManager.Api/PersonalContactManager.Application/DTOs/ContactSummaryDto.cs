namespace PersonalContactManager.Application.DTOs;

public sealed class ContactSummaryDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateOnly? Birthday { get; set; }
    public bool IsFavorite { get; set; }
    public List<TagDto> Tags { get; set; } = [];
}
