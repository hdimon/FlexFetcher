using System.Linq.Expressions;
using FlexFetcher.ExpressionBuilders;
using FlexFetcher.Models.FlexFetcherOptions;
using FlexFetcher.Models.Queries;

namespace FlexFetcher;

public class FlexSorter<TEntity>: BaseFlexSorter where TEntity : class
{
    protected SorterExpressionBuilder<TEntity> ExpressionBuilder { get; }

    public FlexSorterOptions<TEntity> Options { get; }

    public override Type EntityType => typeof(TEntity);

    public FlexSorter() : this(new FlexSorterOptions<TEntity>())
    {
    }

    public FlexSorter(FlexSorterOptions<TEntity> options)
    {
        Options = options;
        ExpressionBuilder = options.ExpressionBuilder;
    }

    public IQueryable<TEntity> SortData(IQueryable<TEntity> query, DataSorters? sorters)
    {
        if (SorterIsEmpty(sorters))
            return query;

        BuildProperties();

        query = ExpressionBuilder.BuildExpression(query, sorters!, Options);

        return query;
    }

    public IEnumerable<TEntity> SortData(IEnumerable<TEntity> query, DataSorters? sorters)
    {
        if (SorterIsEmpty(sorters))
            return query;

        BuildProperties();

        query = ExpressionBuilder.BuildExpression(query.AsQueryable(), sorters!, Options);

        return query;
    }

    public override Expression BuildExpression(Expression property, DataSorter sorter)
    {
        BuildProperties();

        return ExpressionBuilder.BuildPropertyExpression(property, sorter, Options);
    }

    private static bool SorterIsEmpty(DataSorters? sorters)
    {
        if (sorters?.Sorters == null)
            return true;

        return sorters.Sorters.Count == 0;
    }

    private void BuildProperties()
    {
        if (!Options.ArePropertiesBuilt)
            Options.BuildProperties();
    }
}