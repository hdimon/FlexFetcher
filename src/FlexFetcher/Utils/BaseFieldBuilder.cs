namespace FlexFetcher.Utils;

public abstract class BaseFieldBuilder
{
    public string FieldName { get; protected init; }
    public bool IsHidden { get; protected set; }

    public abstract string[] Aliases { get; }
    public abstract void Build();

    protected BaseFieldBuilder(string fieldName)
    {
        FieldName = fieldName;
    }

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