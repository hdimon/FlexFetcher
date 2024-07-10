using FlexFetcher.Models.Queries;
using System.Linq.Expressions;

namespace FlexFetcher.ExpressionBuilders.FilterExpressionHandlers;

public abstract class FilterExpressionHandlerAbstract : IFilterExpressionHandler
{
    public abstract string Operator { get; }

    public abstract Expression BuildExpression(Expression property, DataFilter filter);

    public virtual ConstantExpression BuildValueExpression(DataFilter filter)
    {
        return Expression.Constant(filter.Value);
    }
}