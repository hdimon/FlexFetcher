using System.Diagnostics.CodeAnalysis;

namespace FlexFetcher.Utils;

public class CustomFieldBuilder<TEntity, TFlexCustomField> : FieldBuilderAbstract
    where TEntity : class where TFlexCustomField : IFlexCustomField<TEntity>
{
    private readonly string _fieldName;
    private readonly HashSet<string> _staticAliases = new();
    private readonly HashSet<string> _aliases = new();

    public override string FieldName => _fieldName;
    public override string[] Aliases => _aliases.ToArray();

    public CustomFieldBuilder(IFlexCustomField<TEntity> field)
    {
        _fieldName = field.Field;
    }

    public CustomFieldBuilder<TEntity, TFlexCustomField> Map(string alias)
    {
        _staticAliases.Add(alias);
        return this;
    }

    public override void Build()
    {
        _aliases.Clear();
        _aliases.UnionWith(_staticAliases);
    }

    public override bool TryGetFieldNameByAlias(string alias, [MaybeNullWhen(false)] out string fieldName)
    {
        if (_aliases.Contains(alias))
        {
            fieldName = _fieldName;
            return true;
        }

        fieldName = null;
        return false;
    }
}