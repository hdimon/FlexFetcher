using FlexFetcher.Models.Queries;
using System.Linq.Expressions;

namespace FlexFetcher.ExpressionBuilders;

public interface IFilterExpressionHandler
{
    string Operator { get; }
    Expression BuildExpression(Expression property, DataFilter filter);
}