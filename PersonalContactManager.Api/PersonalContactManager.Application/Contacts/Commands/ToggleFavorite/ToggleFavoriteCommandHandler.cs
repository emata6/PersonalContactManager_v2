using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.Common.Exceptions;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Contacts.Commands.ToggleFavorite;

public sealed class ToggleFavoriteCommandHandler : IRequestHandler<ToggleFavoriteCommand>
{
    private readonly IContactRepository _contactRepository;
    private readonly HybridCache _cache;

    public ToggleFavoriteCommandHandler(IContactRepository contactRepository, HybridCache cache)
    {
        _contactRepository = contactRepository;
        _cache = cache;
    }

    public async Task Handle(ToggleFavoriteCommand request, CancellationToken cancellationToken)
    {
        var contact = await _contactRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Contact", request.Id);

        contact.ToggleFavorite();
        _contactRepository.Update(contact);

        await _cache.RemoveAsync($"contact:{request.Id}", cancellationToken);
        await _cache.RemoveByTagAsync("contacts", cancellationToken);
    }
}
