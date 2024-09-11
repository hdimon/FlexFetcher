using FlexFetcher.Models.Queries;
using FlexFetcher;
using FlexFetcherTests.Stubs.Database;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace FlexFetcherTests.FlexFilterTests.Operators;

public class EndsWithOperatorTests
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
    public void NameEndsWithTest()
    {
        var filter = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Name",
                    Operator = DataFilterOperator.EndsWith,
                    Value = "ane"
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
    public void AddressStreetEndsWithTest()
    {
        var filter = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Address.Street",
                    Operator = DataFilterOperator.EndsWith,
                    Value = "Secondary St"
                }
            }
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People.Include(p => p.Address), filter);
        Assert.That(result.Count(), Is.EqualTo(1));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilters>(json2, NewtonsoftHelper.DeserializationSettings);
        var result2 = flexFilter.FilterData(_ctx.People.Include(p => p.Address), filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter);
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json3);
        filter3 = SystemTextJsonHelper.ProcessFilter(filter3);
        var result3 = flexFilter.FilterData(_ctx.People.Include(p => p.Address), filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));
    }
}