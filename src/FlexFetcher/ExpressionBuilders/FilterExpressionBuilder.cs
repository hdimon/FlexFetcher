﻿using FlexFetcher.Models.Queries;
using System.Linq.Expressions;
using System.Collections.Immutable;
using FlexFetcher.ExpressionBuilders.FilterExpressionHandlers;
using FlexFetcher.Utils;
using FlexFetcher.Models.FlexFetcherOptions;
using FlexFetcher.Exceptions;

namespace FlexFetcher.ExpressionBuilders;

public class FilterExpressionBuilder<TEntity> : IExpressionBuilder<TEntity> where TEntity : class
{
    protected ImmutableList<IFilterExpressionHandler> FilterExpressionHandlers { get; }

    public FilterExpressionBuilder()
    {
        var handlers = new List<IFilterExpressionHandler>();
        AddExpressionHandlers(handlers);
        FilterExpressionHandlers = handlers.ToImmutableList();
    }

    public Expression<Func<TEntity, bool>> BuildExpression(DataFilter filter, FlexFilterOptions<TEntity> options, IFlexFetcherContext? context)
    {
        var parameter = Expression.Parameter(typeof(TEntity));
        var body = BuildExpressionBody(parameter, filter, options, context);
        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }

    private Expression BuildExpressionBody(Expression parameter, DataFilter filter, FlexFilterOptions<TEntity> options, IFlexFetcherContext? context)
    {
        if (filter.Filters == null || filter.Filters.Count == 0)
        {
            return Expression.Constant(true);
        }

        var expressions = new List<Expression>();

        foreach (var dataFilter in filter.Filters)
        {
            var expression = BuildSingleExpression(parameter, dataFilter, options, context);
            expressions.Add(expression);
        }

        Expression body = expressions[0];

        for (int i = 1; i < expressions.Count; i++)
        {
            body = filter.Logic?.ToUpper() == DataFilterLogic.Or.ToUpper()
                ? Expression.OrElse(body, expressions[i])
                : Expression.AndAlso(body, expressions[i]);
        }

        return body;
    }

    public Expression BuildSingleExpression(Expression parameter, DataFilter filter, FlexFilterOptions<TEntity> options,
        IFlexFetcherContext? context)
    {
        if (filter.Filters is { Count: > 0 })
        {
            return BuildExpressionBody(parameter, filter, options, context);
        }

        FilterExpressionResult expressionResult = BuildPropertyExpression(parameter, filter, options, context);

        if (expressionResult.IsFull)
        {
            return expressionResult.FilterExpression;
        }

        Expression property = expressionResult.FilterExpression;

        var handler = FilterExpressionHandlers.FirstOrDefault(h => h.Operator.ToUpper() == filter.Operator?.ToUpper());

        if (handler != null)
        {
            return handler.BuildExpression(property, filter);
        }

        throw new NotSupportedException($"Operator {filter.Operator} is not supported.");
    }

    protected void AddExpressionHandlers(List<IFilterExpressionHandler> handlers)
    {
        handlers.Add(new EqualFilterExpressionHandler());
        handlers.Add(new NotEqualFilterExpressionHandler());
        handlers.Add(new GreaterThanFilterExpressionHandler());
        handlers.Add(new GreaterThanOrEqualFilterExpressionHandler());
        handlers.Add(new LessThanFilterExpressionHandler());
        handlers.Add(new LessThanOrEqualFilterExpressionHandler());
        handlers.Add(new ContainsFilterExpressionHandler());
        handlers.Add(new StartsWithFilterExpressionHandler());
        handlers.Add(new EndsWithFilterExpressionHandler());
        handlers.Add(new InFilterExpressionHandler());

        AddCustomExpressionHandlers(handlers);
    }

    protected virtual void AddCustomExpressionHandlers(List<IFilterExpressionHandler> handlers)
    {
    }

    private static FilterExpressionResult BuildPropertyExpression(Expression parameter, DataFilter filter,
        FlexFilterOptions<TEntity> options, IFlexFetcherContext? context)
    {
        Expression property = parameter;
        string field = filter.Field!;

        var split = field.Split('.');
        var mappedFieldName = split[0];

        if (options.IsHiddenField(mappedFieldName))
            throw new FieldNotFoundException(mappedFieldName);

        if (options.TryGetFieldNameByAlias(mappedFieldName, out var foundFieldName))
            mappedFieldName = foundFieldName;

        if (split.Length == 1)
        {
            if (TryBuildExpressionFromCustomEntityFilter(options, property, mappedFieldName, filter.Operator!,
                    filter.Value, context, out var expressionResult))
            {
                return expressionResult;
            }

            property = CreatePropertyExpression(property, mappedFieldName, options);
            return new FilterExpressionResult(property, false);
        }

        property = CreatePropertyExpression(property, mappedFieldName, options);

        var nestedFilter = options.NestedFlexFilters.FirstOrDefault(f => f.EntityType == property.Type) ??
                           (BaseFlexFilter)Activator.CreateInstance(typeof(FlexFilter<>).MakeGenericType(property.Type),
                               new object[] { })!;

        var reducedFilter = filter with { Field = string.Join(".", split.Skip(1)) };
        var exp = nestedFilter.BuildExpression(property, reducedFilter, context);

        return new FilterExpressionResult(exp, true);
    }

    private static bool TryBuildExpressionFromCustomEntityFilter(FlexFilterOptions<TEntity> options, Expression property,
        string fieldName, string filterOperator, object? filterValue, IFlexFetcherContext? context,
        out FilterExpressionResult expressionResult)
    {
        foreach (var currentOptionsCustomFilter in options.CustomFields)
        {
            if (currentOptionsCustomFilter.Field != fieldName) continue;

            if (currentOptionsCustomFilter is BaseFlexCustomFieldFilter<TEntity> baseFlexCustomFilter)
            {
                var exp = baseFlexCustomFilter.BuildExpression(property, filterOperator, filterValue);
                expressionResult = new FilterExpressionResult(exp, true);
                return true;
            }

            Type filterType = currentOptionsCustomFilter.GetType();

            if (TypeHelper.IsInstanceOfGenericType(currentOptionsCustomFilter, typeof(BaseFlexCustomField<,>)))
            {
                var method = filterType.GetMethod("BuildExpression");

                var exp = (Expression)method!.Invoke(currentOptionsCustomFilter, [property, context])!;
                expressionResult = new FilterExpressionResult(exp, false);
                return true;
            }

            throw new NotSupportedException($"Custom filter of type {filterType} is not supported. " +
                                            "Custom filters must inherit from BaseFlexCustomField or BaseFlexCustomFieldFilter.");
        }

        expressionResult = new FilterExpressionResult(property, false);
        return false;
    }

    private static Expression CreatePropertyExpression(Expression parameter, string fieldName, FlexFilterOptions<TEntity> options)
    {
        try
        {
            var property = Expression.Property(parameter, fieldName);

            var castToType = options.GetFieldCastToType(fieldName);
            if (castToType != null)
                return Expression.Convert(property, castToType);

            return property;
        }
        catch (ArgumentException)
        {
            throw new FieldNotFoundException(fieldName);
        }
    }
}