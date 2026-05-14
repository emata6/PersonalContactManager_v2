using PersonalContactManager.Application.Common.Interfaces;
using PersonalContactManager.Application.DTOs;

namespace PersonalContactManager.Application.Contacts.Queries.GetFavoriteContacts;

public sealed record GetFavoriteContactsQuery : IQuery<List<ContactSummaryDto>>;
