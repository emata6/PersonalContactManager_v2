using PersonalContactManager.Application.Common.Interfaces;

namespace PersonalContactManager.Application.Groups.Commands.CreateGroup;

public sealed record CreateGroupCommand(string Name, string? Description) : ICommand<Guid>;
