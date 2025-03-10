﻿using System.Text.Json;
using System.Text.Json.Serialization;
using FlexFetcher;
using FlexFetcher.Models.FlexFetcherOptions;
using FlexFetcher.Models.Queries;
using FlexFetcher.Serialization.NewtonsoftJson;
using FlexFetcher.Serialization.SystemTextJson;
using FlexFetcher.Serialization.SystemTextJson.Converters;
using FlexFetcherTests.Stubs.Database;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TestData;
using TestData.Database;

namespace FlexFetcherTests.FlexFilterTests.Operators;

public class EqualOperatorTests
{
    private TestDbContext _ctx = null!;
    private List<PeopleEntity> _people = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _people = InMemoryDataHelper.GetPeople();

        _ctx = new TestDbContext();
        _ctx.Database.EnsureDeleted();
        _ctx.Database.EnsureCreated();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _ctx.Database.EnsureDeleted();
        _ctx.Dispose();
    }

    [Test]
    public void StringTest()
    {
        var filter = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Name",
                    Operator = DataFilterOperator.Equal,
                    Value = "John"
                }
            }
        
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(5));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilter>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(5));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilter>(json3, SystemTextJsonHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(5));
    }

    [Test]
    public void ValueObjectStringTest()
    {
        var filter = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "PeopleName",
                    Operator = DataFilterOperator.Equal,
                    Value = "John"
                }
            }

        };

        var options = new FlexFilterOptions<PeopleEntity>();
        options.Field(x => x.PeopleName).CastTo(typeof(string));
        var flexFilter = new FlexFilter<PeopleEntity>(options);
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(5));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilter>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(5));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilter>(json3, SystemTextJsonHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(5));
    }

    [Test]
    public void IntTest()
    {
        var filter = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Age",
                    Operator = DataFilterOperator.Equal,
                    Value = 25
                }
            }
        
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(1));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilter>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilter>(json3, SystemTextJsonHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));
    }

    [Test]
    public void DoubleTest()
    {
        var filter = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Height",
                    Operator = DataFilterOperator.Equal,
                    Value = 190.5d
                }
            }
        
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(1));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilter>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilter>(json3, SystemTextJsonHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));
    }

    [Test]
    public void DoubleNullableTest()
    {
        var filter = new DataFilter
        {
            Logic = DataFilterLogic.Or,
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Weight",
                    Operator = DataFilterOperator.Equal,
                    Value = 70.5d
                },
                new DataFilter
                {
                    Field = "Weight",
                    Operator = DataFilterOperator.Equal,
                    Value = null
                }
            }
        
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(5));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilter>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(5));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilter>(json3, SystemTextJsonHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(5));
    }

    [Test]
    public void DecimalTest()
    {
        var filter = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Salary",
                    Operator = DataFilterOperator.Equal,
                    Value = 55000.5m
                }
            }
        
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(1));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilter>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilter>(json3, SystemTextJsonHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));
    }

    [Test]
    public void BoolTest()
    {
        var filter = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "IsActive",
                    Operator = DataFilterOperator.Equal,
                    Value = true
                }
            }
        
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(2));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilter>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(2));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilter>(json3, SystemTextJsonHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(2));
    }

    [Test]
    public void DateOnlyTest()
    {
        var filter = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "BirthDate",
                    Operator = DataFilterOperator.Equal,
                    Value = new DateOnly(1975, 1, 1)
                }
            }
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(1));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilter>(json2);
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilter>(json3, SystemTextJsonHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));
    }

    [Test]
    public void DateTimeUnspecifiedSqlLiteTest()
    {
        var filter = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "LastLoginUtc",
                    Operator = DataFilterOperator.Equal,
                    Value = new DateTime(2024, 6, 10, 13, 20, 56, DateTimeKind.Unspecified)
                }
            }
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(1));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilter>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilter>(json3, SystemTextJsonHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));
    }

    [Test]
    public void DateTimeUnspecifiedListTest()
    {
        var filter = new DataFilter
        {
            Logic = DataFilterLogic.And,
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "LastLoginUtc",
                    Operator = DataFilterOperator.NotEqual,
                    Value = null
                },
                new DataFilter
                {
                    Field = "LastLoginUtc",
                    Operator = DataFilterOperator.Equal,
                    Value = new DateTime(2024, 6, 10, 13, 20, 56, DateTimeKind.Unspecified)
                }
            }
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_people, filter);
        Assert.That(result.Count(), Is.EqualTo(1));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilter>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilter>(json3, SystemTextJsonHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));
    }

    [Test]
    public void DateTimeUtcSqlLiteTest()
    {
        var filter = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "LastLoginUtc",
                    Operator = DataFilterOperator.Equal,
                    Value = new DateTime(2024, 6, 10, 13, 20, 56, DateTimeKind.Utc)
                }
            }
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(1));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilter>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilter>(json3, SystemTextJsonHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));
    }

    [Test]
    public void DateTimeUtcListTest()
    {
        var filter = new DataFilter
        {
            Logic = DataFilterLogic.And,
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "LastLoginUtc",
                    Operator = DataFilterOperator.NotEqual,
                    Value = null
                },
                new DataFilter
                {
                    Field = "LastLoginUtc",
                    Operator = DataFilterOperator.Equal,
                    Value = new DateTime(2024, 6, 10, 13, 20, 56, DateTimeKind.Utc)
                }
            }
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_people, filter);
        Assert.That(result.Count(), Is.EqualTo(1));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilter>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilter>(json3, SystemTextJsonHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));
    }

    [Test]
    public void DateTimeLocalSqlLiteTest()
    {
        var filter = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "LastLoginUtc",
                    Operator = DataFilterOperator.Equal,
                    Value = new DateTime(2024, 6, 10, 13, 20, 56, DateTimeKind.Local)
                }
            }
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(1));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilter>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilter>(json3, SystemTextJsonHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));
    }

    [Test]
    public void DateTimeLocalListTest()
    {
        var filter = new DataFilter
        {
            Logic = DataFilterLogic.And,
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "LastLoginUtc",
                    Operator = DataFilterOperator.NotEqual,
                    Value = null
                },
                new DataFilter
                {
                    Field = "LastLoginUtc",
                    Operator = DataFilterOperator.Equal,
                    Value = new DateTime(2024, 6, 10, 13, 20, 56, DateTimeKind.Local)
                }
            }
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_people, filter);
        Assert.That(result.Count(), Is.EqualTo(1));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilter>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilter>(json3, SystemTextJsonHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));
    }

    [Test]
    public void DateTimeOffsetTest()
    {
        var filter = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "LastLogin",
                    Operator = DataFilterOperator.Equal,
                    Value = new DateTimeOffset(new DateTime(2024, 6, 10, 10, 20, 56), TimeSpan.FromHours(3))
                }
            }
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(1));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilter>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilter>(json3, SystemTextJsonHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));

        filter.Filters[0].Value = new DateTimeOffset(new DateTime(2024, 6, 10, 10, 20, 56), TimeSpan.FromHours(5));
        var json4 = JsonConvert.SerializeObject(filter);
        var filter4 = JsonConvert.DeserializeObject<DataFilter>(json4, NewtonsoftHelper.GetSerializerSettings());
        var result4 = flexFilter.FilterData(_ctx.People, filter4);
        Assert.That(result4.Count(), Is.EqualTo(1));

        var json5 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter5 = System.Text.Json.JsonSerializer.Deserialize<DataFilter>(json5, SystemTextJsonHelper.GetSerializerSettings());
        var result5 = flexFilter.FilterData(_ctx.People, filter5);
        Assert.That(result5.Count(), Is.EqualTo(1));
    }

    [Test]
    public void TimeSpanTest()
    {
        var filter = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "WorkHours",
                    Operator = DataFilterOperator.Equal,
                    Value = new TimeSpan(8, 30, 0)
                }
            }
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(1));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilter>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilter>(json3, SystemTextJsonHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));
    }

    [Test]
    public void TimeOnlyTest()
    {
        var filter = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "WorkStart",
                    Operator = DataFilterOperator.Equal,
                    Value = new TimeOnly(8, 30, 10)
                }
            }
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(1));

        var json2 = JsonConvert.SerializeObject(filter, NewtonsoftHelper.GetSerializerSettings());
        var filter2 = JsonConvert.DeserializeObject<DataFilter>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilter>(json3, SystemTextJsonHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));
    }

    [Test]
    public void GuidTest()
    {
        var filter = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "ExternalId",
                    Operator = DataFilterOperator.Equal,
                    Value = Guid.Parse("f3f3f3f3-f3f3-f3f3-f3f3-f3f3f3f3f3f3")
                }
            }
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(1));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilter>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilter>(json3, SystemTextJsonHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));
    }

    [Test]
    public void EnumAsIntTest()
    {
        var filter = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Gender",
                    Operator = DataFilterOperator.Equal,
                    Value = Gender.Male
                }
            }
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(4));

        var jsonWithNumber = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilter>(jsonWithNumber, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(4));

        var jsonWithString = JsonConvert.SerializeObject(filter,
            new JsonSerializerSettings { Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() } });
        var filter3 = JsonConvert.DeserializeObject<DataFilter>(jsonWithString, NewtonsoftHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(4));

        var jsonWithNumber2 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter4 = System.Text.Json.JsonSerializer.Deserialize<DataFilter>(jsonWithNumber2, SystemTextJsonHelper.GetSerializerSettings());
        var result4 = flexFilter.FilterData(_ctx.People, filter4);
        Assert.That(result4.Count(), Is.EqualTo(4));

        var jsonWithString2 = System.Text.Json.JsonSerializer.Serialize(filter,
            new JsonSerializerOptions { Converters = { new JsonStringEnumConverter(), new GenericConverter() } });
        var filter5 = System.Text.Json.JsonSerializer.Deserialize<DataFilter>(jsonWithString2, SystemTextJsonHelper.GetSerializerSettings());
        var result5 = flexFilter.FilterData(_ctx.People, filter5);
        Assert.That(result5.Count(), Is.EqualTo(4));

        // Just for the sake of test
        var person = _ctx.People.Include(p => p.GenderEntity).First(p => p.Gender == Gender.Male);
        Assert.That(person.GenderEntity.Name, Is.EqualTo("Male"));
    }

    [Test]
    public void EnumAsStringTest()
    {
        var filter = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Occupation",
                    Operator = DataFilterOperator.Equal,
                    Value = Occupation.Teacher
                }
            }
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(1));

        var jsonWithNumber = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilter>(jsonWithNumber, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var jsonWithString = JsonConvert.SerializeObject(filter,
            new JsonSerializerSettings { Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() } });
        var filter3 = JsonConvert.DeserializeObject<DataFilter>(jsonWithString, NewtonsoftHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));

        var jsonWithNumber2 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter4 = System.Text.Json.JsonSerializer.Deserialize<DataFilter>(jsonWithNumber2, SystemTextJsonHelper.GetSerializerSettings());
        var result4 = flexFilter.FilterData(_ctx.People, filter4);
        Assert.That(result4.Count(), Is.EqualTo(1));

        var jsonWithString2 = System.Text.Json.JsonSerializer.Serialize(filter,
            new JsonSerializerOptions { Converters = { new JsonStringEnumConverter(), new GenericConverter() } });
        var filter5 = System.Text.Json.JsonSerializer.Deserialize<DataFilter>(jsonWithString2, SystemTextJsonHelper.GetSerializerSettings());
        var result5 = flexFilter.FilterData(_ctx.People, filter5);
        Assert.That(result5.Count(), Is.EqualTo(1));
    }
}