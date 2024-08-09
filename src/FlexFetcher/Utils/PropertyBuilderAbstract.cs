using System.Diagnostics.CodeAnalysis;

namespace FlexFetcher.Utils;

public abstract class PropertyBuilderAbstract
{
    public abstract string PropertyName { get; }
    public abstract string[] Aliases { get; }
    public abstract void Build();
    public abstract bool TryGetPropertyNameByAlias(string alias, [MaybeNullWhen(false)] out string propertyName);
}