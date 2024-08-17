using FlexFetcher.ExpressionBuilders;
using FlexFetcher.Models.Queries;
using System.Linq.Expressions;
using FlexFetcher.Models.FlexFetcherOptions;

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

    public static IEnumerable<T> FilterData<T>(this IEnumerable<T> query, DataFilters? filters, FlexFilterOptions<T> options)
        where T : class
    {
        if (FilterIsEmpty(filters))
            return query;

        var expression = BuildExpression(filters!, options);

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

    public static IQueryable<T> FilterData<T>(this IQueryable<T> query, DataFilters? filters, FlexFilterOptions<T> options)
        where T : class
    {
        if (FilterIsEmpty(filters))
            return query;

        var expression = BuildExpression(filters!, options);

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
        var options = new FlexFilterOptions<TEntity>();
        options.Build();
        var expression = builder.BuildExpression(filters!, options);
        return expression;
    }

    private static Expression<Func<TEntity, bool>> BuildExpression<TEntity>(DataFilters? filters,
        FlexFilterOptions<TEntity> options)
        where TEntity : class
    {
        var builder = new FilterExpressionBuilder<TEntity>();

        if (!options.IsBuilt)
            options.Build();

        var expression = builder.BuildExpression(filters!, options);
        return expression;
    }
}