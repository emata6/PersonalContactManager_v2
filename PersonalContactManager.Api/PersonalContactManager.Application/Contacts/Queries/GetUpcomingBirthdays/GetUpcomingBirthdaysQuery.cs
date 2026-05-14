using PersonalContactManager.Application.Common.Interfaces;
using PersonalContactManager.Application.DTOs;

namespace PersonalContactManager.Application.Contacts.Queries.GetUpcomingBirthdays;

public sealed record GetUpcomingBirthdaysQuery(int DaysAhead = 7) : IQuery<List<ContactSummaryDto>>;
