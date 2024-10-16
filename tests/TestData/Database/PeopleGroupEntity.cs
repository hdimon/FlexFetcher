namespace TestData.Database;

public class PeopleGroupEntity
{
    public int PersonId { get; set; }
    public PeopleEntity? Person { get; set; }
    public int GroupId { get; set; }
    public GroupEntity? Group { get; set; }
}