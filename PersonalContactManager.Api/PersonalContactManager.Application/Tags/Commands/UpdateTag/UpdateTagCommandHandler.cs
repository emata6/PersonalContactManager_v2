using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.Common.Exceptions;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Tags.Commands.UpdateTag;

public sealed class UpdateTagCommandHandler : IRequestHandler<UpdateTagCommand>
{
    private readonly ITagRepository _tagRepository;
    private readonly HybridCache _cache;

    public UpdateTagCommandHandler(ITagRepository tagRepository, HybridCache cache)
    {
        _tagRepository = tagRepository;
        _cache = cache;
    }

    public async Task Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _tagRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Tag", request.Id);

        tag.Update(request.Name, request.Color);
        await _cache.RemoveByTagAsync("tags", cancellationToken);
        await _cache.RemoveByTagAsync("contacts", cancellationToken);
    }
}
