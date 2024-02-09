namespace FlexFetcher.Models.Queries;

public class DataFilter : DataFilters
{
    public string? Operator { get; set; }
    public string? Field { get; set; }
    public object? Value { get; set; }

    public override string ToString()
    {
        return $"{Field} {Operator} {Value}";
    }
}