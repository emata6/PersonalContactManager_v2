using PersonalContactManager.Application.Common.Interfaces;
using PersonalContactManager.Application.DTOs;

namespace PersonalContactManager.Application.Tags.Queries.GetAllTags;

public sealed record GetAllTagsQuery : IQuery<List<TagDto>>;
