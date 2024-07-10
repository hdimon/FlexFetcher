using System.Collections.Immutable;

namespace FlexFetcher.Models.ExpressionBuilderOptions;

public class FilterExpressionBuilderOptions<TEntity> : BaseFilterExpressionBuilderOptions where TEntity : class
{
    public IImmutableList<IFlexCustomFilter<TEntity>> CustomFilters { get; private set; }

    public FilterExpressionBuilderOptions(Func<string, string>? mapField, IList<IFlexCustomFilter<TEntity>>? customFilters)
    {
        MapField = mapField;

        CustomFilters = customFilters != null
            ? customFilters.ToImmutableList()
            : ImmutableList<IFlexCustomFilter<TEntity>>.Empty;
    }
}