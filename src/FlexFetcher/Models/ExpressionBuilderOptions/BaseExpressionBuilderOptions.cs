namespace FlexFetcher.Models.ExpressionBuilderOptions;

public class BaseExpressionBuilderOptions
{
    public Func<string, string>? MapField { get; protected set; }
}