namespace PersonalContactManager.Application.DTOs;

public sealed record ContactStatsDto(
    int Total,
    int Favorites,
    int UpcomingBirthdays,
    int PendingReminders);
