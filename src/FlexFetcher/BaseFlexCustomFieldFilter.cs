using System.Linq.Expressions;
using FlexFetcher.Utils;

namespace FlexFetcher;

public abstract class BaseFlexCustomFieldFilter<TEntity> : IFlexCustomField<TEntity> where TEntity : class
{
    public abstract string Field { get; }

    public virtual Expression BuildExpression(Expression parameter, string filterOperator, object? filterValue,
        IFlexFetcherContext? context = null)
    {
        Expression<Func<TEntity, bool>> expressionLambda = BuildFilterExpression(filterOperator, filterValue, context);
        var expression = expressionLambda.Body.ReplaceParameter(expressionLambda.Parameters[0], parameter);
        return expression;
    }

    protected abstract Expression<Func<TEntity, bool>> BuildFilterExpression(string filterOperator, object? filterValue,
        IFlexFetcherContext? context = null);
}