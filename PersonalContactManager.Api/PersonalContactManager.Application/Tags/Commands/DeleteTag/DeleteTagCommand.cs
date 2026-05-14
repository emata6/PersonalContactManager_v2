using PersonalContactManager.Application.Common.Interfaces;

namespace PersonalContactManager.Application.Tags.Commands.DeleteTag;

public sealed record DeleteTagCommand(Guid Id) : ICommand;
