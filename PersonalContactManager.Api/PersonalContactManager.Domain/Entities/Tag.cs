using PersonalContactManager.Domain.Common;

namespace PersonalContactManager.Domain.Entities;

public sealed class Tag : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Color { get; private set; }

    private Tag() { }

    private Tag(string name, string? color)
    {
        Name = name;
        Color = color;
    }

    public static Tag Create(string name, string? color = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tag name cannot be empty.", nameof(name));

        return new Tag(name.Trim(), color?.Trim());
    }

    public void Update(string name, string? color)
    {
        Name = name.Trim();
        Color = color?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}
