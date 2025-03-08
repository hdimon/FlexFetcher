using FlexFetcher.Models.Queries;
using FlexFetcher;
using FlexFetcher.Models.FlexFetcherOptions;
using FlexFetcher.Serialization.NewtonsoftJson;
using FlexFetcher.Serialization.SystemTextJson;
using FlexFetcherTests.Stubs.Database;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using TestData.Database;

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
        var filter = new DataFilter
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
        var filter2 = JsonConvert.DeserializeObject<DataFilter>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People, filter2);
        Assert.That(result2.Count(), Is.EqualTo(5));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilter>(json3, SystemTextJsonHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People, filter3);
        Assert.That(result3.Count(), Is.EqualTo(5));
    }

    [Test]
    public void ValueObjectNameContainsTest()
    {
        var filter = new DataFilter
        {
            Filters = new List<DataFilter>
            {
                new DataFilter
                {
                    Field = "PeopleName",
                    Operator = DataFilterOperator.Contains,
                    Value = "an"
                }
            }
        };

        var options = new FlexFilterOptions<PeopleEntity>();
        options.Field(x => x.PeopleName).CastTo<string>();
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
    public void AddressStreetContainsTest()
    {
        var filter = new DataFilter
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
        var filter2 = JsonConvert.DeserializeObject<DataFilter>(json2, NewtonsoftHelper.GetSerializerSettings());
        var result2 = flexFilter.FilterData(_ctx.People.Include(p => p.Address), filter2);
        Assert.That(result2.Count(), Is.EqualTo(1));

        var json3 = System.Text.Json.JsonSerializer.Serialize(filter, SystemTextJsonHelper.GetSerializerSettings());
        var filter3 = System.Text.Json.JsonSerializer.Deserialize<DataFilter>(json3, SystemTextJsonHelper.GetSerializerSettings());
        var result3 = flexFilter.FilterData(_ctx.People.Include(p => p.Address), filter3);
        Assert.That(result3.Count(), Is.EqualTo(1));
    }
}