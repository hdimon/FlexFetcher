using FlexFetcher.ExpressionBuilders;
using FlexFetcher.Models.Queries;
using System.Linq.Expressions;
using FlexFetcher.Models.FlexFetcherOptions;

namespace FlexFetcher.Utils;

public static class FilterExtensions
{
    public static IEnumerable<T> FilterData<T>(this IEnumerable<T> query, DataFilter? filter) where T : class
    {
        if (FilterIsEmpty(filter))
            return query;

        var expression = BuildExpression<T>(filter!);

        query = query.Where(expression.Compile());

        return query;
    }

    public static IEnumerable<T> FilterData<T>(this IEnumerable<T> query, DataFilter? filter, FlexFilterOptions<T> options)
        where T : class
    {
        if (FilterIsEmpty(filter))
            return query;

        var expression = BuildExpression(filter!, options);

        query = query.Where(expression.Compile());

        return query;
    }

    public static IQueryable<T> FilterData<T>(this IQueryable<T> query, DataFilter? filter) where T : class
    {
        if (FilterIsEmpty(filter))
            return query;

        var expression = BuildExpression<T>(filter!);

        query = query.Where(expression);

        return query;
    }

    public static IQueryable<T> FilterData<T>(this IQueryable<T> query, DataFilter? filter, FlexFilterOptions<T> options)
        where T : class
    {
        if (FilterIsEmpty(filter))
            return query;

        var expression = BuildExpression(filter!, options);

        query = query.Where(expression);

        return query;
    }

    private static bool FilterIsEmpty(DataFilter? filter)
    {
        if (filter?.Filters == null)
            return true;

        return filter.Filters.Count == 0;
    }

    private static Expression<Func<TEntity, bool>> BuildExpression<TEntity>(DataFilter? filter) where TEntity : class
    {
        var builder = new FilterExpressionBuilder<TEntity>();
        var options = new FlexFilterOptions<TEntity>();
        options.Build();
        var expression = builder.BuildExpression(filter!, options, null);
        return expression;
    }

    private static Expression<Func<TEntity, bool>> BuildExpression<TEntity>(DataFilter? filter,
        FlexFilterOptions<TEntity> options)
        where TEntity : class
    {
        var builder = new FilterExpressionBuilder<TEntity>();

        if (!options.IsBuilt)
            options.Build();

        var expression = builder.BuildExpression(filter!, options, null);
        return expression;
    }
}