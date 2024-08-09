using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace FlexFetcher.Utils;

public class PropertyBuilder<TEntity, TProperty, TMapModel> : PropertyBuilderAbstract
    where TEntity : class where TMapModel : class
{
    private readonly string _propertyName;
    private readonly Expression<Func<TEntity, TProperty>> _propertyExpression;
    private readonly HashSet<string> _staticAliases = new();
    private readonly List<Expression<Func<TMapModel, object?>>> _expressions = new();
    private readonly HashSet<string> _aliases = new();

    public override string PropertyName => _propertyName;
    public override string[] Aliases => _aliases.ToArray();

    public PropertyBuilder(Expression<Func<TEntity, TProperty>> propertyExpression)
    {
        _propertyExpression = propertyExpression;
        _propertyName = ((MemberExpression)propertyExpression.Body).Member.Name;
    }

    public PropertyBuilder<TEntity, TProperty, TMapModel> Map(string alias)
    {
        _staticAliases.Add(alias);
        return this;
    }

    public PropertyBuilder<TEntity, TProperty, TMapModel> Map(Expression<Func<TMapModel, object?>> propertyExpression)
    {
        _expressions.Add(propertyExpression);
        return this;
    }

    public override void Build()
    {
        _aliases.Clear();
        _aliases.UnionWith(_staticAliases);

        foreach (var expression in _expressions)
        {
            var alias = ((MemberExpression)expression.Body).Member.Name;
            _aliases.Add(alias);
        }
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