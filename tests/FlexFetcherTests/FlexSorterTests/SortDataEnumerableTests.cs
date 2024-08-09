using FlexFetcher;
using FlexFetcher.Models.FlexFetcherOptions;
using FlexFetcher.Utils;
using FlexFetcherTests.Stubs;
using FlexFetcherTests.Stubs.CustomFields;
using FlexFetcherTests.Stubs.Database;

namespace FlexFetcherTests.FlexSorterTests;

public class SortDataEnumerableTests : SortDataAbstract
{
    private List<PeopleEntity> _people = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _people = InMemoryDataHelper.GetPeople();
    }

    [Test]
    public void SimpleSort()
    {
        SimpleSortTest(sorters => _people.SortData(sorters).ToList());

        var flexSorter = new FlexSorter<PeopleEntity>();
        SimpleSortTest(sorters => flexSorter.SortData(_people, sorters).ToList());
    }

    [Test]
    public void SimpleIdSort()
    {
        SimpleIdSortTest(sorters => _people.SortData(sorters).ToList());

        var flexSorter = new FlexSorter<PeopleEntity>();
        SimpleIdSortTest(sorters => flexSorter.SortData(_people, sorters).ToList());
    }

    [Test]
    public void TwoFieldsSurnameAndIdSort()
    {
        TwoFieldsSurnameAndIdSortTest(sorters => _people.SortData(sorters).ToList());

        var flexSorter = new FlexSorter<PeopleEntity>();
        TwoFieldsSurnameAndIdSortTest(sorters => flexSorter.SortData(_people, sorters).ToList());
    }

    [Test]
    public void TwoFieldsSurnameAndNameSort()
    {
        TwoFieldsSurnameAndNameSortTest(sorters => _people.SortData(sorters).ToList());

        var flexSorter = new FlexSorter<PeopleEntity>();
        TwoFieldsSurnameAndNameSortTest(sorters => flexSorter.SortData(_people, sorters).ToList());
    }

    [Test]
    public void SimpleNestedCitySort()
    {
        var nullCities = _people.Where(p => p.Address == null).ToList();
        nullCities.ForEach(p => p.Address = new AddressEntity { City = "A" });

        SimpleNestedCitySortTest(sorters => _people.SortData(sorters).ToList());

        var flexSorter = new FlexSorter<PeopleEntity>();
        SimpleNestedCitySortTest(sorters => flexSorter.SortData(_people, sorters).ToList());
    }

    [Test]
    public void SimpleSorterWithFieldAlias()
    {
        SimpleSorterWithFieldAliasTest((sorters, options) => _people.SortData(sorters, options).ToList());

        var options = new FlexSorterOptions<PeopleEntity>(); 
        options.Property(x => x.Surname).Map("SecondName");
        var flexSorter = new FlexSorter<PeopleEntity>(options);
        SimpleSorterWithFieldAliasTest((sorters, _) => flexSorter.SortData(_people, sorters).ToList());
    }

    [Test]
    public void SimpleNestedEntitySorterWithFieldAlias()
    {
        var nullCities = _people.Where(p => p.Address == null).ToList();
        nullCities.ForEach(p => p.Address = new AddressEntity { City = "A" });

        var addressOptions = new FlexSorterOptions<AddressEntity>();
        addressOptions.Property(x => x.City).Map("Town");
        var addressSorter = new FlexSorter<AddressEntity>(addressOptions);
        var peopleOptions = new FlexSorterOptions<PeopleEntity>();
        peopleOptions.AddNestedFlexSorter(addressSorter);
        peopleOptions.Property(x => x.Address).Map("Residence");
        var flexSorter = new FlexSorter<PeopleEntity>(peopleOptions);
        SimpleNestedEntitySorterWithFieldAliasTest((sorters) => flexSorter.SortData(_people, sorters).ToList());
    }

    [Test]
    public void SimpleSorterWithCustomSorter()
    {
        var flexSorter = new SimplePeopleSorterWithCustomSorter();
        SimpleSorterWithCustomSorterTest(sorters => flexSorter.SortData(_people, sorters).ToList());
    }

    [Test]
    public void SimpleSorterWithCustomSorterWithAlias()
    {
        var flexSorterCustom = new SimplePeopleSorterWithCustomSorter();
        SimpleSorterWithCustomSorterWithAliasTest(sorters => flexSorterCustom.SortData(_people, sorters).ToList());

        var customField = new PeopleFullNameCustomField();
        var options = new FlexSorterOptions<PeopleEntity>();
        options.AddCustomField(customField).Map("Title");
        var flexSorter = new FlexSorter<PeopleEntity>(options);
        SimpleSorterWithCustomSorterTest(sorters => flexSorter.SortData(_people, sorters).ToList());
    }

    private class SimplePeopleSorterWithCustomSorter : FlexSorter<PeopleEntity>
    {
        public SimplePeopleSorterWithCustomSorter()
        {
            Options.AddCustomField(new PeopleFullNameCustomField()).Map("Title");
        }
    }
}