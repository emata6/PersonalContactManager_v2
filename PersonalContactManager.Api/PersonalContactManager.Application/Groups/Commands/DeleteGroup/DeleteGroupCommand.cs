using PersonalContactManager.Application.Common.Interfaces;

namespace PersonalContactManager.Application.Groups.Commands.DeleteGroup;

public sealed record DeleteGroupCommand(Guid Id) : ICommand;
