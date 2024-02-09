using System.Collections.Immutable;

namespace FlexFetcher.Models.ExpressionBuilderOptions;

public class BaseFilterExpressionBuilderOptions
{
    public Func<string, string>? MapField { get; protected set; }

    public IImmutableList<BaseFilterExpressionBuilderOptions> NestedFilterExpressionBuilderOptions { get; protected set; } =
        new ImmutableArray<BaseFilterExpressionBuilderOptions>();
}