using MediatR;
using Microsoft.AspNetCore.Mvc;
using PersonalContactManager.Application.Contacts.Commands.AddPhoneNumber;
using PersonalContactManager.Application.Contacts.Commands.AssignGroup;
using PersonalContactManager.Application.Contacts.Commands.AssignTag;
using PersonalContactManager.Application.Contacts.Commands.CreateContact;
using PersonalContactManager.Application.Contacts.Commands.DeleteContact;
using PersonalContactManager.Application.Contacts.Commands.RemoveGroup;
using PersonalContactManager.Application.Contacts.Commands.RemovePhoneNumber;
using PersonalContactManager.Application.Contacts.Commands.RemoveTag;
using PersonalContactManager.Application.Contacts.Commands.RestoreContact;
using PersonalContactManager.Application.Contacts.Commands.SendContactEmail;
using PersonalContactManager.Application.Contacts.Commands.ToggleFavorite;
using PersonalContactManager.Application.Contacts.Commands.UpdateContact;
using PersonalContactManager.Application.Contacts.Commands.ImportContacts;
using PersonalContactManager.Application.DTOs;
using PersonalContactManager.Application.Contacts.Queries.ExportContactsCsv;
using PersonalContactManager.Application.Contacts.Queries.GetContactById;
using PersonalContactManager.Application.Contacts.Queries.GetContactStats;
using PersonalContactManager.Application.Contacts.Queries.GetContacts;
using PersonalContactManager.Application.Contacts.Queries.GetFavoriteContacts;
using PersonalContactManager.Application.Contacts.Queries.GetUpcomingBirthdays;

using PersonalContactManager.Api.Requests;

namespace PersonalContactManager.Api.Controllers;

[ApiController]
[Route("api/contacts")]
public sealed class ContactsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? searchTerm,
        [FromQuery] Guid? tagId,
        [FromQuery] Guid? groupId,
        [FromQuery] bool? favoritesOnly,
        [FromQuery] bool? hasBirthday,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDirection,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetContactsQuery(searchTerm, tagId, groupId, favoritesOnly, hasBirthday, sortBy, sortDirection, page, pageSize),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetContactStatsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export(CancellationToken cancellationToken)
    {
        var bytes = await sender.Send(new ExportContactsCsvQuery(), cancellationToken);
        return File(bytes, "text/csv", $"contacts_{DateTime.UtcNow:yyyyMMdd}.csv");
    }

    [HttpPost("import")]
    public async Task<IActionResult> Import([FromBody] ImportContactsRequest request, CancellationToken cancellationToken)
    {
        var count = await sender.Send(new ImportContactsCommand(request.CsvContent), cancellationToken);
        return Ok(new { imported = count });
    }

    [HttpGet("favorites")]
    public async Task<IActionResult> GetFavorites(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetFavoriteContactsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("birthdays")]
    public async Task<IActionResult> GetUpcomingBirthdays(
        [FromQuery] int daysAhead = 7,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetUpcomingBirthdaysQuery(daysAhead), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetContactByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateContactRequest request,
        CancellationToken cancellationToken)
    {
        var id = await sender.Send(
            new CreateContactCommand(
                request.FirstName, request.LastName, request.Email,
                request.Birthday, request.Notes, request.Iban,
                new PhoneInput(request.Phone.Number, request.Phone.Label),
                new AddressInput(request.Address.Street, request.Address.City, request.Address.State,
                    request.Address.PostalCode, request.Address.Country)),
            cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateContactRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new UpdateContactCommand(
                id, request.FirstName, request.LastName, request.Email,
                request.Birthday, request.Notes, request.Iban,
                new AddressInput(request.Address.Street, request.Address.City, request.Address.State,
                    request.Address.PostalCode, request.Address.Country)),
            cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteContactCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/restore")]
    public async Task<IActionResult> Restore(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new RestoreContactCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:guid}/favorite")]
    public async Task<IActionResult> ToggleFavorite(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new ToggleFavoriteCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/tags/{tagId:guid}")]
    public async Task<IActionResult> AssignTag(Guid id, Guid tagId, CancellationToken cancellationToken)
    {
        await sender.Send(new AssignTagCommand(id, tagId), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}/tags/{tagId:guid}")]
    public async Task<IActionResult> RemoveTag(Guid id, Guid tagId, CancellationToken cancellationToken)
    {
        await sender.Send(new RemoveTagCommand(id, tagId), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/groups/{groupId:guid}")]
    public async Task<IActionResult> AssignGroup(Guid id, Guid groupId, CancellationToken cancellationToken)
    {
        await sender.Send(new AssignGroupCommand(id, groupId), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}/groups/{groupId:guid}")]
    public async Task<IActionResult> RemoveGroup(Guid id, Guid groupId, CancellationToken cancellationToken)
    {
        await sender.Send(new RemoveGroupCommand(id, groupId), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/phone-numbers")]
    public async Task<IActionResult> AddPhoneNumber(
        Guid id,
        [FromBody] PhoneNumberRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(new AddPhoneNumberCommand(id, request.Number, request.Label), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}/phone-numbers")]
    public async Task<IActionResult> RemovePhoneNumber(
        Guid id,
        [FromBody] PhoneNumberRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(new RemovePhoneNumberCommand(id, request.Number, request.Label), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/email")]
    public async Task<IActionResult> SendEmail(
        Guid id,
        [FromBody] SendEmailRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new SendContactEmailCommand(id, request.Subject, request.Body, request.ReplyTo),
            cancellationToken);
        return NoContent();
    }
}

