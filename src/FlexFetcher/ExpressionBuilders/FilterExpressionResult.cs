using System.Linq.Expressions;

namespace FlexFetcher.ExpressionBuilders;

public class FilterExpressionResult(Expression expression, bool isFull)
{
    public Expression FilterExpression { get; } = expression;
    public bool IsFull { get; } = isFull;
}