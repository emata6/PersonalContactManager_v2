using MediatR;
using Microsoft.AspNetCore.Mvc;
using PersonalContactManager.Application.Reminders.Commands.CancelReminder;
using PersonalContactManager.Application.Reminders.Commands.CreateReminder;
using PersonalContactManager.Application.Reminders.Commands.DismissReminder;
using PersonalContactManager.Application.Reminders.Commands.UpdateReminder;
using PersonalContactManager.Application.Reminders.Queries.GetRemindersByContact;
using PersonalContactManager.Application.Reminders.Queries.GetUpcomingReminders;
using PersonalContactManager.Domain.Enums;

using PersonalContactManager.Api.Requests;

namespace PersonalContactManager.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class RemindersController(ISender sender) : ControllerBase
{
    [HttpGet("reminders/upcoming")]
    public async Task<IActionResult> GetUpcoming(
        [FromQuery] int daysAhead = 30,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetUpcomingRemindersQuery(daysAhead), cancellationToken);
        return Ok(result);
    }

    [HttpGet("contacts/{contactId:guid}/reminders")]
    public async Task<IActionResult> GetByContact(Guid contactId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetRemindersByContactQuery(contactId), cancellationToken);
        return Ok(result);
    }

    [HttpPost("contacts/{contactId:guid}/reminders")]
    public async Task<IActionResult> Create(
        Guid contactId,
        [FromBody] CreateReminderRequest request,
        CancellationToken cancellationToken)
    {
        var id = await sender.Send(
            new CreateReminderCommand(
                contactId, request.Title, request.Note,
                request.DueAt, request.RecurrenceRule, request.Channel),
            cancellationToken);
        return CreatedAtAction(nameof(GetByContact), new { contactId }, new { id });
    }

    [HttpPut("reminders/{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateReminderRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new UpdateReminderCommand(
                id, request.Title, request.Note,
                request.DueAt, request.RecurrenceRule, request.Channel),
            cancellationToken);
        return NoContent();
    }

    [HttpDelete("reminders/{id:guid}")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new CancelReminderCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("reminders/{id:guid}/dismiss")]
    public async Task<IActionResult> Dismiss(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DismissReminderCommand(id), cancellationToken);
        return NoContent();
    }
}

