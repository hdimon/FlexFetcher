using System.Linq.Expressions;
using FlexFetcher.Models.Queries;

namespace FlexFetcher;

public abstract class BaseFlexFilter
{
    public abstract Type EntityType { get; }
    public abstract Expression BuildExpression(Expression property, DataFilter filter);
}