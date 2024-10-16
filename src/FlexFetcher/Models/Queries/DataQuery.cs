namespace FlexFetcher.Models.Queries;

public record DataQuery
{
    public DataFilters? Filters { get; set; }
    public DataSorters? Sorters { get; set; }
    public DataPager? Pager { get; set; }
}