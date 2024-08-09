using FlexFetcher.ExpressionBuilders;
using System.Collections.Immutable;
using FlexFetcher.Utils;
using System.Linq.Expressions;
using System.Diagnostics.CodeAnalysis;

namespace FlexFetcher.Models.FlexFetcherOptions;

public class FlexSorterOptions<TEntity, TModel> : FlexSorterOptions<TEntity> where TEntity : class where TModel : class
{
    public new PropertyBuilder<TEntity, TProperty, TModel> Property<TProperty>(
        Expression<Func<TEntity, TProperty>> propertyExpression)
    {
        var builder = new PropertyBuilder<TEntity, TProperty, TModel>(propertyExpression);
        PropertyBuilders.Add(builder);
        return builder;
    }
}

public class FlexSorterOptions<TEntity> : FlexSorterOptionsAbstract where TEntity : class
{
    private bool _arePropertiesBuilt;
    private Dictionary<string, string> _propertyNameByAlias { get; } = new();

    public SorterExpressionBuilder<TEntity> ExpressionBuilder { get; }
    public IImmutableList<BaseFlexSorter> NestedFlexSorters { get; private set; }
    public List<PropertyBuilderAbstract> PropertyBuilders { get; } = new();

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

    public CustomPropertyBuilder<TEntity, IFlexCustomField<TEntity>> AddCustomField(IFlexCustomField<TEntity> customField)
    {
        var builder = new CustomPropertyBuilder<TEntity, IFlexCustomField<TEntity>>(customField);
        PropertyBuilders.Add(builder);
        var customFields = CustomFields.Add(customField);
        CustomFields = customFields;
        return builder;
    }

    public virtual PropertyBuilder<TEntity, TProperty, TEntity> Property<TProperty>(
        Expression<Func<TEntity, TProperty>> propertyExpression)
    {
        var builder = new PropertyBuilder<TEntity, TProperty, TEntity>(propertyExpression);
        PropertyBuilders.Add(builder);
        return builder;
    }

    public override bool ArePropertiesBuilt => _arePropertiesBuilt;

    public override void BuildProperties()
    {
        if (_arePropertiesBuilt)
            return;

        foreach (var propertyBuilder in PropertyBuilders)
        {
            propertyBuilder.Build();

            foreach (var alias in propertyBuilder.Aliases)
            {
                _propertyNameByAlias[alias] = propertyBuilder.PropertyName;
            }
        }

        _arePropertiesBuilt = true;
    }

    public override bool TryGetPropertyNameByAlias(string alias, [MaybeNullWhen(false)] out string propertyName)
    {
        if (_propertyNameByAlias.TryGetValue(alias, out propertyName))
            return true;

        propertyName = null;
        return false;
    }
}