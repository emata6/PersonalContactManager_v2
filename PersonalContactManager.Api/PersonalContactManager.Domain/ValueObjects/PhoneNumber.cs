using PersonalContactManager.Domain.Common;

namespace PersonalContactManager.Domain.ValueObjects;

public sealed class PhoneNumber : ValueObject
{
    public string Number { get; private set; } = string.Empty;
    public string Label { get; private set; } = string.Empty;

    private PhoneNumber() { }

    private PhoneNumber(string number, string label)
    {
        Number = number;
        Label = label;
    }

    public static PhoneNumber Create(string number, string label)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Phone number cannot be empty.", nameof(number));

        if (string.IsNullOrWhiteSpace(label))
            throw new ArgumentException("Phone number label cannot be empty.", nameof(label));

        return new PhoneNumber(number.Trim(), label.Trim().ToLowerInvariant());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Number;
        yield return Label;
    }

    public override string ToString() => $"{Label}: {Number}";
}
