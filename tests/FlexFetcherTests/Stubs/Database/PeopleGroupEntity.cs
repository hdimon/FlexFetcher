namespace FlexFetcherTests.Stubs.Database;

public class PeopleGroupEntity
{
    public int PersonId { get; set; }
    public PeopleEntity Person { get; set; } = null!;
    public int GroupId { get; set; }
    public GroupEntity Group { get; set; } = null!;
}