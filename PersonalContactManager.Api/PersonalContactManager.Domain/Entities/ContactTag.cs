namespace PersonalContactManager.Domain.Entities;

public sealed class ContactTag
{
    public Guid ContactId { get; private set; }
    public Guid TagId { get; private set; }
    public Tag Tag { get; private set; } = null!;

    private ContactTag() { }

    public ContactTag(Guid contactId, Guid tagId)
    {
        ContactId = contactId;
        TagId = tagId;
    }
}
