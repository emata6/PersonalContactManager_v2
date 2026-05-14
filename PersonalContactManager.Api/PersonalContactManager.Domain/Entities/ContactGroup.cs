namespace PersonalContactManager.Domain.Entities;

public sealed class ContactGroup
{
    public Guid ContactId { get; private set; }
    public Guid GroupId { get; private set; }
    public Group Group { get; private set; } = null!;

    private ContactGroup() { }

    public ContactGroup(Guid contactId, Guid groupId)
    {
        ContactId = contactId;
        GroupId = groupId;
    }
}
