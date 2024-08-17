using FlexFetcher.ExpressionBuilders;
using FlexFetcher.Utils;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace FlexFetcher.Models.FlexFetcherOptions;

public class FlexFilterOptions<TEntity, TModel> : FlexFilterOptions<TEntity> where TEntity : class where TModel : class
{
    public new FieldBuilder<TEntity, TField, TModel> Field<TField>(
        Expression<Func<TEntity, TField>> fieldExpression)
    {
        var builder = new FieldBuilder<TEntity, TField, TModel>(fieldExpression);
        FieldBuilders.Add(builder);
        return builder;
    }
}

public class FlexFilterOptions<TEntity> : BaseFlexOptions<TEntity, FilterExpressionBuilder<TEntity>> where TEntity : class
{
    public IImmutableList<BaseFlexFilter> NestedFlexFilters { get; private set; }

    public FlexFilterOptions() : this(new FilterExpressionBuilder<TEntity>())
    {
    }

    public FlexFilterOptions(FilterExpressionBuilder<TEntity> expressionBuilder) : base(expressionBuilder)
    {
        NestedFlexFilters = ImmutableList<BaseFlexFilter>.Empty;
    }

    public void AddNestedFlexFilter(BaseFlexFilter flexFilter)
    {
        var nestedFlexSorters = NestedFlexFilters.Add(flexFilter);
        NestedFlexFilters = nestedFlexSorters;
    }
}