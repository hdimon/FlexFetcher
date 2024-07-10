namespace FlexFetcher;

// ReSharper disable once UnusedTypeParameter
public interface IFlexCustomFilter<TEntity> where TEntity : class
{
    string Field { get; }
}