using System;
using System.Collections.Generic;
using TestData.Database.ValueObjects;

namespace TestData.Database;

public class PeopleEntity
{
    public int Id { get; set; }
    public Guid? ExternalId { get; set; }
    public string? Name { get; set; }
    public PeopleName? PeopleName { get; set; }
    public string? Surname { get; set; }
    public int Age { get; set; }
    public double Height { get; set; }
    public double? Weight { get; set; }
    public decimal? Salary { get; set; }
    public string? OriginCountryEn { get; set; }
    public string? OriginCountryDe { get; set; }
    public TimeSpan? WorkHours { get; set; }
#if NET6_0_OR_GREATER
    public TimeOnly? WorkStart { get; set; }
    public DateOnly? BirthDate { get; set; }
#endif
    public DateTime? LastLoginUtc { get; set; }
    public DateTimeOffset? LastLogin { get; set; }
    public bool IsActive { get; set; }
    public AddressEntity? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public Occupation? Occupation { get; set; }
    public int CreatedByUserId { get; set; }
    public UserEntity? CreatedByUser { get; set; }
    public int? UpdatedByUserId { get; set; }
    public UserEntity? UpdatedByUser { get; set; }
    public Gender Gender { get; set; }
    public GenderEntity GenderEntity { get; set; } = null!;
    public List<PeopleGroupEntity> PeopleGroups { get; set; } = new();
}