using FlexFetcher.Models.FlexFetcherOptions;
using FlexFetcher.Models.Queries;
using FlexFetcher.Utils;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace FlexFetcher.ExpressionBuilders;

public class SorterExpressionBuilder<TEntity> where TEntity : class
{
    public IQueryable<TEntity> BuildExpression(IQueryable<TEntity> query, DataSorters sorters, FlexSorterOptions<TEntity> options)
    {
        var sortersList = sorters.Sorters!;

        var expression = query.Expression;
        var parameter = Expression.Parameter(typeof(TEntity), "x");

        for (int i = 0; i < sortersList.Count; i++)
        {
            var sorter = sortersList[i];
            var property = BuildPropertyExpression(parameter, sorter, options);
            var lambda = Expression.Lambda(property, parameter);
            var methodName = sorter.Direction == DataSorterDirection.Asc ? "OrderBy" : "OrderByDescending";
            if (i > 0)
            {
                methodName = sorter.Direction == DataSorterDirection.Asc ? "ThenBy" : "ThenByDescending";
            }

            expression = Expression.Call(typeof(Queryable), methodName, new[] { typeof(TEntity), property.Type }, expression,
                Expression.Quote(lambda));
        }

        return query.Provider.CreateQuery<TEntity>(expression);
    }

    public Expression BuildPropertyExpression(Expression parameter, DataSorter sorter, FlexSorterOptions<TEntity> options)
    {
        IImmutableList<BaseFlexSorter> nestedFlexSorters = options.NestedFlexSorters;
        Expression property = parameter;
        string field = sorter.Field!;

        var split = field.Split('.');
        var mappedPropertyName = split[0];

        if (options.TryGetPropertyNameByAlias(mappedPropertyName, out var foundPropertyName))
            mappedPropertyName = foundPropertyName;

        if (split.Length == 1)
        {
            if (TryBuildExpressionFromCustomEntityField(options, property, mappedPropertyName, out var expressionResult))
            {
                return expressionResult;
            }

            return Expression.Property(parameter, mappedPropertyName);
        }

        property = Expression.Property(property, mappedPropertyName);

        var nestedSorter = nestedFlexSorters.FirstOrDefault(x => x.EntityType == property.Type) ??
                           (BaseFlexSorter)Activator.CreateInstance(typeof(FlexSorter<>).MakeGenericType(property.Type),
                               new object[] { })!;

        var reducedSorter = sorter with { Field = string.Join(".", split.Skip(1)) };

        var exp = nestedSorter.BuildExpression(property, reducedSorter);

        return exp;
    }

    private static bool TryBuildExpressionFromCustomEntityField(FlexSorterOptions<TEntity> options, Expression property,
        string propertyName, out Expression expressionResult)
    {
        foreach (var currentOptionsCustomField in options.CustomFields)
        {
            if (currentOptionsCustomField.Field != propertyName) continue;

            Type customFieldBuilderType = currentOptionsCustomField.GetType();

            if (TypeHelper.IsInstanceOfGenericType(currentOptionsCustomField, typeof(BaseFlexCustomField<,>)))
            {
                var method = customFieldBuilderType.GetMethod("BuildExpression");

                var exp = (Expression)method!.Invoke(currentOptionsCustomField, new object[] { property })!;
                expressionResult = exp;
                return true;
            }

            throw new NotSupportedException($"Custom field of type {customFieldBuilderType} is not supported. " +
                                            "Custom fields must inherit from BaseFlexCustomField.");
        }

        expressionResult = property;
        return false;
    }
}