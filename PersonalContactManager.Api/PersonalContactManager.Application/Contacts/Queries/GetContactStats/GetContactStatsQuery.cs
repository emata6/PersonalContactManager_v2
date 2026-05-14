using PersonalContactManager.Application.Common.Interfaces;
using PersonalContactManager.Application.DTOs;

namespace PersonalContactManager.Application.Contacts.Queries.GetContactStats;

public sealed record GetContactStatsQuery : IQuery<ContactStatsDto>;
