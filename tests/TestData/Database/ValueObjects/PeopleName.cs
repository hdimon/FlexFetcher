namespace TestData.Database.ValueObjects;

public class PeopleName : ValueObjectBase
{
    public const int MinLength = 4;
    public const int MaxLength = 200;

    public string Value { get; }

    public PeopleName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("People name cannot be empty or whitespace.");

        if (value.Length is < MinLength or > MaxLength)
            throw new ArgumentException($"People name length must be between {MinLength} and {MaxLength}.");

        Value = value;
    }

    public static implicit operator string(PeopleName name) => name.Value;

    public override string ToString()
    {
        return Value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}