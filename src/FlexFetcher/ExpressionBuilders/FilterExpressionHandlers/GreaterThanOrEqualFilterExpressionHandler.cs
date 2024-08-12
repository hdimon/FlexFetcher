﻿using System.Linq.Expressions;
using FlexFetcher.Models.Queries;

namespace FlexFetcher.ExpressionBuilders.FilterExpressionHandlers;

public class GreaterThanOrEqualFilterExpressionHandler : FilterExpressionHandlerAbstract
{
    public override string Operator => DataFilterOperator.GreaterThanOrEqual;

    public override Expression BuildExpression(Expression property, DataFilter filter)
    {
        var value = BuildValueExpression(filter);
        return Expression.GreaterThanOrEqual(property, value);
    }
}