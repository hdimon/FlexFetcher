﻿using FlexFetcher.Utils;
using System.Linq.Expressions;

namespace FlexFetcher;

public abstract class BaseFlexCustomField<TEntity, T> : IFlexCustomField<TEntity> where TEntity : class
{
    public abstract string Field { get; }

    // ReSharper disable once UnusedMember.Global
    public virtual Expression BuildExpression(Expression parameter, IFlexFetcherContext? context = null)
    {
        Expression<Func<TEntity, T>> expressionLambda = BuildFieldExpression(context);
        var expression = expressionLambda.Body.ReplaceParameter(expressionLambda.Parameters[0], parameter);
        return expression;
    }

    protected abstract Expression<Func<TEntity, T>> BuildFieldExpression(IFlexFetcherContext? context = null);
}