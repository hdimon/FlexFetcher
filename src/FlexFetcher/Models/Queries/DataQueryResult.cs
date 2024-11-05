namespace FlexFetcher.Models.Queries;

public class DataQueryResult<TEntity> where TEntity : class
{
    public IList<TEntity> Items { get; set; }
    public int TotalCount { get; set; }

    public DataQueryResult(IList<TEntity> results, int totalCount)
    {
        Items = results;
        TotalCount = totalCount;
    }
}