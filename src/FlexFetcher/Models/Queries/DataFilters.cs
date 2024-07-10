namespace FlexFetcher.Models.Queries;

public record DataFilters
{
    public string? Logic { get; set; }
    public List<DataFilter>? Filters { get; set; }
}