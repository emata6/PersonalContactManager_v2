using MediatR;
using Microsoft.AspNetCore.Mvc;
using PersonalContactManager.Application.Tags.Commands.CreateTag;
using PersonalContactManager.Application.Tags.Commands.DeleteTag;
using PersonalContactManager.Application.Tags.Commands.UpdateTag;
using PersonalContactManager.Application.Tags.Queries.GetAllTags;

using PersonalContactManager.Api.Requests;

namespace PersonalContactManager.Api.Controllers;

[ApiController]
[Route("api/tags")]
public sealed class TagsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllTagsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateTagRequest request,
        CancellationToken cancellationToken)
    {
        var id = await sender.Send(new CreateTagCommand(request.Name, request.Color), cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateTagRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(new UpdateTagCommand(id, request.Name, request.Color), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteTagCommand(id), cancellationToken);
        return NoContent();
    }
}

