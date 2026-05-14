using PersonalContactManager.Application.Common.Interfaces;
using PersonalContactManager.Application.DTOs;

namespace PersonalContactManager.Application.Contacts.Queries.GetContacts;

public sealed record GetContactsQuery(
    string? SearchTerm,
    Guid? TagId,
    Guid? GroupId,
    bool? FavoritesOnly,
    bool? HasBirthday,
    string? SortBy,
    string? SortDirection,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResultDto<ContactSummaryDto>>;
