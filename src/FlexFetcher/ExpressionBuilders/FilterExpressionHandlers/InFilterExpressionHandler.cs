using System.Collections;
using FlexFetcher.Models.Queries;
using System.Linq.Expressions;
using FlexFetcher.Utils;

namespace FlexFetcher.ExpressionBuilders.FilterExpressionHandlers;

public class InFilterExpressionHandler : BaseFilterExpressionHandler
{
    public override string Operator => DataFilterOperator.In;

    public override Expression BuildExpression(Expression property, DataFilter filter)
    {
        if (filter.Value == null)
            return Expression.Constant(false);

        Array arrayCopy = CopyFilterValueList(property, (IList)filter.Value);

        ConstantExpression value = Expression.Constant(arrayCopy, typeof(IEnumerable<>).MakeGenericType(property.Type));

        return Expression.Call(typeof(Enumerable), "Contains", [property.Type], value, property);
    }

    private Array CopyFilterValueList(Expression property, IList filterValueList)
    {
        var propertyType = TypeHelper.GetGenericUnderlyingType(property.Type);
        Array arrayCopy = Array.CreateInstance(property.Type, filterValueList.Count);

        for (int i = 0; i < filterValueList.Count; i++)
        {
            var itemValue = ConvertValueIfNeeded(filterValueList[i], propertyType);
            arrayCopy.SetValue(itemValue, i);
        }

        return arrayCopy;
    }
}