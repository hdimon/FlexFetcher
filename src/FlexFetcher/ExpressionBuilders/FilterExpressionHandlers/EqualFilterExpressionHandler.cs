using FlexFetcher.Models.Queries;
using System.Linq.Expressions;

namespace FlexFetcher.ExpressionBuilders.FilterExpressionHandlers;

public class EqualFilterExpressionHandler : BaseFilterExpressionHandler
{
    public override string Operator => DataFilterOperator.Equal;

    public override Expression BuildExpression(Expression property, DataFilter filter)
    {
        var value = BuildValueExpression(property, filter);
        var propertyExpression = GetPropertyExpression(property, value);

        return Expression.Equal(propertyExpression, value);
    }
}