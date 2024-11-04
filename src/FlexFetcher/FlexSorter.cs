using System.Linq.Expressions;
using FlexFetcher.ExpressionBuilders;
using FlexFetcher.Models.FlexFetcherOptions;
using FlexFetcher.Models.Queries;

namespace FlexFetcher;

public class FlexSorter<TEntity, TModel> : FlexSorter<TEntity> where TEntity : class where TModel : class
{
    public FlexSorter() : this(new FlexSorterOptions<TEntity, TModel>())
    {
    }

    public FlexSorter(FlexSorterOptions<TEntity, TModel> options) : base(options)
    {
    }
}

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

    public IQueryable<TEntity> SortData(IQueryable<TEntity> query, DataSorters? sorters, IFlexFetcherContext? context = null)
    {
        if (SorterIsEmpty(sorters))
            return query;

        BuildOptions();

        query = ExpressionBuilder.BuildExpression(query, sorters!, Options, context);

        return query;
    }

    public IEnumerable<TEntity> SortData(IEnumerable<TEntity> query, DataSorters? sorters, IFlexFetcherContext? context = null)
    {
        if (SorterIsEmpty(sorters))
            return query;

        BuildOptions();

        query = ExpressionBuilder.BuildExpression(query.AsQueryable(), sorters!, Options, context);

        return query;
    }

    public override Expression BuildExpression(Expression property, DataSorter sorter, IFlexFetcherContext? context = null)
    {
        BuildOptions();

        return ExpressionBuilder.BuildPropertyExpression(property, sorter, Options, context);
    }

    public bool SorterIsEmpty(DataSorters? sorters)
    {
        if (sorters?.Sorters == null)
            return true;

        return sorters.Sorters.Count == 0;
    }

    private void BuildOptions()
    {
        if (!Options.IsBuilt)
            Options.Build();
    }
}