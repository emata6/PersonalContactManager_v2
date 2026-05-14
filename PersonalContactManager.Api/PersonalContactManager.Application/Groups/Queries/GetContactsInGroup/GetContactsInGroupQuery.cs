using PersonalContactManager.Application.Common.Interfaces;
using PersonalContactManager.Application.DTOs;

namespace PersonalContactManager.Application.Groups.Queries.GetContactsInGroup;

public sealed record GetContactsInGroupQuery(Guid GroupId) : IQuery<List<ContactSummaryDto>>;
