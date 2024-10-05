using FlexFetcher.Models.Queries;
using FlexFetcher;
using FlexFetcherTests.Stubs.Database;
using Newtonsoft.Json;

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
        var idsArray = new int[] { 1, 3, 5, 7 };

        var filterArray = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Id",
                    Operator = DataFilterOperator.In,
                    Value = idsArray
                }
            }
        };

        AssertResults(4, filterArray);
    }

    [Test]
    public void StringInTest()
    {
        var namesArray = new string[] { "Jane", "Doe" };

        var filter = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Name",
                    Operator = DataFilterOperator.In,
                    Value = namesArray
                }
            }
        };

        AssertResults(5, filter);
    }

    [Test]
    public void DoubleInTest()
    {
        var heights = new List<double> { 170.5, 180.2, 190.3 };

        var filter = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Height",
                    Operator = DataFilterOperator.In,
                    Value = heights
                }
            }
        };

        AssertResultsWithExceptionCommaFilter(1, filter);
    }

    [Test]
    public void DoubleNullableInTest()
    {
        var weights = new List<double?> { 70.5, 80.2, null };

        var filter = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Weight",
                    Operator = DataFilterOperator.In,
                    Value = weights
                }
            }
        };

        AssertResultsWithExceptionCommaFilter(5, filter);
    }

    [Test]
    public void DecimalInTest()
    {
        var salaries = new List<decimal> { 50000.75m, 60000.2m, 70000.3m };

        var filter = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Salary",
                    Operator = DataFilterOperator.In,
                    Value = salaries
                }
            }
        };

        AssertResultsWithExceptionCommaFilter(1, filter);
    }

    [Test]
    public void DateOnlyInTest()
    {
        var dates = new List<DateOnly>
            { new DateOnly(1975, 1, 1), new DateOnly(1980, 5, 15), new DateOnly(1990, 10, 30) };

        var filter = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "BirthDate",
                    Operator = DataFilterOperator.In,
                    Value = dates
                }
            }
        };

        AssertResults(1, filter);
    }

    [Test]
    public void DateTimeInTest()
    {
        var dates = new List<DateTime>
        {
            new DateTime(2024, 6, 10, 13, 20, 56), new DateTime(2023, 1, 1, 0, 0, 0), new DateTime(2022, 12, 31, 23, 59, 59)
        };

        var filter = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "LastLoginUtc",
                    Operator = DataFilterOperator.In,
                    Value = dates
                }
            }
        };

        AssertResults(1, filter);
    }

    [Test]
    public void DateTimeOffsetInTest()
    {
        var dates = new List<DateTimeOffset>
        {
            new DateTimeOffset(new DateTime(2024, 6, 10, 10, 20, 56), TimeSpan.FromHours(3)),
            new DateTimeOffset(new DateTime(2023, 1, 1, 0, 0, 0), TimeSpan.Zero),
            new DateTimeOffset(new DateTime(2022, 12, 31, 23, 59, 59), TimeSpan.FromHours(-5))
        };

        var filter = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "LastLogin",
                    Operator = DataFilterOperator.In,
                    Value = dates
                }
            }
        };

        AssertResults(1, filter);
    }

    [Test]
    public void TimeSpanInTest()
    {
        var timeSpans = new List<TimeSpan> { new TimeSpan(8, 30, 0), new TimeSpan(9, 0, 0), new TimeSpan(7, 45, 0) };

        var filter = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "WorkHours",
                    Operator = DataFilterOperator.In,
                    Value = timeSpans
                }
            }
        };

        AssertResults(1, filter);
    }

    [Test]
    public void TimeOnlyInTest()
    {
        var timeOnlys = new List<TimeOnly> { new TimeOnly(8, 30, 10), new TimeOnly(9, 0, 0), new TimeOnly(7, 45, 0) };

        var filter = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "WorkStart",
                    Operator = DataFilterOperator.In,
                    Value = timeOnlys
                }
            }
        };

        AssertResults(1, filter);
    }

    [Test]
    public void GuidInTest()
    {
        var guids = new List<Guid?>
        {
            Guid.Parse("f3f3f3f3-f3f3-f3f3-f3f3-f3f3f3f3f3f3"),
            null,
            Guid.Parse("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1"),
            null
        };

        var filter = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "ExternalId",
                    Operator = DataFilterOperator.In,
                    Value = guids
                }
            }
        };

        AssertResults(9, filter);
    }

    [Test]
    public void EnumAsIntInTest()
    {
        var genders = new List<Gender>
        {
            Gender.Male,
            Gender.Female
        };

        var filter = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Gender",
                    Operator = DataFilterOperator.In,
                    Value = genders
                }
            }
        };

        AssertResults(9, filter);
    }

    [Test]
    public void EnumAsStringInTest()
    {
        var occupations = new List<string?>
        {
            Occupation.Teacher.ToString(),
            null,
            Occupation.Engineer.ToString(),
            null
        };

        var filter = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Occupation",
                    Operator = DataFilterOperator.In,
                    Value = occupations
                }
            }
        };

        AssertResults(9, filter);
    }

    private void AssertResults(int expectedCount, DataFilters? filter)
    {
        var flexFilter = new FlexFilter<PeopleEntity>();
        var resultJson = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(resultJson.Count(), Is.EqualTo(expectedCount));

        var json1 = JsonConvert.SerializeObject(filter, NewtonsoftHelper.GetSerializerSettings());
        var filter1 = JsonConvert.DeserializeObject<DataFilters>(json1, NewtonsoftHelper.GetSerializerSettings());
        var result1 = flexFilter.FilterData(_ctx.People, filter1);
        Assert.That(result1.Count(), Is.EqualTo(expectedCount));

        var json2 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.SerializerSettings);
        var filter2 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json2, SystemTextJsonHelper.SerializerSettings);
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(expectedCount));
    }

    private void AssertResultsWithExceptionCommaFilter(int expectedCount, DataFilters? filter)
    {
        var flexFilter = new FlexFilter<PeopleEntity>();
        var resultJson = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(resultJson.Count(), Is.EqualTo(expectedCount));

        var json1 = JsonConvert.SerializeObject(filter, NewtonsoftHelper.GetSerializerSettings());
        var filter1 = JsonConvert.DeserializeObject<DataFilters>(json1, NewtonsoftHelper.GetSerializerSettings());
        var result1 = flexFilter.FilterData(_ctx.People, filter1);
        Assert.That(result1.Count(), Is.EqualTo(expectedCount));

        var json2 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.SerializerSettings);
        var filter2 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json2, SystemTextJsonHelper.SerializerSettings);
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count, Is.EqualTo(expectedCount));
    }
}