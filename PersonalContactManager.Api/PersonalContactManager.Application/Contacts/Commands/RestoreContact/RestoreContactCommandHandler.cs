using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.Common.Exceptions;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Contacts.Commands.RestoreContact;

public sealed class RestoreContactCommandHandler : IRequestHandler<RestoreContactCommand>
{
    private readonly IContactRepository _contactRepository;
    private readonly HybridCache _cache;

    public RestoreContactCommandHandler(IContactRepository contactRepository, HybridCache cache)
    {
        _contactRepository = contactRepository;
        _cache = cache;
    }

    public async Task Handle(RestoreContactCommand request, CancellationToken cancellationToken)
    {
        var contact = await _contactRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Contact", request.Id);

        contact.Restore();
        _contactRepository.Update(contact);

        await _cache.RemoveByTagAsync("contacts", cancellationToken);
    }
}
