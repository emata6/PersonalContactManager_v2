using PersonalContactManager.Application.Common.Interfaces;
using PersonalContactManager.Application.DTOs;

namespace PersonalContactManager.Application.Reminders.Queries.GetRemindersByContact;

public sealed record GetRemindersByContactQuery(Guid ContactId) : IQuery<List<ReminderDto>>;
