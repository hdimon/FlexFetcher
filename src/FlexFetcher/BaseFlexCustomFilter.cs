using System.Linq.Expressions;
using FlexFetcher.Utils;

namespace FlexFetcher;

public abstract class BaseFlexCustomFilter<TEntity, T> : IFlexCustomFilter<TEntity> where TEntity : class
{
    public abstract string Field { get; }

    public virtual Expression BuildExpression(Expression parameter)
    {
        Expression<Func<TEntity, T>> expressionLambda = BuildFilterExpression();
        var expression = expressionLambda.Body.ReplaceParameter(expressionLambda.Parameters[0], parameter);
        return expression;
    }

    protected abstract Expression<Func<TEntity, T>> BuildFilterExpression();
}

public abstract class BaseFlexCustomFilter<TEntity> : IFlexCustomFilter<TEntity> where TEntity : class
{
    public abstract string Field { get; }

    public virtual Expression BuildExpression(Expression parameter, string filterOperator, object? filterValue)
    {
        Expression<Func<TEntity, bool>> expressionLambda = BuildFilterExpression(filterOperator, filterValue);
        var expression = expressionLambda.Body.ReplaceParameter(expressionLambda.Parameters[0], parameter);
        return expression;
    }

    protected abstract Expression<Func<TEntity, bool>> BuildFilterExpression(string filterOperator, object? filterValue);
}