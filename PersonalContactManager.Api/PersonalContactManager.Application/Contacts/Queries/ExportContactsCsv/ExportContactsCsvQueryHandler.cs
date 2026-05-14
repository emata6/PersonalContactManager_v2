using System.Text;
using MediatR;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Contacts.Queries.ExportContactsCsv;

public sealed class ExportContactsCsvQueryHandler(IContactRepository contactRepository)
    : IRequestHandler<ExportContactsCsvQuery, byte[]>
{
    public async Task<byte[]> Handle(ExportContactsCsvQuery request, CancellationToken cancellationToken)
    {
        var contacts = await contactRepository.GetAllAsync(cancellationToken);

        var sb = new StringBuilder();
        sb.AppendLine("FirstName,LastName,Email,Birthday,Notes,Iban,Phone,PhoneLabel,Street,City,State,PostalCode,Country");

        foreach (var c in contacts)
        {
            var phone = c.PhoneNumbers.FirstOrDefault();
            sb.AppendLine(string.Join(",",
                Escape(c.FirstName),
                Escape(c.LastName),
                Escape(c.Email ?? string.Empty),
                c.Birthday.ToString("yyyy-MM-dd"),
                Escape(c.Notes ?? string.Empty),
                Escape(c.Iban),
                Escape(phone?.Number ?? string.Empty),
                Escape(phone?.Label ?? string.Empty),
                Escape(c.Address?.Street ?? string.Empty),
                Escape(c.Address?.City ?? string.Empty),
                Escape(c.Address?.State ?? string.Empty),
                Escape(c.Address?.PostalCode ?? string.Empty),
                Escape(c.Address?.Country ?? string.Empty)));
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static string Escape(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}
