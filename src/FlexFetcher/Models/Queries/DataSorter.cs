namespace FlexFetcher.Models.Queries;

public record DataSorter
{
    public string? Field { get; set; }
    public string? Direction { get; set; }

    public override string ToString()
    {
        return $"{Field} {Direction}";
    }
}