using FlexFetcher.ExpressionBuilders;
using FlexFetcher.Utils;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace FlexFetcher.Models.FlexFetcherOptions;

public abstract class BaseFlexOptions<TEntity, TExpressionBuilder> : IFlexOptions
    where TEntity : class where TExpressionBuilder : IExpressionBuilder<TEntity>, new()
{
    protected Dictionary<string, string> FieldNameByAlias { get; } = new();
    protected HashSet<string> HiddenFields { get; } = new();

    public TExpressionBuilder ExpressionBuilder { get; }
    public List<BaseFieldBuilder> FieldBuilders { get; } = new();

    public IImmutableList<IFlexCustomField<TEntity>> CustomFields { get; private set; } =
        ImmutableList<IFlexCustomField<TEntity>>.Empty;

    public bool IsBuilt { get; protected set; }
    public bool OriginalFieldsHidden { get; protected set; }

    protected BaseFlexOptions() : this(new TExpressionBuilder())
    {
    }

    protected BaseFlexOptions(TExpressionBuilder expressionBuilder)
    {
        ExpressionBuilder = expressionBuilder;
    }

    public CustomFieldBuilder<TEntity, IFlexCustomField<TEntity>> AddCustomField(IFlexCustomField<TEntity> customField)
    {
        var builder = new CustomFieldBuilder<TEntity, IFlexCustomField<TEntity>>(customField);
        FieldBuilders.Add(builder);
        var customFields = CustomFields.Add(customField);
        CustomFields = customFields;
        return builder;
    }

    public virtual FieldBuilder<TEntity, TField, TEntity> Field<TField>(Expression<Func<TEntity, TField>> fieldExpression)
    {
        var builder = new FieldBuilder<TEntity, TField, TEntity>(fieldExpression);
        FieldBuilders.Add(builder);
        return builder;
    }

    public void Build()
    {
        if (IsBuilt)
            return;

        BuildAliases();
        BuildHiddenFields();

        IsBuilt = true;
    }

    public bool TryGetFieldNameByAlias(string alias,
#if NET5_0_OR_GREATER
[MaybeNullWhen(false)]
#endif
        out string fieldName)
    {
        if (FieldNameByAlias.TryGetValue(alias, out fieldName))
            return true;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        fieldName = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        return false;
    }

    /// <summary>
    /// Hides all original fields of Entity from sorting/filtering,
    /// i.e. if fields are hidden then they can be accessed only by their aliases.
    /// Field aliases are not affected by this method.
    /// </summary>
    public void HideOriginalFields()
    {
        OriginalFieldsHidden = true;
    }

    public bool IsHiddenField(string fieldName)
    {
        return HiddenFields.Contains(fieldName);
    }

    public Type? GetFieldCastToType(string fieldName)
    {
        var fieldBuilder = FieldBuilders.FirstOrDefault(x => x.FieldName == fieldName);
        return fieldBuilder?.CastToType;
    }

    internal void AddFieldBuilderInternal(BaseFieldBuilder fieldBuilder)
    {
        FieldBuilders.Add(fieldBuilder);
    }

    internal void AddCustomFieldInternal(IFlexCustomField<TEntity> customField)
    {
        var customFields = CustomFields.Add(customField);
        CustomFields = customFields;
    }

    private void BuildAliases()
    {
        foreach (var fieldBuilder in FieldBuilders)
        {
            fieldBuilder.Build();

            foreach (var alias in fieldBuilder.Aliases)
            {
                FieldNameByAlias[alias] = fieldBuilder.FieldName;
            }
        }
    }

    private void BuildHiddenFields()
    {
        if (OriginalFieldsHidden)
        {
            // Get all fields of TEntity
            var properties = typeof(TEntity).GetProperties();
            foreach (var property in properties)
            {
                var fieldName = property.Name;
                HiddenFields.Add(fieldName);
            }
        }
        else
        {
            foreach (var fieldBuilder in FieldBuilders)
            {
                if (fieldBuilder.IsHidden)
                    HiddenFields.Add(fieldBuilder.FieldName);
            }
        }
    }
}