using FlexFetcher.ExpressionBuilders;
using FlexFetcher.Models.Queries;
using FlexFetcher.Models.FlexFetcherOptions;

namespace FlexFetcher.Utils;

public static class SorterExtensions
{
    public static IEnumerable<T> SortData<T>(this IEnumerable<T> query, DataSorters? sorters) where T : class
    {
        if (SorterIsEmpty(sorters))
            return query;

        query = BuildExpression(query.AsQueryable(), sorters!);

        return query;
    }

    public static IEnumerable<T> SortData<T>(this IEnumerable<T> query, DataSorters? sorters, FlexSorterOptions<T> options)
        where T : class
    {
        if (SorterIsEmpty(sorters))
            return query;

        query = BuildExpression(query.AsQueryable(), sorters!, options);

        return query;
    }

    public static IQueryable<T> SortData<T>(this IQueryable<T> query, DataSorters? sorters) where T : class
    {
        if (SorterIsEmpty(sorters))
            return query;

        query = BuildExpression(query, sorters!);

        return query;
    }

    public static IQueryable<T> SortData<T>(this IQueryable<T> query, DataSorters? sorters, FlexSorterOptions<T> options)
        where T : class
    {
        if (SorterIsEmpty(sorters))
            return query;

        query = BuildExpression(query, sorters!, options);

        return query;
    }

    private static bool SorterIsEmpty(DataSorters? sorters)
    {
        if (sorters?.Sorters == null)
            return true;

        return sorters.Sorters.Count == 0;
    }

    private static IQueryable<TEntity> BuildExpression<TEntity>(IQueryable<TEntity> query, DataSorters? sorters)
        where TEntity : class
    {
        var builder = new SorterExpressionBuilder<TEntity>();
        var flexSorterOptions = new FlexSorterOptions<TEntity>();
        flexSorterOptions.BuildProperties();
        var expression = builder.BuildExpression(query, sorters!, flexSorterOptions);
        return expression;
    }

    private static IQueryable<TEntity> BuildExpression<TEntity>(IQueryable<TEntity> query, DataSorters? sorters,
        FlexSorterOptions<TEntity> options) where TEntity : class
    {
        var builder = new SorterExpressionBuilder<TEntity>();

        if (!options.ArePropertiesBuilt)
            options.BuildProperties();

        var expression = builder.BuildExpression(query, sorters!, options);
        return expression;
    }
}