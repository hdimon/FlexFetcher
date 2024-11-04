using System.Linq.Expressions;
using FlexFetcher.ExpressionBuilders;
using FlexFetcher.Models.FlexFetcherOptions;
using FlexFetcher.Models.Queries;

namespace FlexFetcher;

public class FlexFilter<TEntity, TModel> : FlexFilter<TEntity> where TEntity : class where TModel : class
{
    public FlexFilter() : this(new FlexFilterOptions<TEntity, TModel>())
    {
    }

    public FlexFilter(FlexFilterOptions<TEntity, TModel> options) : base(options)
    {
    }
}

public class FlexFilter<TEntity>: BaseFlexFilter where TEntity : class
{
    protected FilterExpressionBuilder<TEntity> ExpressionBuilder {get;}

    public FlexFilterOptions<TEntity> Options { get; }

    public override Type EntityType => typeof(TEntity);

    public FlexFilter() : this(new FlexFilterOptions<TEntity>())
    {
    }

    public FlexFilter(FlexFilterOptions<TEntity> options)
    {
        Options = options;
        ExpressionBuilder = options.ExpressionBuilder;
    }

    public IQueryable<TEntity> FilterData(IQueryable<TEntity> query, DataFilter? filter, IFlexFetcherContext? context = null)
    {
        if (FilterIsEmpty(filter))
            return query;

        BuildOptions();

        var expression = BuildExpression(filter!, context);

        query = query.Where(expression);

        return query;
    }

    public IEnumerable<TEntity> FilterData(IEnumerable<TEntity> query, DataFilter? filter, IFlexFetcherContext? context = null)
    {
        if (FilterIsEmpty(filter))
            return query;

        BuildOptions();

        var expression = BuildExpression(filter!, context);

        query = query.Where(expression.Compile());

        return query;
    }

    public override Expression BuildExpression(Expression property, DataFilter filter, IFlexFetcherContext? context = null)
    {
        BuildOptions();

        var expression = ExpressionBuilder.BuildSingleExpression(property, filter, Options, context);
        return expression;
    }

    private Expression<Func<TEntity, bool>> BuildExpression(DataFilter filter, IFlexFetcherContext? context)
    {
        var expression = ExpressionBuilder.BuildExpression(filter, Options, context);
        return expression;
    }

    public bool FilterIsEmpty(DataFilter? filter)
    {
        if (filter?.Filters == null)
            return true;

        return filter.Filters.Count == 0;
    }

    private void BuildOptions()
    {
        if (!Options.IsBuilt)
            Options.Build();
    }
}