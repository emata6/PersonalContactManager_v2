namespace PersonalContactManager.Application.DTOs;

public sealed class GroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
