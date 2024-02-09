using System.Collections.Immutable;
using System.Linq.Expressions;
using FlexFetcher.ExpressionBuilders;
using FlexFetcher.Models.ExpressionBuilderOptions;
using FlexFetcher.Models.Queries;

namespace FlexFetcher;

public class FlexFilter<TEntity>: BaseFlexFilter where TEntity : class
{
    public IImmutableList<BaseFlexFilter> NestedFlexFilters { get; }

    public FilterExpressionBuilderOptions<TEntity> FilterExpressionBuilderOptions
    {
        get
        {
            var options = new FilterExpressionBuilderOptions<TEntity>(MapField, CustomFilters.ToList(),
                NestedFlexFilters.Select(x => x.BaseFilterExpressionBuilderOptions).ToArray());

            return options;
        }
    }

    public override BaseFilterExpressionBuilderOptions BaseFilterExpressionBuilderOptions => FilterExpressionBuilderOptions;

    public IImmutableList<IFlexCustomFilter<TEntity>> CustomFilters { get; private set; } =
        new List<IFlexCustomFilter<TEntity>>().ToImmutableList();

    public FlexFilter(params BaseFlexFilter[] flexFilters)
    {
        var nestedFlexFilters = new List<BaseFlexFilter>();
        
        //TODO: Add validation that nested filters are filters for nested properties
        foreach (var filter in flexFilters)
        {
            nestedFlexFilters.Add(filter);
        }

        NestedFlexFilters = nestedFlexFilters.ToImmutableList();
    }

    public IQueryable<TEntity> FilterData(IQueryable<TEntity> query, DataFilters? filters)
    {
        if (FilterIsEmpty(filters))
            return query;

        var expression = BuildExpression(filters!);

        query = query.Where(expression);

        return query;
    }

    public IEnumerable<TEntity> FilterData(IEnumerable<TEntity> query, DataFilters? filters)
    {
        if (FilterIsEmpty(filters))
            return query;

        var expression = BuildExpression(filters!);

        query = query.Where(expression.Compile());

        return query;
    }

    protected virtual string MapField(string field)
    {
        return field;
    }

    protected void AddCustomFilter(IFlexCustomFilter<TEntity> customFilter)
    {
        var flexCustomFilters = CustomFilters.Add(customFilter);
        CustomFilters = flexCustomFilters;
    }

    private Expression<Func<TEntity, bool>> BuildExpression(DataFilters filters)
    {
        var builder = new FilterExpressionBuilder<TEntity>();
        var expression = builder.BuildExpression(filters, FilterExpressionBuilderOptions);
        return expression;
    }

    private bool FilterIsEmpty(DataFilters? filters)
    {
        if (filters?.Filters == null)
            return true;

        return filters.Filters.Count == 0;
    }
}