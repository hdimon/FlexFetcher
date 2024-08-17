using FlexFetcher.Models.FlexFetcherOptions;
using FlexFetcher.Models.Queries;

namespace FlexFetcher;

public class FlexFetcher<TEntity, TModel> : FlexFetcher<TEntity> where TEntity : class where TModel : class
{
    public FlexFetcher() : this(new FlexFetcherOptions<TEntity, TModel>())
    {
    }

    public FlexFetcher(FlexFetcherOptions<TEntity, TModel> options) : base(options)
    {
    }

    public FlexFetcher(FlexFilter<TEntity, TModel> filter) : base(filter)
    {
    }

    public FlexFetcher(FlexSorter<TEntity, TModel> sorter) : base(sorter)
    {
    }

    public FlexFetcher(FlexPager<TEntity, TModel> pager) : base(pager)
    {
    }

    public FlexFetcher(FlexFilter<TEntity, TModel> filter, FlexSorter<TEntity, TModel> sorter) : base(filter, sorter)
    {
    }

    public FlexFetcher(FlexFilter<TEntity, TModel> filter, FlexPager<TEntity, TModel> pager) : base(filter, pager)
    {
    }

    public FlexFetcher(FlexSorter<TEntity, TModel> sorter, FlexPager<TEntity, TModel> pager) : base(sorter, pager)
    {
    }

    public FlexFetcher(FlexFilter<TEntity, TModel> filter, FlexSorter<TEntity, TModel> sorter, FlexPager<TEntity, TModel> pager) :
        base(filter, sorter, pager)
    {
    }
}

public class FlexFetcher<TEntity> where TEntity : class
{
    public FlexFetcherOptions<TEntity> Options { get; }
    public FlexFilter<TEntity> Filter { get; init; }
    public FlexSorter<TEntity> Sorter { get; init; }
    public FlexPager<TEntity> Pager { get; init; }

    public FlexFetcher() : this(new FlexFetcherOptions<TEntity>())
    {
    }

    public FlexFetcher(FlexFetcherOptions<TEntity> options)
    {
        Options = options;
        Filter = new FlexFilter<TEntity>(options.FilterOptions);
        Sorter = new FlexSorter<TEntity>(options.SorterOptions);
        Pager = new FlexPager<TEntity>();
    }

    public FlexFetcher(FlexFilter<TEntity> filter)
    {
        Options = new FlexFetcherOptions<TEntity>(filter.Options);
        Filter = filter;
        Sorter = new FlexSorter<TEntity>();
        Pager = new FlexPager<TEntity>();
    }

    public FlexFetcher(FlexSorter<TEntity> sorter)
    {
        Options = new FlexFetcherOptions<TEntity>(sorter.Options);
        Filter = new FlexFilter<TEntity>();
        Sorter = sorter;
        Pager = new FlexPager<TEntity>();
    }

    public FlexFetcher(FlexPager<TEntity> pager)
    {
        Options = new FlexFetcherOptions<TEntity>();
        Filter = new FlexFilter<TEntity>();
        Sorter = new FlexSorter<TEntity>();
        Pager = pager;
    }

    public FlexFetcher(FlexFilter<TEntity> filter, FlexSorter<TEntity> sorter)
    {
        Options = new FlexFetcherOptions<TEntity>(filter.Options, sorter.Options);
        Filter = filter;
        Sorter = sorter;
        Pager = new FlexPager<TEntity>();
    }

    public FlexFetcher(FlexFilter<TEntity> filter, FlexPager<TEntity> pager)
    {
        Options = new FlexFetcherOptions<TEntity>(filter.Options);
        Filter = filter;
        Sorter = new FlexSorter<TEntity>();
        Pager = pager;
    }

    public FlexFetcher(FlexSorter<TEntity> sorter, FlexPager<TEntity> pager)
    {
        Options = new FlexFetcherOptions<TEntity>(sorter.Options);
        Filter = new FlexFilter<TEntity>();
        Sorter = sorter;
        Pager = pager;
    }

    public FlexFetcher(FlexFilter<TEntity> filter, FlexSorter<TEntity> sorter, FlexPager<TEntity> pager)
    {
        Options = new FlexFetcherOptions<TEntity>(filter.Options, sorter.Options);
        Filter = filter;
        Sorter = sorter;
        Pager = pager;
    }

    public IQueryable<TEntity> FetchData(IQueryable<TEntity> query, DataFilters? filters, DataSorters? sorters, DataPager? pager)
    {
        if (Filter.FilterIsEmpty(filters) && Sorter.SorterIsEmpty(sorters) && Pager.PagerIsEmpty(pager))
            return query;

        if (!Filter.FilterIsEmpty(filters))
        {
            query = Filter.FilterData(query, filters!);
        }

        if (!Sorter.SorterIsEmpty(sorters))
        {
            query = Sorter.SortData(query, sorters!);
        }

        if (!Pager.PagerIsEmpty(pager))
        {
            query = Pager.PageData(query, pager!);
        }

        return query;
    }

    public IEnumerable<TEntity> FetchData(IEnumerable<TEntity> query, DataFilters? filters, DataSorters? sorters,
        DataPager? pager)
    {
        if (Filter.FilterIsEmpty(filters) && Sorter.SorterIsEmpty(sorters) && Pager.PagerIsEmpty(pager))
            return query;

        if (!Filter.FilterIsEmpty(filters))
        {
            query = Filter.FilterData(query, filters!);
        }

        if (!Sorter.SorterIsEmpty(sorters))
        {
            query = Sorter.SortData(query, sorters!);
        }

        if (!Pager.PagerIsEmpty(pager))
        {
            query = Pager.PageData(query, pager!);
        }

        return query;
    }
}