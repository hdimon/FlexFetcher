using System.Linq.Expressions;
using FlexFetcher.Models.Queries;

namespace FlexFetcher.ExpressionBuilders.FilterExpressionHandlers;

public class GreaterThanOrEqualFilterExpressionHandler : BaseFilterExpressionHandler
{
    public override string Operator => DataFilterOperator.GreaterThanOrEqual;

    public override Expression BuildExpression(Expression property, DataFilter filter)
    {
        var value = BuildValueExpression(property, filter);
        var propertyExpression = GetPropertyExpression(property, value);

        return Expression.GreaterThanOrEqual(propertyExpression, value);
    }
}