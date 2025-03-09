namespace TestData.Database.ValueObjects;

public class PeopleCreatedByUserId : ValueObjectBase
{
    public int Value { get; }

    public PeopleCreatedByUserId(int value)
    {
        Value = value;
    }

    public static implicit operator int(PeopleCreatedByUserId createdByUserId) => createdByUserId.Value;

    public override string ToString()
    {
        return Value.ToString();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}