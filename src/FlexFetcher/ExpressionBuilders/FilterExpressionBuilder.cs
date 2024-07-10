using FlexFetcher.Models.ExpressionBuilderOptions;
using FlexFetcher.Models.Queries;
using System.Linq.Expressions;
using System.Collections.Immutable;
using FlexFetcher.ExpressionBuilders.FilterExpressionHandlers;

namespace FlexFetcher.ExpressionBuilders;

public class FilterExpressionBuilder<TEntity> where TEntity : class
{
    protected ImmutableList<IFilterExpressionHandler> FilterExpressionHandlers { get; }

    public FilterExpressionBuilder()
    {
        var handlers = new List<IFilterExpressionHandler>();
        AddExpressionHandlers(handlers);
        FilterExpressionHandlers = handlers.ToImmutableList();
    }

    public Expression<Func<TEntity, bool>> BuildExpression(DataFilters filters,
        FilterExpressionBuilderOptions<TEntity> builderOptions, IImmutableList<BaseFlexFilter> nestedFlexFilters)
    {
        var parameter = Expression.Parameter(typeof(TEntity));
        var body = BuildExpressionBody(parameter, filters, builderOptions, nestedFlexFilters);
        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }

    private Expression BuildExpressionBody(Expression parameter, DataFilters filters,
        FilterExpressionBuilderOptions<TEntity> builderOptions, IImmutableList<BaseFlexFilter> nestedFlexFilters)
    {
        if (filters.Filters == null || filters.Filters.Count == 0)
        {
            return Expression.Constant(true);
        }

        var expressions = new List<Expression>();

        foreach (var filter in filters.Filters)
        {
            var expression = BuildSingleExpression(parameter, filter, builderOptions, nestedFlexFilters);
            expressions.Add(expression);
        }

        Expression body = expressions[0];

        for (int i = 1; i < expressions.Count; i++)
        {
            body = filters.Logic?.ToUpper() == "AND"
                ? Expression.AndAlso(body, expressions[i])
                : Expression.OrElse(body, expressions[i]);
        }

        return body;
    }

    public Expression BuildSingleExpression(Expression parameter, DataFilter filter,
        FilterExpressionBuilderOptions<TEntity> builderOptions, IImmutableList<BaseFlexFilter> nestedFlexFilters)
    {
        if (filter.Filters is { Count: > 0 })
        {
            return BuildExpressionBody(parameter, filter, builderOptions, nestedFlexFilters);
        }

        FilterExpressionResult expressionResult = BuildPropertyExpression(parameter, filter, builderOptions, nestedFlexFilters);

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
        FilterExpressionBuilderOptions<TEntity> builderOptions, IImmutableList<BaseFlexFilter> nestedFlexFilters)
    {
        Expression property = parameter;
        string field = filter.Field!;

        var fieldIsMapped = false;
        string mappedField = field;

        //First try to map the field from main filter options
        if (builderOptions.MapField != null)
        {
            mappedField = builderOptions.MapField(field);
            fieldIsMapped = mappedField != field;
        }

        var split = mappedField.Split('.');
        var currentPropertyName = split[0];

        var mappedPropertyName = currentPropertyName;

        if (!fieldIsMapped)
            mappedPropertyName = builderOptions.MapField?.Invoke(currentPropertyName) ?? currentPropertyName;

        if (split.Length == 1)
        {
            if (TryBuildExpressionFromCustomEntityFilter(builderOptions, property, mappedPropertyName, filter.Operator!,
                    filter.Value, out var expressionResult))
            {
                return expressionResult;
            }

            property = Expression.Property(property, mappedPropertyName);
            return new FilterExpressionResult(property, false);
        }

        property = Expression.Property(property, mappedPropertyName);

        var nestedFilter = nestedFlexFilters.FirstOrDefault(f => f.EntityType == property.Type) ??
                           (BaseFlexFilter)Activator.CreateInstance(typeof(FlexFilter<>).MakeGenericType(property.Type),
                               new object[] { })!;

        var reducedFilter = filter with { Field = string.Join(".", split.Skip(1)) };
        var exp = nestedFilter.BuildExpression(property, reducedFilter);

        return new FilterExpressionResult(exp, true);
    }

    private static bool TryBuildExpressionFromCustomEntityFilter(FilterExpressionBuilderOptions<TEntity> options,
        Expression property, string propertyName, string filterOperator, object? filterValue,
        out FilterExpressionResult expressionResult)
    {
        foreach (var currentOptionsCustomFilter in options.CustomFilters)
        {
            if (currentOptionsCustomFilter.Field != propertyName) continue;

            if (currentOptionsCustomFilter is BaseFlexCustomFilter<TEntity> baseFlexCustomFilter)
            {
                var exp = baseFlexCustomFilter.BuildExpression(property, filterOperator, filterValue);
                expressionResult = new FilterExpressionResult(exp, true);
                return true;
            }

            Type filterType = currentOptionsCustomFilter.GetType();

            if (IsInstanceOfGenericType(currentOptionsCustomFilter, typeof(BaseFlexCustomFilter<,>)))
            {
                var method = filterType.GetMethod("BuildExpression");

                var exp = (Expression)method!.Invoke(currentOptionsCustomFilter, new object[] { property })!;
                expressionResult = new FilterExpressionResult(exp, false);
                return true;
            }

            throw new NotSupportedException($"Custom filter of type {filterType} is not supported. " +
                                            "Custom filters must inherit from BaseFlexCustomFilter.");
        }

        expressionResult = new FilterExpressionResult(property, false);
        return false;
    }

    private static bool IsInstanceOfGenericType(object obj, Type genericTypeDefinition)
    {
        var objectType = obj.GetType();
        var baseType = objectType;

        while (baseType != null)
        {
            if (baseType.IsGenericType &&
                baseType.GetGenericTypeDefinition() == genericTypeDefinition)
            {
                var typeArguments = baseType.GetGenericArguments();
                var constructedGenericType = genericTypeDefinition.MakeGenericType(typeArguments);

                if (constructedGenericType.IsAssignableFrom(objectType))
                {
                    return true;
                }
            }

            baseType = baseType.BaseType;
        }

        return false;
    }
}