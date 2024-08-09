using System.Collections.Immutable;

namespace FlexFetcher.Models.ExpressionBuilderOptions;

public class FilterExpressionBuilderOptions<TEntity> : BaseExpressionBuilderOptions where TEntity : class
{
    public IImmutableList<IFlexCustomField<TEntity>> CustomFields { get; private set; }

    public FilterExpressionBuilderOptions(Func<string, string>? mapField, IList<IFlexCustomField<TEntity>>? customFields)
    {
        MapField = mapField;

        CustomFields = customFields != null
            ? customFields.ToImmutableList()
            : ImmutableList<IFlexCustomField<TEntity>>.Empty;
    }
}