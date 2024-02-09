namespace FlexFetcherTests.Stubs.Database;

public class PeopleEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public int Age { get; set; }
    public AddressEntity? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int CreatedByUserId { get; set; }
    public UserEntity CreatedByUser { get; set; } = null!;
    public int? UpdatedByUserId { get; set; }
    public UserEntity? UpdatedByUser { get; set; }
    public List<PeopleGroupEntity> PeopleGroups { get; set; } = new();
}