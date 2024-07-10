using FlexFetcher.Models.Queries;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace FlexFetcher.ExpressionBuilders.FilterExpressionHandlers;

public class InFilterExpressionHandler : FilterExpressionHandlerAbstract
{
    public override string Operator => DataFilterOperator.In;

    public override Expression BuildExpression(Expression property, DataFilter filter)
    {
        if (filter.Value == null || string.IsNullOrWhiteSpace(filter.Value.ToString()))
            return Expression.Constant(false);

        var valueString = filter.Value.ToString()!;

        object? array;
        try
        {
            array = JsonSerializer.Deserialize(valueString, typeof(IEnumerable<>).MakeGenericType(property.Type));
        }
        catch (Exception)
        {
            array = ConvertToArray(valueString, property.Type);
        }

        if (array == null)
        {
            return Expression.Constant(false);
        }

        ConstantExpression value = Expression.Constant(array, typeof(IEnumerable<>).MakeGenericType(property.Type));

        return Expression.Call(typeof(Enumerable), "Contains", new[] { property.Type }, value, property);
    }

    private static Array ConvertToArray(string valueString, Type elementType)
    {
        string[] parts = valueString.Split(',');

        // Convert each part to the specified type
        var convertedArray = parts.Select(part =>
        {
            MethodInfo? parseMethod = elementType.GetMethod("Parse", new[] { typeof(string) });
            if (parseMethod != null)
            {
                return parseMethod.Invoke(null, new object[] { part });
            }

            throw new InvalidOperationException($"Type {elementType} does not have a static Parse method.");
        }).ToArray();

        // Create an array of the specified type and copy the converted values
        Array resultArray = Array.CreateInstance(elementType, parts.Length);
        Array.Copy(convertedArray, resultArray, parts.Length);

        return resultArray;
    }
}