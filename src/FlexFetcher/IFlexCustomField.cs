namespace FlexFetcher;

// ReSharper disable once UnusedTypeParameter
public interface IFlexCustomField<TEntity> where TEntity : class
{
    string Field { get; }
}