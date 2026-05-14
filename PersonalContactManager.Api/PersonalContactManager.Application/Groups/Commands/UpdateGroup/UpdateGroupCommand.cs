using PersonalContactManager.Application.Common.Interfaces;

namespace PersonalContactManager.Application.Groups.Commands.UpdateGroup;

public sealed record UpdateGroupCommand(Guid Id, string Name, string? Description) : ICommand;
