using PersonalContactManager.Application.Common.Interfaces;

namespace PersonalContactManager.Application.Contacts.Commands.ImportContacts;

public sealed record ImportContactsCommand(string CsvContent) : ICommand<int>;
