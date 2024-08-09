using System.Linq.Expressions;

namespace FlexFetcher.Utils;

internal static class ExpressionExtensions
{
    public static Expression ReplaceParameter(this Expression expression, Expression oldParameter, Expression newParameter)
    {
        return new ParameterReplacer(oldParameter, newParameter).Visit(expression);
    }

    private class ParameterReplacer(Expression oldParameter, Expression newParameter) : ExpressionVisitor
    {
        private Expression OldParameter { get; } = oldParameter;
        private Expression NewParameter { get; } = newParameter;

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == OldParameter ? NewParameter : base.VisitParameter(node);
        }
    }
}