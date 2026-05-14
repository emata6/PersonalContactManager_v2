using PersonalContactManager.Application.Common.Interfaces;

namespace PersonalContactManager.Application.Contacts.Commands.ToggleFavorite;

public sealed record ToggleFavoriteCommand(Guid Id) : ICommand;
