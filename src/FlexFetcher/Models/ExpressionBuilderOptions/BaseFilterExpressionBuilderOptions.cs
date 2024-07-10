namespace FlexFetcher.Models.ExpressionBuilderOptions;

public class BaseFilterExpressionBuilderOptions
{
    public Func<string, string>? MapField { get; protected set; }
}