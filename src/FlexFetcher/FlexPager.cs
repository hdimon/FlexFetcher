using FlexFetcher.Models.Queries;

namespace FlexFetcher;

public class FlexPager<TEntity, TModel> : FlexPager<TEntity> where TEntity : class where TModel : class
{
}

public class FlexPager<TEntity> where TEntity : class
{
    public IQueryable<TEntity> PageData(IQueryable<TEntity> query, DataPager? pager)
    {
        if (PagerIsEmpty(pager))
            return query;

        ValidatePager(pager!);

        var (skip, take) = CalculateSkipAndTake(pager!);

        return query.Skip(skip).Take(take);
    }

    public IEnumerable<TEntity> PageData(IEnumerable<TEntity> query, DataPager? pager)
    {
        if (PagerIsEmpty(pager))
            return query;

        ValidatePager(pager!);

        var (skip, take) = CalculateSkipAndTake(pager!);

        return query.Skip(skip).Take(take);
    }

    public bool PagerIsValid(DataPager pager)
    {
        if (pager is { PageSize: <= 0, Take: <= 0 })
            return false;

        if (pager is { PageSize: > 0, Page: < 1 })
            return false;

        if (pager is { Take: > 0, Skip: < 0 })
            return false;

        return true;
    }

    public bool PagerIsEmpty(DataPager? pager)
    {
        return pager == null;
    }

    private void ValidatePager(DataPager pager)
    {
        if (!PagerIsValid(pager))
            throw new ArgumentException($"Invalid pager: {nameof(DataPager.PageSize)} and {nameof(DataPager.Page)} " +
                                        $"or {nameof(DataPager.Take)} must be provided.");
    }

    private (int skip, int take) CalculateSkipAndTake(DataPager pager)
    {
        int skip;
        int take;

        if (pager.PageSize > 0)
        {
            skip = (pager.Page - 1) * pager.PageSize;
            take = pager.PageSize;
        }
        else
        {
            skip = pager.Skip;
            take = pager.Take;
        }

        return (skip, take);
    }
}