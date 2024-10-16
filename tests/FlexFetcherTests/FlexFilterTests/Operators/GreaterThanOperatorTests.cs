using FlexFetcher.Models.Queries;
using FlexFetcher;
using FlexFetcher.Serialization.NewtonsoftJson;
using FlexFetcher.Serialization.SystemTextJson;
using FlexFetcherTests.Stubs.Database;
using Newtonsoft.Json;
using TestData.Database;

namespace FlexFetcherTests.FlexFilterTests.Operators;

public class GreaterThanOperatorTests
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
    public void IntTest()
    {
        var filter = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Age",
                    Operator = DataFilterOperator.GreaterThan,
                    Value = 50
                }
            }
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(3));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilters>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(3));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json3, SystemTextJsonHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(3));
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
                    Operator = DataFilterOperator.GreaterThan,
                    Value = 190.0d
                }
            }
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(2));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilters>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(2));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json3, SystemTextJsonHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(2));
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
                    Operator = DataFilterOperator.GreaterThan,
                    Value = 60000.0m
                }
            }
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(2));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilters>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(2));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json3, SystemTextJsonHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(2));
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
                    Operator = DataFilterOperator.GreaterThan,
                    Value = new DateTime(2023, 1, 1)
                }
            }
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(1));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilters>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json3, SystemTextJsonHelper.GetSerializerSettings());
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
                    Operator = DataFilterOperator.GreaterThan,
                    Value = new DateTimeOffset(new DateTime(2023, 1, 1), TimeSpan.FromHours(0))
                }
            }
        };

        // Catch InvalidOperationException because EF Core
        // cannot translate DateTimeOffset relational comparisons into SQL for SQLite.
        Assert.Throws<InvalidOperationException>(() =>
        {
            var flexFilter = new FlexFilter<PeopleEntity>();
            var result = flexFilter.FilterData(_ctx.People, filter);
            var _ = result.Count();
        });

        /*var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(2));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilters>(json2, _deserializationSettings);
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(2));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter);
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json3);
        filter3 = SystemTextJsonHelper.ProcessFilter(filter3);
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(2));*/
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
                    Operator = DataFilterOperator.GreaterThan,
                    Value = new DateOnly(1990, 1, 1)
                }
            }
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(2));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilters>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(2));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json3, SystemTextJsonHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(2));
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
                    Operator = DataFilterOperator.GreaterThan,
                    Value = new TimeOnly(8, 0)
                }
            }
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(1));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilters>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json3, SystemTextJsonHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));
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
                    Operator = DataFilterOperator.GreaterThan,
                    Value = new TimeSpan(4, 0, 0)
                }
            }
        };

        // Catch InvalidOperationException because EF Core
        // cannot translate TimeSpan relational comparisons into SQL for SQLite.
        Assert.Throws<InvalidOperationException>(() =>
        {
            var flexFilter = new FlexFilter<PeopleEntity>();
            var result = flexFilter.FilterData(_ctx.People, filter);
            var _ = result.Count();
        });

        /*var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(1));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilters>(json2, _deserializationSettings);
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter);
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json3);
        filter3 = SystemTextJsonHelper.ProcessFilter(filter3);
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));*/
    }
}