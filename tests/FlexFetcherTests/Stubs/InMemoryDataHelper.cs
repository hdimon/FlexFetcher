﻿using FlexFetcherTests.Stubs.Database;

namespace FlexFetcherTests.Stubs;

public class InMemoryDataHelper
{
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
                Name = "John",
                Surname = "Doe",
                Age = 20,
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
                Name = "Jane",
                Surname = "Doe",
                Age = 25,
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
                Surname = "Smith",
                Age = 30,
                Address = new AddressEntity
                {
                    Id = 3,
                    PersonId = 3,
                    Street = "789 Main St",
                    City = "Chicago",
                    State = "IL",
                    Zip = "23456"
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
                Surname = "Smith",
                Age = 35,
                Address = new AddressEntity
                {
                    Id = 4,
                    PersonId = 4,
                    Street = "012 Main St",
                    City = "Houston",
                    State = "TX",
                    Zip = "78901"
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
                Surname = "Jones",
                Age = 40,
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
                Surname = "Jones",
                Age = 45,
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
                Surname = "Williams",
                Age = 50,
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
                Surname = "Williams",
                Age = 55,
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
                Surname = "Brown",
                Age = 60,
                Address = new AddressEntity
                {
                    Id = 9,
                    PersonId = 9,
                    Street = "567 Main St",
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
                Surname = "Brown",
                Age = 65,
                Phone = "012-345-6789",
                Email = "",
                CreatedByUserId = user4.Id,
                CreatedByUser = user4
            }
        };
    }
}