namespace PersonalContactManager.Api.Requests;

public sealed record CreateGroupRequest(string Name, string? Description);
public sealed record UpdateGroupRequest(string Name, string? Description);
