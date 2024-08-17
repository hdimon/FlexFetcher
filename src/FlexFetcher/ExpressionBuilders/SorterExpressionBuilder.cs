﻿using FlexFetcher.Exceptions;
using FlexFetcher.Models.FlexFetcherOptions;
using FlexFetcher.Models.Queries;
using FlexFetcher.Utils;
using System.Linq.Expressions;

namespace FlexFetcher.ExpressionBuilders;

public class SorterExpressionBuilder<TEntity> : IExpressionBuilder<TEntity> where TEntity : class
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
        Expression property = parameter;
        string field = sorter.Field!;

        var split = field.Split('.');
        var mappedFieldName = split[0];

        if (options.IsHiddenField(mappedFieldName))
            throw new FieldNotFoundException(mappedFieldName);

        if (options.TryGetFieldNameByAlias(mappedFieldName, out var foundFieldName))
            mappedFieldName = foundFieldName;

        if (split.Length == 1)
        {
            if (TryBuildExpressionFromCustomEntityField(options, property, mappedFieldName, out var expressionResult))
            {
                return expressionResult;
            }

            return CreatePropertyExpression(parameter, mappedFieldName);
        }

        property = CreatePropertyExpression(property, mappedFieldName);

        var nestedSorter = options.NestedFlexSorters.FirstOrDefault(x => x.EntityType == property.Type) ??
                           (BaseFlexSorter)Activator.CreateInstance(typeof(FlexSorter<>).MakeGenericType(property.Type),
                               new object[] { })!;

        var reducedSorter = sorter with { Field = string.Join(".", split.Skip(1)) };

        var exp = nestedSorter.BuildExpression(property, reducedSorter);

        return exp;
    }

    private static bool TryBuildExpressionFromCustomEntityField(FlexSorterOptions<TEntity> options, Expression property,
        string fieldName, out Expression expressionResult)
    {
        foreach (var currentOptionsCustomField in options.CustomFields)
        {
            if (currentOptionsCustomField.Field != fieldName) continue;

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

    private static MemberExpression CreatePropertyExpression(Expression parameter, string fieldName)
    {
        try
        {
            return Expression.Property(parameter, fieldName);
        }
        catch (ArgumentException)
        {
            throw new FieldNotFoundException(fieldName);
        }
    }
}