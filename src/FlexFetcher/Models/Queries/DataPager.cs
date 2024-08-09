namespace FlexFetcher.Models.Queries;

public record DataPager
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }
}