using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace FlexFetcher.Utils;

public class FieldBuilder<TEntity, TField, TMapModel> : FieldBuilderAbstract
    where TEntity : class where TMapModel : class
{
    private readonly string _fieldName;
    private readonly Expression<Func<TEntity, TField>> _fieldExpression;
    private readonly HashSet<string> _staticAliases = new();
    private readonly List<Expression<Func<TMapModel, object?>>> _expressions = new();
    private readonly HashSet<string> _aliases = new();

    public override string FieldName => _fieldName;
    public override string[] Aliases => _aliases.ToArray();

    public FieldBuilder(Expression<Func<TEntity, TField>> fieldExpression)
    {
        _fieldExpression = fieldExpression;
        _fieldName = ((MemberExpression)fieldExpression.Body).Member.Name;
    }

    public FieldBuilder<TEntity, TField, TMapModel> Map(string alias)
    {
        _staticAliases.Add(alias);
        return this;
    }

    public FieldBuilder<TEntity, TField, TMapModel> Map(Expression<Func<TMapModel, object?>> fieldExpression)
    {
        _expressions.Add(fieldExpression);
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