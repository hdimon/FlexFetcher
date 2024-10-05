using FlexFetcher.Models.Queries;
using FlexFetcher;
using FlexFetcherTests.Stubs.Database;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace FlexFetcherTests.FlexFilterTests.Operators;

public class ContainsOperatorTests
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
    public void NameContainsTest()
    {
        var filter = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Name",
                    Operator = DataFilterOperator.Contains,
                    Value = "an"
                }
            }
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People, filter);
        Assert.That(result.Count(), Is.EqualTo(5));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilters>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(5));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.SerializerSettings);
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json3, SystemTextJsonHelper.SerializerSettings);
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(5));
    }

    [Test]
    public void AddressStreetContainsTest()
    {
        var filter = new DataFilters
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "Address.Street",
                    Operator = DataFilterOperator.Contains,
                    Value = "Secondary"
                }
            }
        };

        var flexFilter = new FlexFilter<PeopleEntity>();
        var result = flexFilter.FilterData(_ctx.People.Include(p => p.Address), filter);
        Assert.That(result.Count(), Is.EqualTo(1));

        var json2 = JsonConvert.SerializeObject(filter);
        var filter2 = JsonConvert.DeserializeObject<DataFilters>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People.Include(p => p.Address), filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.SerializerSettings);
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilters>(json3, SystemTextJsonHelper.SerializerSettings);
        var result3 = flexFilter.FilterData(_ctx.People.Include(p => p.Address), filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));
    }
}