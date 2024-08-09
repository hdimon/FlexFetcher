using System.Collections.Immutable;
using System.Linq.Expressions;
using FlexFetcher.ExpressionBuilders;
using FlexFetcher.Models.ExpressionBuilderOptions;
using FlexFetcher.Models.Queries;

namespace FlexFetcher;

public class FlexFilter<TEntity>: BaseFlexFilter where TEntity : class
{
    protected FilterExpressionBuilder<TEntity> ExpressionBuilder {get;}
    public IImmutableList<BaseFlexFilter> NestedFlexFilters { get; }

    public FilterExpressionBuilderOptions<TEntity> FilterExpressionBuilderOptions
    {
        get
        {
            var options = new FilterExpressionBuilderOptions<TEntity>(MapField, CustomFilters.ToList());

            return options;
        }
    }

    public override Type EntityType => typeof(TEntity);

    public IImmutableList<IFlexCustomField<TEntity>> CustomFilters { get; private set; } =
        new List<IFlexCustomField<TEntity>>().ToImmutableList();

    public FlexFilter() : this(new FilterExpressionBuilder<TEntity>(), Array.Empty<BaseFlexFilter>())
    {
    }

    public FlexFilter(FilterExpressionBuilder<TEntity> expressionBuilder) : this(expressionBuilder, Array.Empty<BaseFlexFilter>())
    {
    }

    public FlexFilter(params BaseFlexFilter[] flexFilters) : this(new FilterExpressionBuilder<TEntity>(), flexFilters)
    {
    }

    public FlexFilter(FilterExpressionBuilder<TEntity> expressionBuilder, params BaseFlexFilter[] flexFilters)
    {
        ExpressionBuilder = expressionBuilder;
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

    public override Expression BuildExpression(Expression property, DataFilter filter)
    {
        var expression = ExpressionBuilder.BuildSingleExpression(property, filter, FilterExpressionBuilderOptions, NestedFlexFilters);
        return expression;
    }

    protected virtual string MapField(string field)
    {
        return field;
    }

    protected void AddCustomField(IFlexCustomField<TEntity> customFilter)
    {
        var flexCustomFilters = CustomFilters.Add(customFilter);
        CustomFilters = flexCustomFilters;
    }

    private Expression<Func<TEntity, bool>> BuildExpression(DataFilters filters)
    {
        var expression = ExpressionBuilder.BuildExpression(filters, FilterExpressionBuilderOptions, NestedFlexFilters);
        return expression;
    }

    private bool FilterIsEmpty(DataFilters? filters)
    {
        if (filters?.Filters == null)
            return true;

        return filters.Filters.Count == 0;
    }
}