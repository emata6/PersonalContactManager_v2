namespace PersonalContactManager.Api.Requests;

public sealed record CreateTagRequest(string Name, string? Color);
public sealed record UpdateTagRequest(string Name, string? Color);
