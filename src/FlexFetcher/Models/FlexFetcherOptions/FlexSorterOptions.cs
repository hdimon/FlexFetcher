using FlexFetcher.ExpressionBuilders;
using System.Collections.Immutable;
using FlexFetcher.Utils;
using System.Linq.Expressions;

namespace FlexFetcher.Models.FlexFetcherOptions;

public class FlexSorterOptions<TEntity, TModel> : FlexSorterOptions<TEntity> where TEntity : class where TModel : class
{
    public new FieldBuilder<TEntity, TField, TModel> Field<TField>(
        Expression<Func<TEntity, TField>> fieldExpression)
    {
        var builder = new FieldBuilder<TEntity, TField, TModel>(fieldExpression);
        FieldBuilders.Add(builder);
        return builder;
    }
}

public class FlexSorterOptions<TEntity> : BaseFlexOptions<TEntity, SorterExpressionBuilder<TEntity>> where TEntity : class
{
    public IImmutableList<BaseFlexSorter> NestedFlexSorters { get; private set; }

    public FlexSorterOptions() : this(new SorterExpressionBuilder<TEntity>())
    {
    }

    public FlexSorterOptions(SorterExpressionBuilder<TEntity> expressionBuilder) : base(expressionBuilder)
    {
        NestedFlexSorters = ImmutableList<BaseFlexSorter>.Empty;
    }

    public void AddNestedFlexSorter(BaseFlexSorter flexSorter)
    {
        var nestedFlexSorters = NestedFlexSorters.Add(flexSorter);
        NestedFlexSorters = nestedFlexSorters;
    }
}