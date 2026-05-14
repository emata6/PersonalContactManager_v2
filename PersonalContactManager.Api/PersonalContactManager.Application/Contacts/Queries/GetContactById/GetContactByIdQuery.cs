using PersonalContactManager.Application.Common.Interfaces;
using PersonalContactManager.Application.DTOs;

namespace PersonalContactManager.Application.Contacts.Queries.GetContactById;

public sealed record GetContactByIdQuery(Guid Id) : IQuery<ContactDetailDto>;
