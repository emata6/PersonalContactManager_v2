using PersonalContactManager.Application.Common.Interfaces;

namespace PersonalContactManager.Application.Tags.Commands.UpdateTag;

public sealed record UpdateTagCommand(Guid Id, string Name, string? Color) : ICommand;
