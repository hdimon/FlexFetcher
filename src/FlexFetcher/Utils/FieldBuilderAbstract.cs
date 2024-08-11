using System.Diagnostics.CodeAnalysis;

namespace FlexFetcher.Utils;

public abstract class FieldBuilderAbstract
{
    public abstract string FieldName { get; }
    public abstract string[] Aliases { get; }
    public abstract void Build();
    public abstract bool TryGetFieldNameByAlias(string alias, [MaybeNullWhen(false)] out string fieldName);
}