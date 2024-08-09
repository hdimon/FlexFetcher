using FlexFetcher;
using FlexFetcher.Models.FlexFetcherOptions;
using FlexFetcher.Utils;
using FlexFetcherTests.Stubs.CustomFields;
using FlexFetcherTests.Stubs.Database;

namespace FlexFetcherTests.FlexSorterTests;

public class SortDataQueryableTests : SortDataAbstract
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
    public void SimpleSort()
    {
        SimpleSortTest(sorters => _ctx.People.SortData(sorters).ToList());

        var flexSorter = new FlexSorter<PeopleEntity>();
        SimpleSortTest(sorters => flexSorter.SortData(_ctx.People, sorters).ToList());
    }

    [Test]
    public void SimpleIdSort()
    {
        SimpleIdSortTest(sorters => _ctx.People.SortData(sorters).ToList());

        var flexSorter = new FlexSorter<PeopleEntity>();
        SimpleIdSortTest(sorters => flexSorter.SortData(_ctx.People, sorters).ToList());
    }

    [Test]
    public void TwoFieldsSurnameAndIdSort()
    {
        TwoFieldsSurnameAndIdSortTest(sorters => _ctx.People.SortData(sorters).ToList());

        var flexSorter = new FlexSorter<PeopleEntity>();
        TwoFieldsSurnameAndIdSortTest(sorters => flexSorter.SortData(_ctx.People, sorters).ToList());
    }

    [Test]
    public void TwoFieldsSurnameAndNameSort()
    {
        TwoFieldsSurnameAndNameSortTest(sorters => _ctx.People.SortData(sorters).ToList());

        var flexSorter = new FlexSorter<PeopleEntity>();
        TwoFieldsSurnameAndNameSortTest(sorters => flexSorter.SortData(_ctx.People, sorters).ToList());
    }

    [Test]
    public void SimpleNestedCitySort()
    {
        SimpleNestedCitySortTest(sorters => _ctx.People.SortData(sorters).ToList());

        var flexSorter = new FlexSorter<PeopleEntity>();
        SimpleNestedCitySortTest(sorters => flexSorter.SortData(_ctx.People, sorters).ToList());
    }

    [Test]
    public void SimpleSorterWithFieldAlias()
    {
        SimpleSorterWithFieldAliasTest((sorters, options) => _ctx.People.SortData(sorters, options).ToList());

        var options = new FlexSorterOptions<PeopleEntity>();
        options.Property(x => x.Surname).Map("SecondName");
        var flexSorter = new FlexSorter<PeopleEntity>(options);
        SimpleSorterWithFieldAliasTest((sorters, _) => flexSorter.SortData(_ctx.People, sorters).ToList());
    }

    [Test]
    public void SimpleNestedEntitySorterWithFieldAlias()
    {
        var addressSorterCustom = new SimpleNestedAddressSorterWithFieldAlias();
        var flexSorter = new SimpleNestedPeopleSorterWithFieldAlias(addressSorterCustom);
        SimpleNestedEntitySorterWithFieldAliasTest((sorters) => flexSorter.SortData(_ctx.People, sorters).ToList());

        // Simple way without creating a new sorter class
        var addressOptions = new FlexSorterOptions<AddressEntity>();
        addressOptions.Property(x => x.City).Map(entity => entity.City).Map("Town");
        var addressSorter = new FlexSorter<AddressEntity>(addressOptions);
        var peopleOptions = new FlexSorterOptions<PeopleEntity>();
        peopleOptions.AddNestedFlexSorter(addressSorter);
        peopleOptions.Property(x => x.Address).Map(entity => entity.Address).Map("Residence");
        var peopleSorter = new FlexSorter<PeopleEntity>(peopleOptions);
        SimpleNestedEntitySorterWithFieldAliasTest((sorters) => peopleSorter.SortData(_ctx.People, sorters).ToList());
        
        // With model
        var addressOptionsModel = new FlexSorterOptions<AddressEntity, AddressModel>();
        addressOptionsModel.Property(x => x.City).Map(model => model.Town).Map("Town");
        var addressSorterModel = new FlexSorter<AddressEntity>(addressOptionsModel);
        var peopleOptionsModel = new FlexSorterOptions<PeopleEntity, PeopleModel>();
        peopleOptionsModel.AddNestedFlexSorter(addressSorterModel);
        peopleOptionsModel.Property(x => x.Address).Map(model => model.Residence).Map("Residence");
        var peopleSorterModel = new FlexSorter<PeopleEntity>(peopleOptionsModel);
        SimpleNestedEntitySorterWithFieldAliasTest((sorters) => peopleSorterModel.SortData(_ctx.People, sorters).ToList());
    }

    private class PeopleModel
    {
        public AddressModel Residence { get; set; } = null!;
    }

    private class AddressModel
    {
        public string Town { get; set; } = null!;
    }

    private class SimpleNestedPeopleSorterWithFieldAlias : FlexSorter<PeopleEntity>
    {
        public SimpleNestedPeopleSorterWithFieldAlias(SimpleNestedAddressSorterWithFieldAlias addressSorter)
        {
            Options.AddNestedFlexSorter(addressSorter);
            Options.Property(x => x.Address).Map("Residence");
        }
    }

    private class SimpleNestedAddressSorterWithFieldAlias : FlexSorter<AddressEntity>
    {
        public SimpleNestedAddressSorterWithFieldAlias()
        {
            Options.Property(x => x.City).Map("Town");
        }
    }

    [Test]
    public void SimpleSorterWithCustomSorter()
    {
        var flexSorter = new SimplePeopleSorterWithCustomSorter();
        SimpleSorterWithCustomSorterTest(sorters => flexSorter.SortData(_ctx.People, sorters).ToList());
    }

    [Test]
    public void SimpleSorterWithCustomSorterWithAlias()
    {
        var flexSorterCustom = new SimplePeopleSorterWithCustomSorter();
        SimpleSorterWithCustomSorterWithAliasTest(sorters => flexSorterCustom.SortData(_ctx.People, sorters).ToList());

        var customField = new PeopleFullNameCustomField();
        var options = new FlexSorterOptions<PeopleEntity>();
        options.AddCustomField(customField).Map("Title");
        var flexSorter = new FlexSorter<PeopleEntity>(options);
        SimpleSorterWithCustomSorterTest(sorters => flexSorter.SortData(_ctx.People, sorters).ToList());
    }

    private class SimplePeopleSorterWithCustomSorter : FlexSorter<PeopleEntity>
    {
        public SimplePeopleSorterWithCustomSorter()
        {
            Options.AddCustomField(new PeopleFullNameCustomField()).Map("Title");
        }
    }
}