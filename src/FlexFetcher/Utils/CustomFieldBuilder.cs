namespace FlexFetcher.Utils;

public class CustomFieldBuilder<TEntity, TFlexCustomField> : BaseFieldBuilder
    where TEntity : class where TFlexCustomField : IFlexCustomField<TEntity>
{
    private readonly HashSet<string> _staticAliases = new();
    private readonly HashSet<string> _aliases = new();

    public override string[] Aliases => _aliases.ToArray();

    public CustomFieldBuilder(IFlexCustomField<TEntity> field) : base(field.Field)
    {
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
}