using MediatR;
using Microsoft.AspNetCore.Mvc;
using PersonalContactManager.Application.Groups.Commands.CreateGroup;
using PersonalContactManager.Application.Groups.Commands.DeleteGroup;
using PersonalContactManager.Application.Groups.Commands.UpdateGroup;
using PersonalContactManager.Application.Groups.Queries.GetAllGroups;
using PersonalContactManager.Application.Groups.Queries.GetContactsInGroup;

using PersonalContactManager.Api.Requests;

namespace PersonalContactManager.Api.Controllers;

[ApiController]
[Route("api/groups")]
public sealed class GroupsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllGroupsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}/contacts")]
    public async Task<IActionResult> GetContacts(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetContactsInGroupQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateGroupRequest request,
        CancellationToken cancellationToken)
    {
        var id = await sender.Send(new CreateGroupCommand(request.Name, request.Description), cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateGroupRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(new UpdateGroupCommand(id, request.Name, request.Description), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteGroupCommand(id), cancellationToken);
        return NoContent();
    }
}

