using System.Text.Json;
using System.Text.Json.Serialization;
using FlexFetcher;
using FlexFetcher.Models.Queries;
using FlexFetcherTests.Stubs.Database;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FlexFetcherTests.FlexFilterTests.Operators;

public class EqualOperatorTests
{
    private TestDbContext _ctx = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
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
        var filter = new DataFilters
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
        var filter2 = JsonConvert.DeserializeObject<DataFilters>(json2, NewtonsoftHelper.DeserializationSettings);
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(5));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter);
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json3);
        filter3 = SystemTextJsonHelper.ProcessFilter(filter3);
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(5));
    }

    [Test]
    public void IntTest()
    {
        var filter = new DataFilters
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
        var filter2 = JsonConvert.DeserializeObject<DataFilters>(json2, NewtonsoftHelper.DeserializationSettings);
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter);
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json3);
        filter3 = SystemTextJsonHelper.ProcessFilter(filter3);
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));
    }

    [Test]
    public void DoubleTest()
    {
        var filter = new DataFilters
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
        var filter2 = JsonConvert.DeserializeObject<DataFilters>(json2, NewtonsoftHelper.DeserializationSettings);
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter);
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json3);
        filter3 = SystemTextJsonHelper.ProcessFilter(filter3);
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));
    }

    [Test]
    public void DoubleNullableTest()
    {
        var filter = new DataFilters
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
        var filter2 = JsonConvert.DeserializeObject<DataFilters>(json2, NewtonsoftHelper.DeserializationSettings);
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(5));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter);
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json3);
        filter3 = SystemTextJsonHelper.ProcessFilter(filter3);
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(5));
    }

    [Test]
    public void DecimalTest()
    {
        var filter = new DataFilters
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
        var filter2 = JsonConvert.DeserializeObject<DataFilters>(json2, NewtonsoftHelper.DeserializationSettings);
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter);
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json3);
        filter3 = SystemTextJsonHelper.ProcessFilter(filter3);
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));
    }

    [Test]
    public void BoolTest()
    {
        var filter = new DataFilters
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
        var filter2 = JsonConvert.DeserializeObject<DataFilters>(json2, NewtonsoftHelper.DeserializationSettings);
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(2));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter);
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json3);
        filter3 = SystemTextJsonHelper.ProcessFilter(filter3);
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(2));
    }

    [Test]
    public void DateOnlyTest()
    {
        var filter = new DataFilters
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
        var filter2 = JsonConvert.DeserializeObject<DataFilters>(json2);
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter);
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json3);
        filter3 = SystemTextJsonHelper.ProcessFilter(filter3);
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));
    }

    [Test]
    public void DateTimeTest()
    {
        var filter = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "LastLoginUtc",
                    Operator = DataFilterOperator.Equal,
                    Value = new DateTime(2024, 6, 10, 13, 20, 56)
                }
            }
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(1));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilters>(json2, NewtonsoftHelper.DeserializationSettings);
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter);
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json3);
        filter3 = SystemTextJsonHelper.ProcessFilter(filter3);
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));
    }

    [Test]
    public void DateTimeOffsetTest()
    {
        var filter = new DataFilters
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
        var filter2 = JsonConvert.DeserializeObject<DataFilters>(json2, NewtonsoftHelper.DeserializationSettings);
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter);
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json3);
        filter3 = SystemTextJsonHelper.ProcessFilter(filter3);
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));

        filter.Filters[0].Value = new DateTimeOffset(new DateTime(2024, 6, 10, 10, 20, 56), TimeSpan.FromHours(5));
        var json4 = JsonConvert.SerializeObject(filter);
        var filter4 = JsonConvert.DeserializeObject<DataFilters>(json4, NewtonsoftHelper.DeserializationSettings);
        var result4 = flexFilter.FilterData(_ctx.People, filter4);
        Assert.That(result4.Count(), Is.EqualTo(1));

        var json5 = System.Text.Json.JsonSerializer.Serialize(filter);
        var filter5 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json5);
        filter5 = SystemTextJsonHelper.ProcessFilter(filter5);
        var result5 = flexFilter.FilterData(_ctx.People, filter5);
        Assert.That(result5.Count(), Is.EqualTo(1));
    }

    [Test]
    public void TimeSpanTest()
    {
        var filter = new DataFilters
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
        var filter2 = JsonConvert.DeserializeObject<DataFilters>(json2, NewtonsoftHelper.DeserializationSettings);
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter);
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json3);
        filter3 = SystemTextJsonHelper.ProcessFilter(filter3);
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));
    }

    [Test]
    public void TimeOnlyTest()
    {
        var filter = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "WorkStart",
                    Operator = DataFilterOperator.Equal,
                    Value = new TimeOnly(8, 30, 0)
                }
            }
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(1));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilters>(json2, NewtonsoftHelper.DeserializationSettings);
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter);
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json3);
        filter3 = SystemTextJsonHelper.ProcessFilter(filter3);
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));
    }

    [Test]
    public void GuidTest()
    {
        var filter = new DataFilters
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
        var filter2 = JsonConvert.DeserializeObject<DataFilters>(json2, NewtonsoftHelper.DeserializationSettings);
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter);
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json3);
        filter3 = SystemTextJsonHelper.ProcessFilter(filter3);
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));
    }

    [Test]
    public void EnumAsIntTest()
    {
        var filter = new DataFilters
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
        var filter2 = JsonConvert.DeserializeObject<DataFilters>(jsonWithNumber, NewtonsoftHelper.DeserializationSettings);
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(4));

        var jsonWithString = JsonConvert.SerializeObject(filter,
            new JsonSerializerSettings { Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() } });
        var filter3 = JsonConvert.DeserializeObject<DataFilters>(jsonWithString, NewtonsoftHelper.DeserializationSettings);
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(4));

        var jsonWithNumber2 = System.Text.Json.JsonSerializer.Serialize(filter);
        var filter4 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(jsonWithNumber2);
        filter4 = SystemTextJsonHelper.ProcessFilter(filter4);
        var result4 = flexFilter.FilterData(_ctx.People, filter4);
        Assert.That(result4.Count(), Is.EqualTo(4));

        var jsonWithString2 = System.Text.Json.JsonSerializer.Serialize(filter,
            new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } });
        var filter5 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(jsonWithString2);
        filter5 = SystemTextJsonHelper.ProcessFilter(filter5);
        var result5 = flexFilter.FilterData(_ctx.People, filter5);
        Assert.That(result5.Count(), Is.EqualTo(4));

        // Just for the sake of test
        var person = _ctx.People.Include(p => p.GenderEntity).First(p => p.Gender == Gender.Male);
        Assert.That(person.GenderEntity.Name, Is.EqualTo("Male"));
    }

    [Test]
    public void EnumAsStringTest()
    {
        var filter = new DataFilters
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
        var filter2 = JsonConvert.DeserializeObject<DataFilters>(jsonWithNumber, NewtonsoftHelper.DeserializationSettings);
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var jsonWithString = JsonConvert.SerializeObject(filter,
            new JsonSerializerSettings { Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() } });
        var filter3 = JsonConvert.DeserializeObject<DataFilters>(jsonWithString, NewtonsoftHelper.DeserializationSettings);
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));

        var jsonWithNumber2 = System.Text.Json.JsonSerializer.Serialize(filter);
        var filter4 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(jsonWithNumber2);
        filter4 = SystemTextJsonHelper.ProcessFilter(filter4);
        var result4 = flexFilter.FilterData(_ctx.People, filter4);
        Assert.That(result4.Count(), Is.EqualTo(1));

        var jsonWithString2 = System.Text.Json.JsonSerializer.Serialize(filter,
            new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } });
        var filter5 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(jsonWithString2);
        filter5 = SystemTextJsonHelper.ProcessFilter(filter5);
        var result5 = flexFilter.FilterData(_ctx.People, filter5);
        Assert.That(result5.Count(), Is.EqualTo(1));
    }
}