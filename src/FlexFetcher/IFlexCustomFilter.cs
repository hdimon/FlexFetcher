namespace FlexFetcher;

public interface IFlexCustomFilter<TEntity> where TEntity : class
{
    string Field { get; }
    //Expression BuildExpression(Expression parameter);
}