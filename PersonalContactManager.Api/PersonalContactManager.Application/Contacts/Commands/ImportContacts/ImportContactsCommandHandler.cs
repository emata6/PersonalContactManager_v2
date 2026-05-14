using MediatR;
using PersonalContactManager.Domain.Entities;
using PersonalContactManager.Domain.Repositories;
using PersonalContactManager.Domain.ValueObjects;

namespace PersonalContactManager.Application.Contacts.Commands.ImportContacts;

public sealed class ImportContactsCommandHandler(IContactRepository contactRepository)
    : IRequestHandler<ImportContactsCommand, int>
{
    public async Task<int> Handle(ImportContactsCommand request, CancellationToken cancellationToken)
    {
        var lines = request.CsvContent
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.TrimEnd('\r'))
            .ToArray();

        var dataLines = lines.Length > 0 && lines[0].StartsWith("FirstName", StringComparison.OrdinalIgnoreCase)
            ? lines.Skip(1)
            : lines;

        var contacts = new List<Contact>();

        foreach (var line in dataLines)
        {
            var cols = ParseCsvLine(line);

            var firstName  = cols.ElementAtOrDefault(0)?.Trim() ?? string.Empty;
            var lastName   = cols.ElementAtOrDefault(1)?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName)) continue;

            var email      = cols.ElementAtOrDefault(2)?.Trim();
            var birthdayRaw= cols.ElementAtOrDefault(3)?.Trim();
            var notes      = cols.ElementAtOrDefault(4)?.Trim();
            var iban       = cols.ElementAtOrDefault(5)?.Trim();
            var phone      = cols.ElementAtOrDefault(6)?.Trim();
            var phoneLabel = cols.ElementAtOrDefault(7)?.Trim();
            var street     = cols.ElementAtOrDefault(8)?.Trim();
            var city       = cols.ElementAtOrDefault(9)?.Trim();
            var state      = cols.ElementAtOrDefault(10)?.Trim();
            var postalCode = cols.ElementAtOrDefault(11)?.Trim();
            var country    = cols.ElementAtOrDefault(12)?.Trim();

            if (!DateOnly.TryParse(birthdayRaw, out var birthday)) continue;
            if (string.IsNullOrWhiteSpace(iban)) continue;
            if (string.IsNullOrWhiteSpace(phone)) continue;
            if (string.IsNullOrWhiteSpace(street) || string.IsNullOrWhiteSpace(city) ||
                string.IsNullOrWhiteSpace(postalCode) || string.IsNullOrWhiteSpace(country)) continue;

            var contact = Contact.Create(
                firstName, lastName,
                birthday,
                iban,
                string.IsNullOrEmpty(email) ? null : email,
                string.IsNullOrEmpty(notes) ? null : notes);

            contact.AddPhoneNumber(PhoneNumber.Create(phone, string.IsNullOrWhiteSpace(phoneLabel) ? "Mobile" : phoneLabel));
            contact.SetAddress(Address.Create(street, city, string.IsNullOrEmpty(state) ? null : state, postalCode, country));

            contacts.Add(contact);
        }

        if (contacts.Count > 0)
            await contactRepository.AddRangeAsync(contacts, cancellationToken);

        return contacts.Count;
    }

    private static string[] ParseCsvLine(string line)
    {
        var result = new List<string>();
        var current = new System.Text.StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var ch = line[i];
            if (ch == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (ch == ',' && !inQuotes)
            {
                result.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(ch);
            }
        }
        result.Add(current.ToString());
        return result.ToArray();
    }
}
