using System.Linq.Expressions;

namespace FlexFetcher.Utils;

public class FieldBuilder<TEntity, TField, TMapModel> : BaseFieldBuilder
    where TEntity : class where TMapModel : class
{
    // ReSharper disable once NotAccessedField.Local
    private readonly Expression<Func<TEntity, TField>> _fieldExpression;
    private readonly HashSet<string> _staticAliases = new();
    private readonly List<Expression<Func<TMapModel, object?>>> _expressions = new();
    private readonly HashSet<string> _aliases = new();
    
    public override string[] Aliases => _aliases.ToArray();

    public FieldBuilder(Expression<Func<TEntity, TField>> fieldExpression) : base(((MemberExpression)fieldExpression.Body).Member.Name)
    {
        _fieldExpression = fieldExpression;
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

    public FieldBuilder<TEntity, TField, TMapModel> CastTo(Type type)
    {
        CastToType = type;
        return this;
    }

    public FieldBuilder<TEntity, TField, TMapModel> CastTo<TTargetType>()
    {
        CastToType = typeof(TTargetType);
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
}