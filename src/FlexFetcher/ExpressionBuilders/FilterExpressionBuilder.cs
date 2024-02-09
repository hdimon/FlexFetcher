using System.Collections;
using FlexFetcher.Models.ExpressionBuilderOptions;
using FlexFetcher.Models.Queries;
using System.Linq.Expressions;
using System.Text.Json;
using System.Reflection;

namespace FlexFetcher.ExpressionBuilders;

public class FilterExpressionBuilder<TEntity> where TEntity : class
{
    /*public Expression<Func<TEntity, bool>> BuildExpression(DataFilters filters)
    {
        var parameter = Expression.Parameter(typeof(TEntity));
        var body = BuildExpressionBody(parameter, filters, null);
        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }*/

    public Expression<Func<TEntity, bool>> BuildExpression(DataFilters filters, FilterExpressionBuilderOptions<TEntity>? builderOptions)
    {
        var parameter = Expression.Parameter(typeof(TEntity));
        var body = BuildExpressionBody(parameter, filters, builderOptions);
        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }

    /*public Expression<Func<TEntity, bool>> BuildExpression(DataFilters filters, Func<string, string>? mapField)
    {
        var parameter = Expression.Parameter(typeof(TEntity));
        var body = BuildExpressionBody(parameter, filters, mapField);
        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }*/

