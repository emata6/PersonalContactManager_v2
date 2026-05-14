using Microsoft.EntityFrameworkCore;
using PersonalContactManager.Domain.Common;
using PersonalContactManager.Domain.Entities;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Infrastructure.Persistence.Repositories;

public sealed class ContactRepository : IContactRepository
{
    private readonly AppDbContext _context;

    public ContactRepository(AppDbContext context) => _context = context;

    public async Task<Contact?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Contacts
            .Include(c => c.PhoneNumbers)
            .Include(c => c.Tags).ThenInclude(ct => ct.Tag)
            .Include(c => c.Groups).ThenInclude(cg => cg.Group)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<Contact?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _context.Contacts
            .FirstOrDefaultAsync(c => c.Email == email.ToLowerInvariant(), cancellationToken);

    public async Task<PagedResult<Contact>> SearchAsync(ContactSearchParams parameters, CancellationToken cancellationToken = default)
    {
        var query = _context.Contacts
            .Include(c => c.PhoneNumbers)
            .Include(c => c.Tags).ThenInclude(ct => ct.Tag)
            .Include(c => c.Groups).ThenInclude(cg => cg.Group)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var term = parameters.SearchTerm.ToLower();
            query = query.Where(c =>
                c.FirstName.ToLower().Contains(term) ||
                c.LastName.ToLower().Contains(term) ||
                (c.Email != null && c.Email.Contains(term)));
        }

        if (parameters.TagId.HasValue)
            query = query.Where(c => c.Tags.Any(ct => ct.TagId == parameters.TagId.Value));

        if (parameters.GroupId.HasValue)
            query = query.Where(c => c.Groups.Any(cg => cg.GroupId == parameters.GroupId.Value));

        if (parameters.FavoritesOnly == true)
            query = query.Where(c => c.IsFavorite);

        if (parameters.HasBirthday == true)
            query = query.Where(c => c.Birthday != default);

        var totalCount = await query.CountAsync(cancellationToken);

        query = parameters.SortBy == "createdAt"
            ? parameters.SortDirection == "desc"
                ? query.OrderByDescending(c => c.CreatedAt).ThenByDescending(c => c.Id)
                : query.OrderBy(c => c.CreatedAt).ThenBy(c => c.Id)
            : parameters.SortDirection == "desc"
                ? query.OrderByDescending(c => c.LastName).ThenByDescending(c => c.FirstName)
                : query.OrderBy(c => c.LastName).ThenBy(c => c.FirstName);

        var items = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Contact>(items, totalCount, parameters.Page, parameters.PageSize);
    }

    public async Task<List<Contact>> GetUpcomingBirthdaysAsync(int daysAhead, CancellationToken cancellationToken = default)
    {
        // Load birthday contacts into memory — year-agnostic comparison not expressible in LINQ-to-SQL
        var contacts = await _context.Contacts.ToListAsync(cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var end = today.AddDays(daysAhead);

        return contacts.Where(c => IsUpcoming(c.Birthday, today, end)).ToList();
    }

    public async Task<List<Contact>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Contacts
            .Include(c => c.PhoneNumbers)
            .Include(c => c.Tags).ThenInclude(ct => ct.Tag)
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ToListAsync(cancellationToken);

    public async Task<(int Total, int Favorites)> GetCountsAsync(CancellationToken cancellationToken = default)
    {
        var total = await _context.Contacts.CountAsync(cancellationToken);
        var favorites = await _context.Contacts.CountAsync(c => c.IsFavorite, cancellationToken);
        return (total, favorites);
    }

    public async Task AddAsync(Contact contact, CancellationToken cancellationToken = default)
        => await _context.Contacts.AddAsync(contact, cancellationToken);

    public async Task AddRangeAsync(IEnumerable<Contact> contacts, CancellationToken cancellationToken = default)
        => await _context.Contacts.AddRangeAsync(contacts, cancellationToken);

    public void Update(Contact contact) => _context.Contacts.Update(contact);

    public void Remove(Contact contact) => _context.Contacts.Remove(contact);

    private static bool IsUpcoming(DateOnly birthday, DateOnly today, DateOnly end)
    {
        // Handle Feb 29 on non-leap years
        int month = birthday.Month, day = birthday.Day;
        if (month == 2 && day == 29 && !DateTime.IsLeapYear(today.Year))
            day = 28;

        var thisYear = new DateOnly(today.Year, month, day);
        var nextYear = new DateOnly(today.Year + 1, month, day);
        return (thisYear >= today && thisYear <= end) || (nextYear >= today && nextYear <= end);
    }
}
