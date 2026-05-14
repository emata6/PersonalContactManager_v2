using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalContactManager.Domain.Entities;

namespace PersonalContactManager.Infrastructure.Persistence.Configurations;

public sealed class ContactGroupConfiguration : IEntityTypeConfiguration<ContactGroup>
{
    public void Configure(EntityTypeBuilder<ContactGroup> builder)
    {
        builder.ToTable("ContactGroups");
        builder.HasKey(cg => new { cg.ContactId, cg.GroupId });

        builder.HasOne(cg => cg.Group)
            .WithMany()
            .HasForeignKey(cg => cg.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
