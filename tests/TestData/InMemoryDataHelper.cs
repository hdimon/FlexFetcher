using System.Diagnostics.CodeAnalysis;
using TestData.Database;
using TestData.Database.ValueObjects;

namespace TestData;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class InMemoryDataHelper
{
    public static List<GenderEntity> GetGenders()
    {
        var list = new List<GenderEntity>
        {
            new GenderEntity { Id = Gender.Unknown, Name = "Unknown" },
            new GenderEntity { Id = Gender.Male, Name = "Male" },
            new GenderEntity { Id = Gender.Female, Name = "Female" }
        };
        return list;
    }

    public static List<PeopleEntity> GetPeople()
    {
        var user1 = new UserEntity
        {
            Id = 1,
            Name = "John",
            Surname = "Doe"
        };
        var user2 = new UserEntity
        {
            Id = 2,
            Name = "Jane",
            Surname = "Doe"
        };
        var user3 = new UserEntity
        {
            Id = 3,
            Name = "John",
            Surname = "Smith"
        };
        var user4 = new UserEntity
        {
            Id = 4,
            Name = "Jane",
            Surname = "Smith"
        };

        var group1 = new GroupEntity
        {
            Id = 1,
            Name = "Group 1"
        };
        var group2 = new GroupEntity
        {
            Id = 2,
            Name = "Group 2"
        };

        return new List<PeopleEntity>
        {
            new PeopleEntity
            {
                Id = 1,
                ExternalId = new Guid("f3f3f3f3-f3f3-f3f3-f3f3-f3f3f3f3f3f3"),
                Name = "John",
                PeopleName = new PeopleName("John"),
                Surname = "Doe",
                Age = 20,
                Height = 150.5,
                Weight = 50.5,
                Gender = Gender.Male,
                Salary = 50000,
                Occupation = Occupation.Student,
                WorkHours = new TimeSpan(8, 30, 0),
#if NET6_0_OR_GREATER
                WorkStart = new TimeOnly(8, 30, 10),
                BirthDate = new DateOnly(2000, 1, 1),
#endif
                LastLoginUtc = new DateTime(2024, 6, 10, 13, 20, 56, DateTimeKind.Utc),
                LastLogin = new DateTimeOffset(new DateTime(2024, 6, 10, 10, 20, 56), TimeSpan.FromHours(3)),
                Address = new AddressEntity
                {
                    Id = 1,
                    PersonId = 1,
                    Street = "123 Main St",
                    City = "New York",
                    State = "NY",
                    Zip = "12345"
                },
                Phone = "123-456-7890",
                Email = "",
                CreatedByUserId = user1.Id,
                CreatedByUser = user1,
                PeopleGroups = new List<PeopleGroupEntity>
                {
                    new()
                    {
                        PersonId = 1,
                        GroupId = group1.Id,
                        Group = group1
                    }
                }
            },
            new PeopleEntity
            {
                Id = 2,
                ExternalId = new Guid("f4f4f4f4-f4f4-f4f4-f4f4-f4f4f4f4f4f4"),
                Name = "Jane",
                PeopleName = new PeopleName("Jane"),
                Surname = "Doe",
                Age = 25,
                Height = 156,
                Weight = null,
                Gender = Gender.Female,
                Salary = null,
                IsActive = true,
#if NET6_0_OR_GREATER
                BirthDate = new DateOnly(1995, 1, 1),
#endif
                LastLogin = new DateTimeOffset(new DateTime(2024, 6, 10, 10, 20, 56), TimeSpan.FromHours(5)),
                Address = new AddressEntity
                {
                    Id = 2,
                    PersonId = 2,
                    Street = "456 Main St",
                    City = "Los Angeles",
                    State = "CA",
                    Zip = "67890"
                },
                Phone = "234-567-8901",
                Email = "jane.doe@example.com",
                CreatedByUserId = user1.Id,
                CreatedByUser = user1,
                UpdatedByUserId = user1.Id,
                UpdatedByUser = user1,
                PeopleGroups = new List<PeopleGroupEntity>
                {
                    new()
                    {
                        PersonId = 2,
                        GroupId = group2.Id,
                        Group = group2
                    }
                }
            },
            new PeopleEntity
            {
                Id = 3,
                Name = "John",
                PeopleName = new PeopleName("John"),
                Surname = "Smith",
                Age = 30,
                Height = 160.5,
                Weight = null,
                Gender = Gender.Male,
                Salary = 50000.75m,
                OriginCountryEn = "Germany",
#if NET6_0_OR_GREATER
                BirthDate = new DateOnly(1990, 1, 1),
#endif
                Address = new AddressEntity
                {
                    Id = 3,
                    PersonId = 3,
                    Street = "789 Main St",
                    City = "Chicago",
                    State = "IL",
                    Zip = "23456",
                    CountryEn = "Germany",
                },
                Phone = "345-678-9012",
                Email = "",
                CreatedByUserId = user1.Id,
                CreatedByUser = user1,
                UpdatedByUserId = user2.Id,
                UpdatedByUser = user2,
                PeopleGroups = new List<PeopleGroupEntity>
                {
                    new()
                    {
                        PersonId = 3,
                        GroupId = group1.Id,
                        Group = group1
                    }
                }
            },
            new PeopleEntity
            {
                Id = 4,
                Name = "Jane",
                PeopleName = new PeopleName("Jane"),
                Surname = "Smith",
                Age = 35,
                Height = 165,
                Weight = 55.5,
                Gender = Gender.Female,
                Salary = 60000,
                OriginCountryDe = "Deutschland",
#if NET6_0_OR_GREATER
                BirthDate = null,
#endif
                Address = new AddressEntity
                {
                    Id = 4,
                    PersonId = 4,
                    Street = "012 Main St",
                    City = "Houston",
                    State = "TX",
                    Zip = "78901",
                    CountryDe = "Deutschland",
                },
                Phone = "456-789-0123",
                Email = "",
                CreatedByUserId = user2.Id,
                CreatedByUser = user2,
                PeopleGroups = new List<PeopleGroupEntity>
                {
                    new()
                    {
                        PersonId = 4,
                        GroupId = group2.Id,
                        Group = group2
                    }
                }
            },
            new PeopleEntity
            {
                Id = 5,
                Name = "John",
                PeopleName = new PeopleName("John"),
                Surname = "Jones",
                Age = 40,
                Height = 170.5,
                Weight = 60.5,
                Salary = 55000.5m,
                Gender = Gender.Male,
                IsActive = true,
#if NET6_0_OR_GREATER
                BirthDate = new DateOnly(1980, 1, 1),
#endif
                Address = new AddressEntity
                {
                    Id = 5,
                    PersonId = 5,
                    Street = "345 Main St",
                    City = "Philadelphia",
                    State = "PA",
                    Zip = "34567"
                },
                Phone = "567-890-1234",
                Email = "",
                CreatedByUserId = user2.Id,
                CreatedByUser = user2,
                UpdatedByUserId = user1.Id,
                UpdatedByUser = user1,
                PeopleGroups = new List<PeopleGroupEntity>
                {
                    new()
                    {
                        PersonId = 5,
                        GroupId = group1.Id,
                        Group = group1
                    },
                    new()
                    {
                        PersonId = 5,
                        GroupId = group2.Id,
                        Group = group2
                    }
                }
            },
            new PeopleEntity
            {
                Id = 6,
                Name = "Jane",
                PeopleName = new PeopleName("Jane"),
                Surname = "Jones",
                Age = 45,
                Height = 175,
                Weight = 65,
                Gender = Gender.Female,
                Salary = null,
                Occupation = Occupation.Teacher,
#if NET6_0_OR_GREATER
                BirthDate = new DateOnly(1975, 1, 1),
#endif
                Address = new AddressEntity
                {
                    Id = 6,
                    PersonId = 6,
                    Street = "678 Main St",
                    City = "Phoenix",
                    State = "AZ",
                    Zip = "89012"
                },
                Phone = "678-901-2345",
                Email = "",
                CreatedByUserId = user2.Id,
                CreatedByUser = user2,
                UpdatedByUserId = user3.Id,
                UpdatedByUser = user3,
                PeopleGroups = new List<PeopleGroupEntity>
                {
                    new()
                    {
                        PersonId = 6,
                        GroupId = group1.Id,
                        Group = group1
                    },
                    new()
                    {
                        PersonId = 6,
                        GroupId = group2.Id,
                        Group = group2
                    }
                }
            },
            new PeopleEntity
            {
                Id = 7,
                Name = "John",
                PeopleName = new PeopleName("John"),
                Surname = "Williams",
                Age = 50,
                Height = 180.5,
                Weight = 70.5,
                Gender = Gender.Unknown,
                Salary = 60000.5m,
                Occupation = Occupation.Engineer,
#if NET6_0_OR_GREATER
                BirthDate = null,
#endif
                Address = new AddressEntity
                {
                    Id = 7,
                    PersonId = 7,
                    Street = "901 Main St",
                    City = "San Antonio",
                    State = "TX",
                    Zip = "45678"
                },
                Phone = "789-012-3456",
                Email = "",
                CreatedByUserId = user2.Id,
                CreatedByUser = user2,
                UpdatedByUserId = user3.Id,
                UpdatedByUser = user3
            },
            new PeopleEntity
            {
                Id = 8,
                Name = "Jane",
                PeopleName = new PeopleName("Jane"),
                Surname = "Williams",
                Age = 55,
                Height = 185,
                Weight = null,
                Gender = Gender.Female,
                Salary = 65000,
                Address = new AddressEntity
                {
                    Id = 8,
                    PersonId = 8,
                    Street = "234 Main St",
                    City = "San Diego",
                    State = "CA",
                    Zip = "90123"
                },
                Phone = "890-123-4567",
                Email = "",
                CreatedByUserId = user3.Id,
                CreatedByUser = user3,
                UpdatedByUserId = user4.Id,
                UpdatedByUser = user4
            },
            new PeopleEntity
            {
                Id = 9,
                Name = "John",
                PeopleName = new PeopleName("John"),
                Surname = "Brown",
                Age = 60,
                Height = 190.5,
                Weight = 75.5,
                Gender = Gender.Male,
                Salary = null,
                Address = new AddressEntity
                {
                    Id = 9,
                    PersonId = 9,
                    Street = "567 Secondary St",
                    City = "Dallas",
                    State = "TX",
                    Zip = "56789"
                },
                Phone = "901-234-5678",
                Email = "",
                CreatedByUserId = user3.Id,
                CreatedByUser = user3,
                UpdatedByUserId = user2.Id,
                UpdatedByUser = user2
            },
            new PeopleEntity
            {
                Id = 10,
                Name = "Jane",
                PeopleName = new PeopleName("Jane"),
                Surname = "Brown",
                Age = 65,
                Height = 195,
                Gender = Gender.Female,
#if NET6_0_OR_GREATER
                BirthDate = new DateOnly(1955, 1, 1),
#endif
                Phone = "012-345-6789",
                Email = "",
                CreatedByUserId = user4.Id,
                CreatedByUser = user4
            }
        };
    }
}