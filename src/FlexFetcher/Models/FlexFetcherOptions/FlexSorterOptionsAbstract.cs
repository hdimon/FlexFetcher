using System.Diagnostics.CodeAnalysis;

namespace FlexFetcher.Models.FlexFetcherOptions;

public abstract class FlexSorterOptionsAbstract
{
    public bool OriginalFieldsHidden { get; protected set; }
    public abstract bool IsBuilt { get; }
    public abstract void Build();
    public abstract bool TryGetFieldNameByAlias(string alias, [MaybeNullWhen(false)] out string fieldName);

    /// <summary>
    /// Hides all original fields of Entity from sorting/filtering,
    /// i.e. if fields are hidden then they can be accessed only by their aliases.
    /// Field aliases are not affected by this method.
    /// </summary>
    public void HideOriginalFields()
    {
        OriginalFieldsHidden = true;
    }
}