using System.Linq.Expressions;
using FlexFetcher.Models.Queries;

namespace FlexFetcher.ExpressionBuilders.FilterExpressionHandlers;

public class ContainsFilterExpressionHandler : FilterExpressionHandlerAbstract
{
    public override string Operator => DataFilterOperator.Contains;

    public override Expression BuildExpression(Expression property, DataFilter filter)
    {
        var value = BuildValueExpression(filter);
        return Expression.Call(property, "Contains", null, value);
    }
}