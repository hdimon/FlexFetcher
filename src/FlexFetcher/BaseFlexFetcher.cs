namespace FlexFetcher;

public abstract class BaseFlexFetcher
{
    public abstract BaseFlexFilter Filter { get; }
    public abstract BaseFlexSorter Sorter { get; }
    public abstract BaseFlexPager Pager { get; }
}