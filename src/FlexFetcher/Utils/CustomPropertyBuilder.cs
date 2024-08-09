using System.Diagnostics.CodeAnalysis;

namespace FlexFetcher.Utils;

public class CustomPropertyBuilder<TEntity, TFlexCustomField> : PropertyBuilderAbstract
    where TEntity : class where TFlexCustomField : IFlexCustomField<TEntity>
{
    private readonly string _propertyName;
    private readonly HashSet<string> _staticAliases = new();
    private readonly HashSet<string> _aliases = new();

    public override string PropertyName => _propertyName;
    public override string[] Aliases => _aliases.ToArray();

    public CustomPropertyBuilder(IFlexCustomField<TEntity> field)
    {
        _propertyName = field.Field;
    }

    public CustomPropertyBuilder<TEntity, TFlexCustomField> Map(string alias)
    {
        _staticAliases.Add(alias);
        return this;
    }

    public override void Build()
    {
        _aliases.Clear();
        _aliases.UnionWith(_staticAliases);
    }

    public override bool TryGetPropertyNameByAlias(string alias, [MaybeNullWhen(false)] out string propertyName)
    {
        if (_aliases.Contains(alias))
        {
            propertyName = _propertyName;
            return true;
        }

        propertyName = null;
        return false;
    }
}