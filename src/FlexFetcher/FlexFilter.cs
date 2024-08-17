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

    public IQueryable<TEntity> FilterData(IQueryable<TEntity> query, DataFilters? filters)
    {
        if (FilterIsEmpty(filters))
            return query;

        BuildOptions();

        var expression = BuildExpression(filters!);

        query = query.Where(expression);

        return query;
    }

    public IEnumerable<TEntity> FilterData(IEnumerable<TEntity> query, DataFilters? filters)
    {
        if (FilterIsEmpty(filters))
            return query;

        BuildOptions();

        var expression = BuildExpression(filters!);

        query = query.Where(expression.Compile());

        return query;
    }

    public override Expression BuildExpression(Expression property, DataFilter filter)
    {
        BuildOptions();

        var expression = ExpressionBuilder.BuildSingleExpression(property, filter, Options);
        return expression;
    }

    private Expression<Func<TEntity, bool>> BuildExpression(DataFilters filters)
    {
        var expression = ExpressionBuilder.BuildExpression(filters, Options);
        return expression;
    }

    public bool FilterIsEmpty(DataFilters? filters)
    {
        if (filters?.Filters == null)
            return true;

        return filters.Filters.Count == 0;
    }

    private void BuildOptions()
    {
        if (!Options.IsBuilt)
            Options.Build();
    }
}