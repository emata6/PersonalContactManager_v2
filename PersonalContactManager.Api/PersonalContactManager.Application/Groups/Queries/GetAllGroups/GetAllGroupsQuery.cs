using PersonalContactManager.Application.Common.Interfaces;
using PersonalContactManager.Application.DTOs;

namespace PersonalContactManager.Application.Groups.Queries.GetAllGroups;

public sealed record GetAllGroupsQuery : IQuery<List<GroupDto>>;
