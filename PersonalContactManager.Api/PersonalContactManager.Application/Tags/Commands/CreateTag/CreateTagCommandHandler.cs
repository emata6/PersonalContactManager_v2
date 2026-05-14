using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.Common.Exceptions;
using PersonalContactManager.Domain.Entities;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Tags.Commands.CreateTag;

public sealed class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, Guid>
{
    private readonly ITagRepository _tagRepository;
    private readonly HybridCache _cache;

    public CreateTagCommandHandler(ITagRepository tagRepository, HybridCache cache)
    {
        _tagRepository = tagRepository;
        _cache = cache;
    }

    public async Task<Guid> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var existing = await _tagRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existing is not null)
            throw new ConflictException($"A tag named '{request.Name}' already exists.");

        var tag = Tag.Create(request.Name, request.Color);
        await _tagRepository.AddAsync(tag, cancellationToken);

        await _cache.RemoveByTagAsync("tags", cancellationToken);

        return tag.Id;
    }
}
