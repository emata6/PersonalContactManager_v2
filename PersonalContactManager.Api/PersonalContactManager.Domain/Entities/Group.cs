using PersonalContactManager.Domain.Common;

namespace PersonalContactManager.Domain.Entities;

public sealed class Group : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    private Group() { }

    private Group(string name, string? description)
    {
        Name = name;
        Description = description;
    }

    public static Group Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Group name cannot be empty.", nameof(name));

        return new Group(name.Trim(), description?.Trim());
    }

    public void Update(string name, string? description)
    {
        Name = name.Trim();
        Description = description?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}
