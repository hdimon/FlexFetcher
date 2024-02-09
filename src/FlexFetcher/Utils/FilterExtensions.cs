using FlexFetcher.ExpressionBuilders;
using FlexFetcher.Models.Queries;
using System.Linq.Expressions;
using FlexFetcher.Models.ExpressionBuilderOptions;

namespace FlexFetcher.Utils;

public static class FilterExtensions
{
    public static IEnumerable<T> FilterData<T>(this IEnumerable<T> query, DataFilters? filters) where T : class
    {
        if (FilterIsEmpty(filters))
            return query;

        var expression = BuildExpression<T>(filters!);

        query = query.Where(expression.Compile());

        return query;
    }

    public static IEnumerable<T> FilterData<T>(this IEnumerable<T> query, DataFilters? filters, Func<string, string> mapField) where T : class
    {
        if (FilterIsEmpty(filters))
            return query;

        var expression = BuildExpression<T>(filters!, mapField);

        query = query.Where(expression.Compile());

        return query;
    }

    public static IQueryable<T> FilterData<T>(this IQueryable<T> query, DataFilters? filters) where T : class
    {
        if (FilterIsEmpty(filters))
            return query;

        var expression = BuildExpression<T>(filters!);

        query = query.Where(expression);

        return query;
    }

    public static IQueryable<T> FilterData<T>(this IQueryable<T> query, DataFilters? filters, Func<string, string> mapField) where T : class
    {
        if (FilterIsEmpty(filters))
            return query;

        var expression = BuildExpression<T>(filters!, mapField);

        query = query.Where(expression);

        return query;
    }

    private static bool FilterIsEmpty(DataFilters? filters)
    {
        if (filters?.Filters == null)
            return true;

        return filters.Filters.Count == 0;
    }

    private static Expression<Func<TEntity, bool>> BuildExpression<TEntity>(DataFilters? filters) where TEntity : class
    {
        var builder = new FilterExpressionBuilder<TEntity>();
        var expression = builder.BuildExpression(filters!, null);
        return expression;
    }

    private static Expression<Func<TEntity, bool>> BuildExpression<TEntity>(DataFilters? filters, Func<string, string> mapField)
        where TEntity : class
    {
        var builderOptions = new FilterExpressionBuilderOptions<TEntity>(mapField, null);

        var builder = new FilterExpressionBuilder<TEntity>();
        var expression = builder.BuildExpression(filters!, builderOptions);
        return expression;
    }
}