using PersonalContactManager.Application.Common.Interfaces;

namespace PersonalContactManager.Application.Tags.Commands.CreateTag;

public sealed record CreateTagCommand(string Name, string? Color) : ICommand<Guid>;
