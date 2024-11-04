using System.Diagnostics.CodeAnalysis;
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

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class FlexFetcher<TEntity> : BaseFlexFetcher where TEntity : class
{
    public FlexFetcherOptions<TEntity> Options { get; }
#if NETSTANDARD2_0
    public override BaseFlexFilter Filter { get; }
    public override BaseFlexSorter Sorter { get; }
    public override BaseFlexPager Pager { get; }
#else
    public override FlexFilter<TEntity> Filter { get; }
    public override FlexSorter<TEntity> Sorter { get; }
    public override FlexPager<TEntity> Pager { get; }
#endif

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

    public IQueryable<TEntity> FetchData(IQueryable<TEntity> query, DataFilter? filter, DataSorters? sorters, DataPager? pager)
    {
        return FetchData(query, filter, sorters, pager, null);
    }

    public IQueryable<TEntity> FetchData(IQueryable<TEntity> query, DataFilter? filter, DataSorters? sorters, DataPager? pager, IFlexFetcherContext? context)
    {
#if NETSTANDARD2_0
        if (((FlexFilter<TEntity>)Filter).FilterIsEmpty(filter) && ((FlexSorter<TEntity>)Sorter).SorterIsEmpty(sorters) && ((FlexPager<TEntity>)Pager).PagerIsEmpty(pager))
            return query;

        if (!((FlexFilter<TEntity>)Filter).FilterIsEmpty(filter))
        {
            query = ((FlexFilter<TEntity>)Filter).FilterData(query, filter!);
        }

        if (!((FlexSorter<TEntity>)Sorter).SorterIsEmpty(sorters))
        {
            query = ((FlexSorter<TEntity>)Sorter).SortData(query, sorters!);
        }

        if (!((FlexPager<TEntity>)Pager).PagerIsEmpty(pager))
        {
            query = ((FlexPager<TEntity>)Pager).PageData(query, pager!);
        }
#else
        if (Filter.FilterIsEmpty(filter) && Sorter.SorterIsEmpty(sorters) && Pager.PagerIsEmpty(pager))
            return query;

        if (!Filter.FilterIsEmpty(filter))
        {
            query = Filter.FilterData(query, filter!);
        }

        if (!Sorter.SorterIsEmpty(sorters))
        {
            query = Sorter.SortData(query, sorters!);
        }

        if (!Pager.PagerIsEmpty(pager))
        {
            query = Pager.PageData(query, pager!);
        }
#endif

        return query;
    }

    public IEnumerable<TEntity> FetchData(IEnumerable<TEntity> query, DataFilter? filter, DataSorters? sorters, DataPager? pager)
    {
        return FetchData(query, filter, sorters, pager, null);
    }

    public IEnumerable<TEntity> FetchData(IEnumerable<TEntity> query, DataFilter? filter, DataSorters? sorters,
        DataPager? pager, IFlexFetcherContext? context)
    {
#if NETSTANDARD2_0
        if (((FlexFilter<TEntity>)Filter).FilterIsEmpty(filter) && ((FlexSorter<TEntity>)Sorter).SorterIsEmpty(sorters) && ((FlexPager<TEntity>)Pager).PagerIsEmpty(pager))
            return query;

        if (!((FlexFilter<TEntity>)Filter).FilterIsEmpty(filter))
        {
            query = ((FlexFilter<TEntity>)Filter).FilterData(query, filter!);
        }

        if (!((FlexSorter<TEntity>)Sorter).SorterIsEmpty(sorters))
        {
            query = ((FlexSorter<TEntity>)Sorter).SortData(query, sorters!);
        }

        if (!((FlexPager<TEntity>)Pager).PagerIsEmpty(pager))
        {
            query = ((FlexPager<TEntity>)Pager).PageData(query, pager!);
        }
#else
        if (Filter.FilterIsEmpty(filter) && Sorter.SorterIsEmpty(sorters) && Pager.PagerIsEmpty(pager))
            return query;

        if (!Filter.FilterIsEmpty(filter))
        {
            query = Filter.FilterData(query, filter!);
        }

        if (!Sorter.SorterIsEmpty(sorters))
        {
            query = Sorter.SortData(query, sorters!);
        }

        if (!Pager.PagerIsEmpty(pager))
        {
            query = Pager.PageData(query, pager!);
        }
#endif

        return query;
    }
}