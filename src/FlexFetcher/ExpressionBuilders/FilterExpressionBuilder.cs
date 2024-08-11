using FlexFetcher.Models.ExpressionBuilderOptions;
using FlexFetcher.Models.Queries;
using System.Linq.Expressions;
using System.Collections.Immutable;
using FlexFetcher.ExpressionBuilders.FilterExpressionHandlers;
using FlexFetcher.Utils;

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

        // First try to map the field from main filter options.
        // At this point we are not sure if the field is complex (with nested fields) or not
        // but just try, i.e. if mapping contains "Field" or "Field.NestedField"
        // then it will be mapped.
        if (builderOptions.MapField != null)
        {
            mappedField = builderOptions.MapField(field);
            fieldIsMapped = mappedField != field;
        }

        var split = mappedField.Split('.');
        var currentFieldName = split[0];

        var mappedFieldName = currentFieldName;

        // At this point only simple fields are tried to be mapped.
        if (!fieldIsMapped)
            mappedFieldName = builderOptions.MapField?.Invoke(currentFieldName) ?? currentFieldName;

        if (split.Length == 1)
        {
            if (TryBuildExpressionFromCustomEntityFilter(builderOptions, property, mappedFieldName, filter.Operator!,
                    filter.Value, out var expressionResult))
            {
                return expressionResult;
            }

            property = Expression.Property(property, mappedFieldName);
            return new FilterExpressionResult(property, false);
        }

        property = Expression.Property(property, mappedFieldName);

        var nestedFilter = nestedFlexFilters.FirstOrDefault(f => f.EntityType == property.Type) ??
                           (BaseFlexFilter)Activator.CreateInstance(typeof(FlexFilter<>).MakeGenericType(property.Type),
                               new object[] { })!;

        var reducedFilter = filter with { Field = string.Join(".", split.Skip(1)) };
        var exp = nestedFilter.BuildExpression(property, reducedFilter);

        return new FilterExpressionResult(exp, true);
    }

    private static bool TryBuildExpressionFromCustomEntityFilter(FilterExpressionBuilderOptions<TEntity> options,
        Expression property, string fieldName, string filterOperator, object? filterValue,
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

                var exp = (Expression)method!.Invoke(currentOptionsCustomFilter, new object[] { property })!;
                expressionResult = new FilterExpressionResult(exp, false);
                return true;
            }

            throw new NotSupportedException($"Custom filter of type {filterType} is not supported. " +
                                            "Custom filters must inherit from BaseFlexCustomField or BaseFlexCustomFieldFilter.");
        }

        expressionResult = new FilterExpressionResult(property, false);
        return false;
    }
}