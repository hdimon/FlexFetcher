using FlexFetcher.Models.Queries;
using System.Linq.Expressions;

namespace FlexFetcher.ExpressionBuilders.FilterExpressionHandlers;

public class EqualFilterExpressionHandler : FilterExpressionHandlerAbstract
{
    public override string Operator => DataFilterOperator.Equal;

    public override Expression BuildExpression(Expression property, DataFilter filter)
    {
        var value = BuildValueExpression(filter);
        return Expression.Equal(property, value);
    }
}