namespace FlexFetcher.Models.Queries;

public record DataFilter
{
    public string? Logic { get; set; }
    public List<DataFilter>? Filters { get; set; }

    public string? Operator { get; set; }
    public string? Field { get; set; }
    public object? Value { get; set; }

    public override string ToString()
    {
        if (Field is null)
            return $"{Logic?.ToUpper()} {Filters?.Count ?? 0} filters";

        return $"{Field} {Operator} {Value}";
    }
}