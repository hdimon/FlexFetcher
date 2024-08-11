using FlexFetcher.ExpressionBuilders;
using System.Collections.Immutable;
using FlexFetcher.Utils;
using System.Linq.Expressions;
using System.Diagnostics.CodeAnalysis;

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

public class FlexSorterOptions<TEntity> : FlexSorterOptionsAbstract where TEntity : class
{
    private bool _arePropertiesBuilt;
    private Dictionary<string, string> _fieldNameByAlias { get; } = new();

    public SorterExpressionBuilder<TEntity> ExpressionBuilder { get; }
    public IImmutableList<BaseFlexSorter> NestedFlexSorters { get; private set; }
    public List<FieldBuilderAbstract> FieldBuilders { get; } = new();

    public IImmutableList<IFlexCustomField<TEntity>> CustomFields { get; private set; } =
        ImmutableList<IFlexCustomField<TEntity>>.Empty;

    public FlexSorterOptions() : this(new SorterExpressionBuilder<TEntity>())
    {
    }

    public FlexSorterOptions(SorterExpressionBuilder<TEntity> expressionBuilder)
    {
        ExpressionBuilder = expressionBuilder;
        NestedFlexSorters = ImmutableList<BaseFlexSorter>.Empty;
    }

    public void AddNestedFlexSorter(BaseFlexSorter flexSorter)
    {
        var nestedFlexSorters = NestedFlexSorters.Add(flexSorter);
        NestedFlexSorters = nestedFlexSorters;
    }

    public CustomFieldBuilder<TEntity, IFlexCustomField<TEntity>> AddCustomField(IFlexCustomField<TEntity> customField)
    {
        var builder = new CustomFieldBuilder<TEntity, IFlexCustomField<TEntity>>(customField);
        FieldBuilders.Add(builder);
        var customFields = CustomFields.Add(customField);
        CustomFields = customFields;
        return builder;
    }

    public virtual FieldBuilder<TEntity, TField, TEntity> Field<TField>(Expression<Func<TEntity, TField>> propertyExpression)
    {
        var builder = new FieldBuilder<TEntity, TField, TEntity>(propertyExpression);
        FieldBuilders.Add(builder);
        return builder;
    }

    public override bool ArePropertiesBuilt => _arePropertiesBuilt;

    public override void BuildProperties()
    {
        if (_arePropertiesBuilt)
            return;

        foreach (var fieldBuilder in FieldBuilders)
        {
            fieldBuilder.Build();

            foreach (var alias in fieldBuilder.Aliases)
            {
                _fieldNameByAlias[alias] = fieldBuilder.FieldName;
            }
        }

        _arePropertiesBuilt = true;
    }

    public override bool TryGetFieldNameByAlias(string alias, [MaybeNullWhen(false)] out string fieldName)
    {
        if (_fieldNameByAlias.TryGetValue(alias, out fieldName))
            return true;

        fieldName = null;
        return false;
    }
}