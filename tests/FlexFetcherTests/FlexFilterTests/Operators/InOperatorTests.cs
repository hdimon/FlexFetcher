using FlexFetcher.Models.Queries;
using FlexFetcher;
using FlexFetcherTests.Stubs.Database;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlexFetcherTests.FlexFilterTests.Operators;

public class InOperatorTests
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
    public void IntInTest()
    {
        var idsJson = System.Text.Json.JsonSerializer.Serialize(new List<int> { 1, 3, 5, 7 });
        var idsStr = string.Join(",", new List<int> { 1, 3, 5, 7 });

        var filterJson = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Id",
                    Operator = DataFilterOperator.In,
                    Value = idsJson
                }
            }
        };

        var filterStr = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Id",
                    Operator = DataFilterOperator.In,
                    Value = idsStr
                }
            }
        };

        AssertResults(4, filterJson, filterStr);
    }

    [Test]
    public void StringInTest()
    {
        var namesJson = System.Text.Json.JsonSerializer.Serialize(new List<string> { "Jane", "Doe" });
        var namesStr = string.Join(",", new List<string> { "Jane", "Doe" });

        var filterJson = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Name",
                    Operator = DataFilterOperator.In,
                    Value = namesJson
                }
            }
        };

        var filterStr = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Name",
                    Operator = DataFilterOperator.In,
                    Value = namesStr
                }
            }
        };

        AssertResults(5, filterJson, filterStr);
    }

    [Test]
    public void DoubleInTest()
    {
        var heightsJson = System.Text.Json.JsonSerializer.Serialize(new List<double> { 170.5, 180.2, 190.3 });
        var heightsStrWithComma = string.Join(",", new List<double> { 170.5, 180.2, 190.3 });

        var filterJson = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Height",
                    Operator = DataFilterOperator.In,
                    Value = heightsJson
                }
            }
        };

        var filterStrWithComma = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Height",
                    Operator = DataFilterOperator.In,
                    Value = heightsStrWithComma
                }
            }
        };

        AssertResultsWithExceptionCommaFilter(1, filterJson, filterStrWithComma);
    }

    [Test]
    public void DoubleNullableInTest()
    {
        var weightsJson = System.Text.Json.JsonSerializer.Serialize(new List<double?> { 70.5, 80.2, null });
        var weightsStr = string.Join(",", new List<double?> { 70.5, 80.2, null });

        var filterJson = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Weight",
                    Operator = DataFilterOperator.In,
                    Value = weightsJson
                }
            }
        };

        var filterStr = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Weight",
                    Operator = DataFilterOperator.In,
                    Value = weightsStr
                }
            }
        };

        AssertResultsWithExceptionCommaFilter(5, filterJson, filterStr);
    }

    [Test]
    public void DecimalInTest()
    {
        var salariesJson = System.Text.Json.JsonSerializer.Serialize(new List<decimal> { 50000.75m, 60000.2m, 70000.3m });
        var salariesStr = string.Join(",", new List<decimal> { 50000.75m, 60000.2m, 70000.3m });

        var filterJson = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Salary",
                    Operator = DataFilterOperator.In,
                    Value = salariesJson
                }
            }
        };

        var filterStr = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Salary",
                    Operator = DataFilterOperator.In,
                    Value = salariesStr
                }
            }
        };

        AssertResultsWithExceptionCommaFilter(1, filterJson, filterStr);
    }

    [Test]
    public void DateOnlyInTest()
    {
        var datesJson = System.Text.Json.JsonSerializer.Serialize(new List<DateOnly>
            { new DateOnly(1975, 1, 1), new DateOnly(1980, 5, 15), new DateOnly(1990, 10, 30) });
        var datesStr = string.Join(",",
            new List<DateOnly> { new DateOnly(1975, 1, 1), new DateOnly(1980, 5, 15), new DateOnly(1990, 10, 30) });

        var filterJson = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "BirthDate",
                    Operator = DataFilterOperator.In,
                    Value = datesJson
                }
            }
        };

        var filterStr = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "BirthDate",
                    Operator = DataFilterOperator.In,
                    Value = datesStr
                }
            }
        };

        AssertResults(1, filterJson, filterStr);
    }

    [Test]
    public void DateTimeInTest()
    {
        var dateTimesJson = System.Text.Json.JsonSerializer.Serialize(new List<DateTime>
            { new DateTime(2024, 6, 10, 13, 20, 56), new DateTime(2023, 1, 1, 0, 0, 0), new DateTime(2022, 12, 31, 23, 59, 59) });
        var dateTimesStr = string.Join(",",
            new List<DateTime>
            {
                new DateTime(2024, 6, 10, 13, 20, 56), new DateTime(2023, 1, 1, 0, 0, 0), new DateTime(2022, 12, 31, 23, 59, 59)
            });

        var filterJson = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "LastLoginUtc",
                    Operator = DataFilterOperator.In,
                    Value = dateTimesJson
                }
            }
        };

        var filterStr = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "LastLoginUtc",
                    Operator = DataFilterOperator.In,
                    Value = dateTimesStr
                }
            }
        };

        AssertResults(1, filterJson, filterStr);
    }

    [Test]
    public void DateTimeOffsetInTest()
    {
        var dateTimeOffsetsJson = System.Text.Json.JsonSerializer.Serialize(new List<DateTimeOffset>
        {
            new DateTimeOffset(new DateTime(2024, 6, 10, 10, 20, 56), TimeSpan.FromHours(3)),
            new DateTimeOffset(new DateTime(2023, 1, 1, 0, 0, 0), TimeSpan.Zero),
            new DateTimeOffset(new DateTime(2022, 12, 31, 23, 59, 59), TimeSpan.FromHours(-5))
        });
        var dateTimeOffsetsStr = string.Join(",",
            new List<DateTimeOffset>
            {
                new DateTimeOffset(new DateTime(2024, 6, 10, 10, 20, 56), TimeSpan.FromHours(3)),
                new DateTimeOffset(new DateTime(2023, 1, 1, 0, 0, 0), TimeSpan.Zero),
                new DateTimeOffset(new DateTime(2022, 12, 31, 23, 59, 59), TimeSpan.FromHours(-5))
            });

        var filterJson = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "LastLogin",
                    Operator = DataFilterOperator.In,
                    Value = dateTimeOffsetsJson
                }
            }
        };

        var filterStr = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "LastLogin",
                    Operator = DataFilterOperator.In,
                    Value = dateTimeOffsetsStr
                }
            }
        };

        AssertResults(1, filterJson, filterStr);
    }

    [Test]
    public void TimeSpanInTest()
    {
        var timeSpansJson = System.Text.Json.JsonSerializer.Serialize(new List<TimeSpan>
            { new TimeSpan(8, 30, 0), new TimeSpan(9, 0, 0), new TimeSpan(7, 45, 0) });
        var timeSpansStr = string.Join(",",
            new List<TimeSpan> { new TimeSpan(8, 30, 0), new TimeSpan(9, 0, 0), new TimeSpan(7, 45, 0) });

        var filterJson = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "WorkHours",
                    Operator = DataFilterOperator.In,
                    Value = timeSpansJson
                }
            }
        };

        var filterStr = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "WorkHours",
                    Operator = DataFilterOperator.In,
                    Value = timeSpansStr
                }
            }
        };

        AssertResults(1, filterJson, filterStr);
    }

    [Test]
    public void TimeOnlyInTest()
    {
        var timeOnlyJson = System.Text.Json.JsonSerializer.Serialize(new List<TimeOnly>
            { new TimeOnly(8, 30, 0), new TimeOnly(9, 0, 0), new TimeOnly(7, 45, 0) });
        var timeOnlyStr = string.Join(",",
            new List<TimeOnly> { new TimeOnly(8, 30, 0), new TimeOnly(9, 0, 0), new TimeOnly(7, 45, 0) });

        var filterJson = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "WorkStart",
                    Operator = DataFilterOperator.In,
                    Value = timeOnlyJson
                }
            }
        };

        var filterStr = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "WorkStart",
                    Operator = DataFilterOperator.In,
                    Value = timeOnlyStr
                }
            }
        };

        AssertResults(1, filterJson, filterStr);
    }

    [Test]
    public void GuidInTest()
    {
        var guidsJson = System.Text.Json.JsonSerializer.Serialize(new List<Guid?>
        {
            Guid.Parse("f3f3f3f3-f3f3-f3f3-f3f3-f3f3f3f3f3f3"),
            null,
            Guid.Parse("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1"),
            null
        });
        var guidsStr = string.Join(",", new List<Guid?>
        {
            Guid.Parse("f3f3f3f3-f3f3-f3f3-f3f3-f3f3f3f3f3f3"),
            null,
            Guid.Parse("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1"),
            null
        });

        var filterJson = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "ExternalId",
                    Operator = DataFilterOperator.In,
                    Value = guidsJson
                }
            }
        };

        var filterStr = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "ExternalId",
                    Operator = DataFilterOperator.In,
                    Value = guidsStr
                }
            }
        };

        AssertResults(9, filterJson, filterStr);
    }

    [Test]
    public void EnumAsIntInTest()
    {
        var gendersJson = System.Text.Json.JsonSerializer.Serialize(new List<Gender>
        {
            Gender.Male,
            Gender.Female
        });
        var gendersStr = string.Join(",", new List<int>
        {
            (int)Gender.Male,
            (int)Gender.Female
        });

        var filterJson = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Gender",
                    Operator = DataFilterOperator.In,
                    Value = gendersJson
                }
            }
        };

        var filterStr = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Gender",
                    Operator = DataFilterOperator.In,
                    Value = gendersStr
                }
            }
        };

        AssertResults(9, filterJson, filterStr);
    }

    [Test]
    public void EnumAsStringInTest()
    {
        var occupationsJson = System.Text.Json.JsonSerializer.Serialize(new List<Occupation?>
        {
            Occupation.Teacher,
            null,
            Occupation.Engineer,
            null
        }, new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        });
        var occupationsStr = string.Join(",", new List<string?>
        {
            Occupation.Teacher.ToString(),
            null,
            Occupation.Engineer.ToString(),
            null
        });

        var filterJson = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Occupation",
                    Operator = DataFilterOperator.In,
                    Value = occupationsJson
                }
            }
        };

        var filterStr = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Occupation",
                    Operator = DataFilterOperator.In,
                    Value = occupationsStr
                }
            }
        };

        AssertResults(9, filterJson, filterStr);
    }

    private void AssertResults(int expectedCount, DataFilters? filterJson, DataFilters? filterStr)
    {
        var flexFilter = new FlexFilter<PeopleEntity>();
        var resultJson = flexFilter.FilterData(_ctx.People, filterJson);
        Assert.That(resultJson.Count(), Is.EqualTo(expectedCount));

        var resultStr = flexFilter.FilterData(_ctx.People, filterStr);
        Assert.That(resultStr.Count(), Is.EqualTo(expectedCount));

        var json1 = JsonConvert.SerializeObject(filterJson);
        var filter1 = JsonConvert.DeserializeObject<DataFilters>(json1, NewtonsoftHelper.DeserializationSettings);
        var result1 = flexFilter.FilterData(_ctx.People, filter1);
        Assert.That(result1.Count(), Is.EqualTo(expectedCount));

        var json2 = System.Text.Json.JsonSerializer.Serialize(filterJson);
        var filter2 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json2);
        filter2 = SystemTextJsonHelper.ProcessFilter(filter2);
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(expectedCount));

        var json3 = JsonConvert.SerializeObject(filterStr);
        var filter3 = JsonConvert.DeserializeObject<DataFilters>(json3, NewtonsoftHelper.DeserializationSettings);
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(expectedCount));

        var json4 = System.Text.Json.JsonSerializer.Serialize(filterStr);
        var filter4 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json4);
        filter4 = SystemTextJsonHelper.ProcessFilter(filter4);
        var result4 = flexFilter.FilterData(_ctx.People, filter4);
        Assert.That(result4.Count(), Is.EqualTo(expectedCount));
    }

    private void AssertResultsWithExceptionCommaFilter(int expectedCount, DataFilters? filterJson, DataFilters? filterStr)
    {
        var flexFilter = new FlexFilter<PeopleEntity>();
        var resultJson = flexFilter.FilterData(_ctx.People, filterJson);
        Assert.That(resultJson.Count(), Is.EqualTo(expectedCount));

        Assert.Throws<InvalidOperationException>(() => flexFilter.FilterData(_ctx.People, filterStr));

        var json1 = JsonConvert.SerializeObject(filterJson);
        var filter1 = JsonConvert.DeserializeObject<DataFilters>(json1, NewtonsoftHelper.DeserializationSettings);
        var result1 = flexFilter.FilterData(_ctx.People, filter1);
        Assert.That(result1.Count(), Is.EqualTo(expectedCount));

        var json2 = System.Text.Json.JsonSerializer.Serialize(filterJson);
        var filter2 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json2);
        filter2 = SystemTextJsonHelper.ProcessFilter(filter2);
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count, Is.EqualTo(expectedCount));

        var json3 = JsonConvert.SerializeObject(filterStr);
        var filter3 = JsonConvert.DeserializeObject<DataFilters>(json3, NewtonsoftHelper.DeserializationSettings);
        Assert.Throws<InvalidOperationException>(() => flexFilter.FilterData(_ctx.People, filter3));

        var json4 = System.Text.Json.JsonSerializer.Serialize(filterStr);
        var filter4 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json4);
        filter4 = SystemTextJsonHelper.ProcessFilter(filter4);
        Assert.Throws<InvalidOperationException>(() => flexFilter.FilterData(_ctx.People, filter4));
    }
}