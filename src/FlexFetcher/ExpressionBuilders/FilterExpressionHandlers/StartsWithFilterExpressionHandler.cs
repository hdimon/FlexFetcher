using System.Linq.Expressions;
using FlexFetcher.Models.Queries;

namespace FlexFetcher.ExpressionBuilders.FilterExpressionHandlers;

public class StartsWithFilterExpressionHandler : FilterExpressionHandlerAbstract
{
    public override string Operator => DataFilterOperator.StartsWith;

    public override Expression BuildExpression(Expression property, DataFilter filter)
    {
        var value = BuildValueExpression(filter);
        return Expression.Call(property, "StartsWith", null, value);
    }
}