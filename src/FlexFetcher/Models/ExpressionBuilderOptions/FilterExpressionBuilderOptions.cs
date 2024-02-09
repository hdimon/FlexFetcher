using System.Collections.Immutable;

namespace FlexFetcher.Models.ExpressionBuilderOptions;

public class FilterExpressionBuilderOptions<TEntity> : BaseFilterExpressionBuilderOptions where TEntity : class
{
    public IImmutableList<IFlexCustomFilter<TEntity>> CustomFilters { get; private set; }

    public FilterExpressionBuilderOptions(Func<string, string>? mapField, IList<IFlexCustomFilter<TEntity>>? customFilters,
        params BaseFilterExpressionBuilderOptions[] options)
    {
        MapField = mapField;
        var list = new List<BaseFilterExpressionBuilderOptions>();

        //TODO: Add validation that nested filters are filters for nested properties
        foreach (var filter in options)
        {
            list.Add(filter);
        }

        CustomFilters = customFilters != null
            ? customFilters.ToImmutableList()
            : ImmutableList<IFlexCustomFilter<TEntity>>.Empty;

        NestedFilterExpressionBuilderOptions = list.ToImmutableList();
    }
}