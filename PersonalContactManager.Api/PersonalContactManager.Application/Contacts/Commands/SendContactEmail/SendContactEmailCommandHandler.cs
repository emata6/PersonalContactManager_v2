using MediatR;
using PersonalContactManager.Application.Common.Exceptions;
using PersonalContactManager.Application.Interfaces;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Contacts.Commands.SendContactEmail;

public sealed class SendContactEmailCommandHandler : IRequestHandler<SendContactEmailCommand>
{
    private readonly IContactRepository _contactRepository;
    private readonly IEmailService _emailService;

    public SendContactEmailCommandHandler(IContactRepository contactRepository, IEmailService emailService)
    {
        _contactRepository = contactRepository;
        _emailService = emailService;
    }

    public async Task Handle(SendContactEmailCommand request, CancellationToken cancellationToken)
    {
        var contact = await _contactRepository.GetByIdAsync(request.ContactId, cancellationToken)
            ?? throw new NotFoundException("Contact", request.ContactId);

        if (contact.Email is null)
            throw new ConflictException($"Contact '{contact.FullName}' does not have an email address.");

        await _emailService.SendContactEmailAsync(
            new ContactEmailDto(contact.Email, request.Subject, request.Body, request.ReplyTo),
            cancellationToken);
    }
}
