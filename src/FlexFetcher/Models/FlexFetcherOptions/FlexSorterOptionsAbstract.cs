using System.Diagnostics.CodeAnalysis;

namespace FlexFetcher.Models.FlexFetcherOptions;

public abstract class FlexSorterOptionsAbstract
{
    public abstract bool ArePropertiesBuilt { get; }
    public abstract void BuildProperties();
    public abstract bool TryGetPropertyNameByAlias(string alias, [MaybeNullWhen(false)] out string propertyName);
}