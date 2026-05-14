using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalContactManager.Domain.Entities;

namespace PersonalContactManager.Infrastructure.Persistence.Configurations;

public sealed class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contacts");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(c => c.LastName).HasMaxLength(100).IsRequired();
        builder.Property(c => c.Email).HasMaxLength(255);
        builder.Property(c => c.Iban).HasMaxLength(34).IsRequired();
        builder.Property(c => c.Notes).HasMaxLength(2000);

        builder.HasIndex(c => c.Email).IsUnique().HasFilter("[Email] IS NOT NULL");
        builder.HasIndex(c => new { c.LastName, c.FirstName });

        // Address as owned type — columns on the Contacts table, all nullable
        builder.OwnsOne(c => c.Address, a =>
        {
            a.Property(p => p.Street).HasColumnName("Address_Street").HasMaxLength(200).IsRequired();
            a.Property(p => p.City).HasColumnName("Address_City").HasMaxLength(100).IsRequired();
            a.Property(p => p.State).HasColumnName("Address_State").HasMaxLength(100);
            a.Property(p => p.PostalCode).HasColumnName("Address_PostalCode").HasMaxLength(20).IsRequired();
            a.Property(p => p.Country).HasColumnName("Address_Country").HasMaxLength(100).IsRequired();
        });

        // PhoneNumbers as owned entity collection — separate table
        builder.OwnsMany(c => c.PhoneNumbers, pn =>
        {
            pn.ToTable("ContactPhoneNumbers");
            pn.WithOwner().HasForeignKey("ContactId");
            pn.Property(p => p.Number).HasMaxLength(20).IsRequired();
            pn.Property(p => p.Label).HasMaxLength(50).IsRequired();
            pn.HasKey("ContactId", "Number", "Label");
        });

        // Soft delete global query filter
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
