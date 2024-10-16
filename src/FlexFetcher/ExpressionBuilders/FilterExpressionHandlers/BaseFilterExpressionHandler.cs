using FlexFetcher.Models.Queries;
using System.Linq.Expressions;

namespace FlexFetcher.ExpressionBuilders.FilterExpressionHandlers;

public abstract class BaseFilterExpressionHandler : IFilterExpressionHandler
{
    public abstract string Operator { get; }

    public abstract Expression BuildExpression(Expression property, DataFilter filter);

    public virtual ConstantExpression BuildValueExpression(DataFilter filter)
    {
        return Expression.Constant(filter.Value);
    }

    /// <summary>
    /// This method considers situation when property is nullable
    /// </summary>
    /// <param name="property"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public virtual ConstantExpression BuildValueExpression(Expression property, DataFilter filter)
    {
        var propertyType = property.Type;

        // If property is nullable, get the underlying type
        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            propertyType = propertyType.GetGenericArguments()[0];
        }

        return BuildValueExpression(filter, propertyType);
    }

    public virtual ConstantExpression BuildValueExpression(DataFilter filter, Type valueType)
    {
        var value = filter.Value;
        value = ConvertValueIfNeeded(value, valueType);

        return Expression.Constant(value);
    }

    /// <summary>
    /// This method considers situation when property is nullable
    /// </summary>
    /// <param name="property"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual Expression GetPropertyExpression(Expression property, ConstantExpression value)
    {
        var propertyValue = property;

        // Consider situation when property is nullable
        if (property.Type.IsGenericType && property.Type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
            value is { Value: not null })
        {
            propertyValue = Expression.Property(property, "Value");
        }

        return propertyValue;
    }

    protected object? ConvertValueIfNeeded(object? value, Type valueType)
    {
        if (value is null || value.GetType() == valueType) 
            return value;

#if NET6_0_OR_GREATER
        if (value is string s && valueType == typeof(DateOnly))
        {
            value = DateOnly.Parse(s);
        }
        else if (value is string value1 && valueType == typeof(TimeOnly))
        {
            value = TimeOnly.Parse(value1);
        }
        else
#endif
        if (value is string s1 && valueType == typeof(DateTimeOffset))
        {
            value = DateTimeOffset.Parse(s1);
        }
        else if (value is string value2 && valueType == typeof(TimeSpan))
        {
            value = TimeSpan.Parse(value2);
        }
        else if (value is string s2 && valueType == typeof(Guid))
        {
            value = Guid.Parse(s2);
        }
        // If value type is string and property type is enum, convert string to enum
        else if (value is string value3 && valueType.IsEnum)
        {
            value = Enum.Parse(valueType, value3);
        }
        // If value type is int32 and property type is enum, convert int32 to enum
        else if (value is int i && valueType.IsEnum)
        {
            value = Enum.ToObject(valueType, i);
        }
        // If value type is int64 and property type is enum, convert int64 to enum
        else if (value is long l && valueType.IsEnum)
        {
            value = Enum.ToObject(valueType, l);
        }
        // If value type is decimal and property type is enum, convert decimal to enum
        else if (value is decimal d && valueType.IsEnum)
        {
            value = Enum.ToObject(valueType, (int)d);
        }
        // If value type is DateTimeOffset and property type is DateTime, convert DateTimeOffset to DateTime
        else if (value is DateTimeOffset dto && valueType == typeof(DateTime))
        {
            value = dto.DateTime;
        }
        else
            value = Convert.ChangeType(value, valueType);

        return value;
    }
}