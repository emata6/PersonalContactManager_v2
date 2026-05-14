using PersonalContactManager.Domain.Common;
using PersonalContactManager.Domain.Entities;

namespace PersonalContactManager.Domain.Repositories;

public sealed record ContactSearchParams(
    string? SearchTerm,
    Guid? TagId,
    Guid? GroupId,
    bool? FavoritesOnly,
    bool? HasBirthday,
    string? SortBy,
    string? SortDirection,
    int Page,
    int PageSize);

public interface IContactRepository
{
    Task<Contact?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Contact?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<PagedResult<Contact>> SearchAsync(ContactSearchParams parameters, CancellationToken cancellationToken = default);
    Task<List<Contact>> GetUpcomingBirthdaysAsync(int daysAhead, CancellationToken cancellationToken = default);
    Task<List<Contact>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(int Total, int Favorites)> GetCountsAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Contact contact, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Contact> contacts, CancellationToken cancellationToken = default);
    void Update(Contact contact);
    void Remove(Contact contact);
}
