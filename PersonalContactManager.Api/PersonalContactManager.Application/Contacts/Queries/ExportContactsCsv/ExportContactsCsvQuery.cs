using PersonalContactManager.Application.Common.Interfaces;

namespace PersonalContactManager.Application.Contacts.Queries.ExportContactsCsv;

public sealed record ExportContactsCsvQuery : IQuery<byte[]>;
