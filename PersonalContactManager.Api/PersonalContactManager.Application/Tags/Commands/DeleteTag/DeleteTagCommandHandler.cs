using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.Common.Exceptions;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Tags.Commands.DeleteTag;

public sealed class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand>
{
    private readonly ITagRepository _tagRepository;
    private readonly HybridCache _cache;

    public DeleteTagCommandHandler(ITagRepository tagRepository, HybridCache cache)
    {
        _tagRepository = tagRepository;
        _cache = cache;
    }

    public async Task Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _tagRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Tag", request.Id);

        _tagRepository.Remove(tag);

        await _cache.RemoveByTagAsync("tags", cancellationToken);
        await _cache.RemoveByTagAsync("contacts", cancellationToken);
    }
}
