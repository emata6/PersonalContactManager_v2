using Mapster;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.Common.Exceptions;
using PersonalContactManager.Application.DTOs;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Contacts.Queries.GetContactById;

public sealed class GetContactByIdQueryHandler : IRequestHandler<GetContactByIdQuery, ContactDetailDto>
{
    private readonly IContactRepository _contactRepository;
    private readonly HybridCache _cache;

    public GetContactByIdQueryHandler(IContactRepository contactRepository, HybridCache cache)
    {
        _contactRepository = contactRepository;
        _cache = cache;
    }

    public async Task<ContactDetailDto> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
    {
        var dto = await _cache.GetOrCreateAsync(
            $"contact:{request.Id}",
            async ct =>
            {
                var contact = await _contactRepository.GetByIdAsync(request.Id, ct)
                    ?? throw new NotFoundException("Contact", request.Id);

                return contact.Adapt<ContactDetailDto>();
            },
            tags: ["contacts"],
            cancellationToken: cancellationToken);

        return dto;
    }
}
