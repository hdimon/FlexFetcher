using FlexFetcher.Models.Queries;
using System.Linq.Expressions;

namespace FlexFetcher;

public abstract class BaseFlexSorter
{
    public abstract Type EntityType { get; }
    public abstract Expression BuildExpression(Expression property, DataSorter sorter);
}