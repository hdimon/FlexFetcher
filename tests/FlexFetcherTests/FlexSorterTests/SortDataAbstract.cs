using FlexFetcher.Models.FlexFetcherOptions;
using FlexFetcher.Models.Queries;
using FlexFetcherTests.Stubs.Database;

namespace FlexFetcherTests.FlexSorterTests;

public abstract class SortDataAbstract
{
    protected void SimpleSortTest(Func<DataSorters, List<PeopleEntity>> sorter)
    {
        var sorters = new DataSorters
        {
            Sorters = new List<DataSorter>
            {
                new DataSorter
                {
                    Field = "Surname",
                    Direction = DataSorterDirection.Asc
                }
            }
        };

        var result = sorter(sorters);
        Assert.That(result.Select(p => p.Surname).ToList(),
            Is.EquivalentTo(new List<string>
                { "Brown", "Brown", "Doe", "Doe", "Jones", "Jones", "Smith", "Smith", "Williams", "Williams" }));
    }

    protected void SimpleIdSortTest(Func<DataSorters, List<PeopleEntity>> sorter)
    {
        var sorters = new DataSorters
        {
            Sorters = new List<DataSorter>
            {
                new DataSorter
                {
                    Field = "Id",
                    Direction = DataSorterDirection.Asc
                }
            }
        };

        var result = sorter(sorters);
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }));
    }

    protected void TwoFieldsSurnameAndIdSortTest(Func<DataSorters, List<PeopleEntity>> sorter)
    {
        var sorters = new DataSorters
        {
            Sorters = new List<DataSorter>
            {
                new DataSorter
                {
                    Field = "Surname",
                    Direction = DataSorterDirection.Asc
                },
                new DataSorter
                {
                    Field = "Id",
                    Direction = DataSorterDirection.Asc
                }
            }
        };

        var result = sorter(sorters);
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 9, 10, 1, 2, 5, 6, 3, 4, 7, 8 }));
    }

    protected void TwoFieldsSurnameAndNameSortTest(Func<DataSorters, List<PeopleEntity>> sorter)
    {
        var sorters = new DataSorters
        {
            Sorters = new List<DataSorter>
            {
                new DataSorter
                {
                    Field = "Surname",
                    Direction = DataSorterDirection.Asc
                },
                new DataSorter
                {
                    Field = "Name",
                    Direction = DataSorterDirection.Asc
                }
            }
        };

        var result = sorter(sorters);
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 10, 9, 2, 1, 6, 5, 4, 3, 8, 7 }));
    }

    protected void SimpleNestedCitySortTest(Func<DataSorters, List<PeopleEntity>> sorter)
    {
        var sorters = new DataSorters
        {
            Sorters = new List<DataSorter>
            {
                new DataSorter
                {
                    Field = "Address.City",
                    Direction = DataSorterDirection.Asc
                }
            }
        };

        var result = sorter(sorters);
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 10, 3, 9, 4, 2, 1, 5, 6, 7, 8 }));
    }

    protected void SimpleSorterWithFieldAliasTest(Func<DataSorters, FlexSorterOptions<PeopleEntity>, List<PeopleEntity>> sorter)
    {
        var sorters = new DataSorters
        {
            Sorters = new List<DataSorter>
            {
                new DataSorter
                {
                    Field = "SecondName",
                    Direction = DataSorterDirection.Asc
                }
            }
        };

        var options = new FlexSorterOptions<PeopleEntity>();
        options.Property(x => x.Surname).Map("SecondName");

        var result = sorter(sorters, options);
        Assert.That(result.Select(p => p.Surname).ToList(),
            Is.EquivalentTo(new List<string>
                { "Brown", "Brown", "Doe", "Doe", "Jones", "Jones", "Smith", "Smith", "Williams", "Williams" }));
    }

    protected void SimpleNestedEntitySorterWithFieldAliasTest(Func<DataSorters, List<PeopleEntity>> sorter)
    {
        var sorters = new DataSorters
        {
            Sorters = new List<DataSorter>
            {
                new DataSorter
                {
                    Field = "Residence.Town",
                    Direction = DataSorterDirection.Asc
                }
            }
        };

        var result = sorter(sorters);
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 10, 3, 9, 4, 2, 1, 5, 6, 7, 8 }));
    }

    protected void SimpleSorterWithCustomSorterTest(Func<DataSorters, List<PeopleEntity>> sorter)
    {
        var sorters = new DataSorters
        {
            Sorters = new List<DataSorter>
            {
                new DataSorter
                {
                    Field = "FullName",
                    Direction = DataSorterDirection.Asc
                }
            }
        };

        var result = sorter(sorters);
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 10, 9, 2, 1, 6, 5, 4, 3, 8, 7 }));
    }

    protected void SimpleSorterWithCustomSorterWithAliasTest(Func<DataSorters, List<PeopleEntity>> sorter)
    {
        var sorters = new DataSorters
        {
            Sorters = new List<DataSorter>
            {
                new DataSorter
                {
                    Field = "Title",
                    Direction = DataSorterDirection.Asc
                }
            }
        };

        var result = sorter(sorters);
        Assert.That(result.Select(p => p.Id).ToList(), Is.EquivalentTo(new List<int> { 10, 9, 2, 1, 6, 5, 4, 3, 8, 7 }));
    }
}