using System.Linq.Expressions;
using FlexFetcher.Models.Queries;

namespace FlexFetcher.ExpressionBuilders.FilterExpressionHandlers;

public class GreaterThanFilterExpressionHandler : FilterExpressionHandlerAbstract
{
    public override string Operator => DataFilterOperator.GreaterThan;

    public override Expression BuildExpression(Expression property, DataFilter filter)
    {
        var value = BuildValueExpression(property, filter);
        var propertyExpression = GetPropertyExpression(property, value);

        return Expression.GreaterThan(propertyExpression, value);
    }
}