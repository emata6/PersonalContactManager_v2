using PersonalContactManager.Domain.Common;
using PersonalContactManager.Domain.Events;
using PersonalContactManager.Domain.ValueObjects;

namespace PersonalContactManager.Domain.Entities;

public sealed class Contact : AggregateRoot
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public DateOnly Birthday { get; private set; }
    public string? Notes { get; private set; }
    public string Iban { get; private set; } = string.Empty;
    public bool IsFavorite { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Address? Address { get; private set; }

    private readonly List<PhoneNumber> _phoneNumbers = [];
    public IReadOnlyList<PhoneNumber> PhoneNumbers => _phoneNumbers.AsReadOnly();

    private readonly List<ContactTag> _tags = [];
    public IReadOnlyList<ContactTag> Tags => _tags.AsReadOnly();

    private readonly List<ContactGroup> _groups = [];
    public IReadOnlyList<ContactGroup> Groups => _groups.AsReadOnly();

    public string FullName => $"{FirstName} {LastName}".Trim();

    private Contact() { }

    private Contact(string firstName, string lastName, string? email, DateOnly birthday, string? notes, string iban)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Birthday = birthday;
        Notes = notes;
        Iban = iban;

        AddDomainEvent(new ContactCreatedEvent(Id, FullName, Email, CreatedAt));
    }

    public static Contact Create(
        string firstName,
        string lastName,
        DateOnly birthday,
        string iban,
        string? email = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required.", nameof(lastName));
        if (string.IsNullOrWhiteSpace(iban))
            throw new ArgumentException("IBAN is required.", nameof(iban));

        return new Contact(
            firstName.Trim(),
            lastName.Trim(),
            email?.Trim().ToLowerInvariant(),
            birthday,
            notes?.Trim(),
            NormaliseIban(iban)!);
    }

    public void Update(string firstName, string lastName, string? email, DateOnly birthday, string? notes, string iban)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email?.Trim().ToLowerInvariant();
        Birthday = birthday;
        Notes = notes?.Trim();
        Iban = NormaliseIban(iban)!;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ContactUpdatedEvent(Id, FullName, UpdatedAt));
    }

    public void SetAddress(Address? address)
    {
        Address = address;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddPhoneNumber(PhoneNumber phoneNumber)
    {
        if (_phoneNumbers.Contains(phoneNumber))
            throw new InvalidOperationException($"Phone number '{phoneNumber.Number}' with label '{phoneNumber.Label}' already exists.");

        _phoneNumbers.Add(phoneNumber);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemovePhoneNumber(PhoneNumber phoneNumber)
    {
        _phoneNumbers.Remove(phoneNumber);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignTag(Guid tagId)
    {
        if (_tags.Any(t => t.TagId == tagId))
            return;

        _tags.Add(new ContactTag(Id, tagId));
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveTag(Guid tagId)
    {
        var existing = _tags.FirstOrDefault(t => t.TagId == tagId);
        if (existing is not null)
        {
            _tags.Remove(existing);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void AssignGroup(Guid groupId)
    {
        if (_groups.Any(g => g.GroupId == groupId))
            return;

        _groups.Add(new ContactGroup(Id, groupId));
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveGroup(Guid groupId)
    {
        var existing = _groups.FirstOrDefault(g => g.GroupId == groupId);
        if (existing is not null)
        {
            _groups.Remove(existing);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void ToggleFavorite()
    {
        IsFavorite = !IsFavorite;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        if (IsDeleted) return;

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ContactDeletedEvent(Id, DeletedAt.Value));
    }

    public void Restore()
    {
        if (!IsDeleted) return;

        IsDeleted = false;
        DeletedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    // Strips spaces and uppercases — IBANs are case-insensitive and often typed with spaces
    private static string? NormaliseIban(string? iban) =>
        string.IsNullOrWhiteSpace(iban) ? null : iban.Replace(" ", "").ToUpperInvariant();
}
