using FlexFetcher.Models.Queries;

namespace FlexFetcher.Utils;

public static class PagerExtensions
{
    public static IEnumerable<T> PageData<T>(this IEnumerable<T> query, DataPager? pager) where T : class
    {
        var flexPager = new FlexPager<T>();

        return flexPager.PageData(query, pager);
    }

    public static IQueryable<T> PageData<T>(this IQueryable<T> query, DataPager? pager) where T : class
    {
        var flexPager = new FlexPager<T>();

        return flexPager.PageData(query, pager);
    }
}