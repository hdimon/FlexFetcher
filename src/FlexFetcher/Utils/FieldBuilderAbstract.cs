using System.Diagnostics.CodeAnalysis;

namespace FlexFetcher.Utils;

public abstract class FieldBuilderAbstract
{
    public bool IsHidden { get; protected set; }

    public abstract string FieldName { get; }
    public abstract string[] Aliases { get; }
    public abstract void Build();
    public abstract bool TryGetFieldNameByAlias(string alias, [MaybeNullWhen(false)] out string fieldName);
    
    /// <summary>
    /// Hides original field from sorting/filtering,
    /// i.e. if field is hidden then it can be accessed only by its aliases.
    /// Field aliases are not affected by this method.
    /// </summary>
    public void Hide()
    {
        IsHidden = true;
    }
}