    private Expression BuildExpressionBody(ParameterExpression parameter, DataFilters filters,
        FilterExpressionBuilderOptions<TEntity>? builderOptions)
    {
        if (filters.Filters == null || filters.Filters.Count == 0)
        {
            return Expression.Constant(true);
        }

        var expressions = new List<Expression>();

        foreach (var filter in filters.Filters)
        {
            var expression = BuildSingleExpression(parameter, filter, builderOptions);
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

    private Expression BuildSingleExpression(ParameterExpression parameter, DataFilter filter, FilterExpressionBuilderOptions<TEntity>? builderOptions)
    {
        if (filter.Filters is { Count: > 0 })
        {
            return BuildExpressionBody(parameter, filter, builderOptions);
        }

        FilterExpressionResult expressionResult = BuildPropertyExpression(parameter, filter, builderOptions);

        if (expressionResult.IsFull)
        {
            return expressionResult.FilterExpression;
        }

        Expression property = expressionResult.FilterExpression;

        ConstantExpression value;

        if (filter.Operator?.ToUpper() == "IN")
        {
            if (filter.Value == null || string.IsNullOrWhiteSpace(filter.Value.ToString()))
                return Expression.Constant(false);

            var valueString = filter.Value.ToString()!;

            object? array;
            try
            {
                array = JsonSerializer.Deserialize(valueString, typeof(IEnumerable<>).MakeGenericType(property.Type));
            }
            catch (Exception e)
            {
                array = ConvertToArray(valueString, property.Type);
            }

            if (array == null)
            {
                return Expression.Constant(false);
            }

            value = Expression.Constant(array, typeof(IEnumerable<>).MakeGenericType(property.Type));
        }
        else
        {
            value = Expression.Constant(filter.Value, property.Type);
        }

        switch (filter.Operator?.ToUpper())
        {
            case "EQ":
                return Expression.Equal(property, value);
            case "NEQ":
                return Expression.NotEqual(property, value);
            case "GT":
                return Expression.GreaterThan(property, value);
            case "GTE":
                return Expression.GreaterThanOrEqual(property, value);
            case "LT":
                return Expression.LessThan(property, value);
            case "LTE":
                return Expression.LessThanOrEqual(property, value);
            case "CONTAINS":
                return Expression.Call(property, "Contains", null, value);
            case "STARTSWITH":
                return Expression.Call(property, "StartsWith", null, value);
            case "ENDSWITH":
                return Expression.Call(property, "EndsWith", null, value);
            case "IN":
                return Expression.Call(typeof(Enumerable), "Contains", new[] { property.Type }, value, property);

            default:
                throw new NotSupportedException($"Operator {filter.Operator} is not supported.");
        }
    }

    private static FilterExpressionResult BuildPropertyExpression(ParameterExpression parameter, DataFilter filter,
        FilterExpressionBuilderOptions<TEntity>? builderOptions)
    {
        Expression property = parameter;
        string field = filter.Field;

        var fieldIsMapped = false;
        string mappedField = field;

        //First try to map the field from main filter options
        if (builderOptions?.MapField != null)
        {
            mappedField = builderOptions.MapField(field);
            fieldIsMapped = mappedField != field;
        }

        BaseFilterExpressionBuilderOptions? currentOptions = builderOptions;

        var split = mappedField.Split('.');
        for (var index = 0; index < split.Length; index++)
        {
            var propertyName = split[index];
            var mappedPropertyName = propertyName;

            if (!fieldIsMapped)
                mappedPropertyName = currentOptions?.MapField?.Invoke(propertyName) ?? propertyName;

            var isCustomFilter = false;
            if (index == split.Length - 1)
            {
                if (TryBuildExpressionFromCustomEntityFilter(currentOptions, property, mappedPropertyName, filter.Operator,
                        filter.Value, out var expressionResult))
                {
                    isCustomFilter = true;
                    return expressionResult;
                }
            }

            if (!isCustomFilter)
                property = Expression.Property(property, mappedPropertyName);

            //if (!fieldIsMapped)
            {
                currentOptions =
                    currentOptions?.NestedFilterExpressionBuilderOptions.FirstOrDefault(f =>
                        f.GetType().GenericTypeArguments.Any(t => t == property.Type));
            }
        }

        return new FilterExpressionResult(property, false);
        //return property;
    }

    private static bool TryBuildExpressionFromCustomEntityFilter(BaseFilterExpressionBuilderOptions? currentOptions,
        Expression property, string propertyName, string filterOperator, object filterValue, out FilterExpressionResult expressionResult)
    {
        var propertyInfo = currentOptions?.GetType().GetProperty("CustomFilters");
        if (propertyInfo != null)
        {
            var customFilters = (propertyInfo.GetValue(currentOptions) as IEnumerable)!;
            foreach (var filter in customFilters)
            {
                var fieldProperty = filter.GetType().GetProperty("Field");
                if (fieldProperty == null) 
                    continue;

                var fieldValue = (string)fieldProperty.GetValue(filter)!;
                if (fieldValue != propertyName) 
                    continue;

                // Assuming 'filter' is the instance of IFlexCustomFilter<TEntity>
                var methodInfo = filter.GetType().GetMethod("BuildExpression");

                if (methodInfo != null)
                {
                    object[]? methodParams = new object[] { property };
                    bool isFull = false;

                    if (methodInfo.GetParameters().Length == 1)
                    {
                        methodParams = new object[] { property };
                    }
                    else if (methodInfo.GetParameters().Length == 3)
                    {
                        methodParams = new object[] { property, filterOperator, filterValue };
                        isFull = true;
                    }

                    Expression exp = methodInfo.Invoke(filter, methodParams) as Expression;
                    expressionResult = new FilterExpressionResult(exp, isFull);
                    return true;
                }

                break;
            }
        }

        //exp = property;
        expressionResult = new FilterExpressionResult(property, false);
        return false;
    }

    static Array ConvertToArray(string valueString, Type elementType)
    {
        // Split the string into parts
        string[] parts = valueString.Split(',');

        // Convert each part to the specified type
        var convertedArray = parts.Select(part =>
        {
            // Use reflection to call the static Parse method on the specified type
            MethodInfo parseMethod = elementType.GetMethod("Parse", new[] { typeof(string) });
            if (parseMethod != null)
            {
                return parseMethod.Invoke(null, new object[] { part });
            }
            else
            {
                throw new InvalidOperationException($"Type {elementType} does not have a static Parse method.");
            }
        }).ToArray();

        // Create an array of the specified type and copy the converted values
        Array resultArray = Array.CreateInstance(elementType, parts.Length);
        Array.Copy(convertedArray, resultArray, parts.Length);

        return resultArray;
    }
}