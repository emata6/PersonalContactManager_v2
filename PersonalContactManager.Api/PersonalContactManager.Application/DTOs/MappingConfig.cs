using Mapster;
using PersonalContactManager.Domain.Entities;
using PersonalContactManager.Domain.ValueObjects;

namespace PersonalContactManager.Application.DTOs;

public sealed class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Contact, ContactDetailDto>()
            .Map(dest => dest.FullName, src => src.FullName)
            .Map(dest => dest.PhoneNumbers, src => src.PhoneNumbers.Adapt<List<PhoneNumberDto>>())
            .Map(dest => dest.Tags, src => src.Tags.Select(ct => ct.Tag).Adapt<List<TagDto>>())
            .Map(dest => dest.Groups, src => src.Groups.Select(cg => cg.Group).Adapt<List<GroupDto>>());

        config.NewConfig<Contact, ContactSummaryDto>()
            .Map(dest => dest.FullName, src => src.FullName)
            .Map(dest => dest.Tags, src => src.Tags.Select(ct => ct.Tag).Adapt<List<TagDto>>());

        config.NewConfig<PhoneNumber, PhoneNumberDto>();

        config.NewConfig<Reminder, ReminderDto>()
            .Map(dest => dest.ContactFullName, src => src.Contact.FullName);
    }
}
