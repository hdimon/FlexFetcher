namespace FlexFetcher.Models.Queries;

public record DataQuery
{
    public DataFilter? Filter { get; set; }
    public DataSorters? Sorters { get; set; }
    public DataPager? Pager { get; set; }
}