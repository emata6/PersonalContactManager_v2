using PersonalContactManager.Application.Common.Interfaces;
using PersonalContactManager.Application.DTOs;

namespace PersonalContactManager.Application.Reminders.Queries.GetUpcomingReminders;

public sealed record GetUpcomingRemindersQuery(int DaysAhead = 7) : IQuery<List<ReminderDto>>;
