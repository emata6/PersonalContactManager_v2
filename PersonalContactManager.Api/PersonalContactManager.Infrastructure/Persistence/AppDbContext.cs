using Microsoft.EntityFrameworkCore;
using PersonalContactManager.Domain.Entities;

namespace PersonalContactManager.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Reminder> Reminders => Set<Reminder>();
    public DbSet<ContactTag> ContactTags => Set<ContactTag>();
    public DbSet<ContactGroup> ContactGroups => Set<ContactGroup>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
