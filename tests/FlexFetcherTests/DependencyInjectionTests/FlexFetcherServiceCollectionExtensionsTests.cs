using FlexFetcher;
using FlexFetcher.DependencyInjection.Microsoft;
using FlexFetcher.Models.FlexFetcherOptions;
using FlexFetcher.Models.Queries;
using Microsoft.Extensions.DependencyInjection;
using TestData.Database;

namespace FlexFetcherTests.DependencyInjectionTests;

public class FlexFetcherServiceCollectionExtensionsTests
{
    [Test]
    public void FlexOptions()
    {
        string alias1 = "Test1";
        string alias2 = "Test2";

        // FilterOptions and Singleton
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingletonFlexOptions<FlexFilterOptions<PeopleEntity>>(options =>
        {
            options.Field(entity => entity.Name).Map(alias1);
        });
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var filterOptions1 = serviceProvider.GetRequiredService<FlexFilterOptions<PeopleEntity>>();
        filterOptions1.Build();
        Assert.That(filterOptions1.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(filterOptions1.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias1));

        serviceCollection = new ServiceCollection();
        serviceCollection.AddSingletonFlexOptions<FlexFilterOptions<PeopleEntity, PeopleModel>>(options =>
        {
            options.Field(entity => entity.Name).Map(model => model.FirstName);
        });
        serviceProvider = serviceCollection.BuildServiceProvider();
        var filterOptions2 = serviceProvider.GetRequiredService<FlexFilterOptions<PeopleEntity, PeopleModel>>();
        filterOptions2.Build();
        Assert.That(filterOptions2.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(filterOptions2.FieldBuilders.First().Aliases.First(), Is.EqualTo("FirstName"));

        // SorterOptions and Transient
        serviceCollection = new ServiceCollection();
        serviceCollection.AddTransientFlexOptions<FlexSorterOptions<PeopleEntity>>(options =>
        {
            options.Field(entity => entity.Name).Map(alias2);
        });
        serviceProvider = serviceCollection.BuildServiceProvider();
        var sorterOptions1 = serviceProvider.GetRequiredService<FlexSorterOptions<PeopleEntity>>();
        sorterOptions1.Build();
        Assert.That(sorterOptions1.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(sorterOptions1.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias2));

        serviceCollection = new ServiceCollection();
        serviceCollection.AddTransientFlexOptions<FlexSorterOptions<PeopleEntity, PeopleModel>>(options =>
        {
            options.Field(entity => entity.Name).Map(model => model.FirstName);
        });
        serviceProvider = serviceCollection.BuildServiceProvider();
        var sorterOptions2 = serviceProvider.GetRequiredService<FlexSorterOptions<PeopleEntity, PeopleModel>>();
        sorterOptions2.Build();
        Assert.That(sorterOptions2.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(sorterOptions2.FieldBuilders.First().Aliases.First(), Is.EqualTo("FirstName"));

        // FetcherOptions and Scoped
        serviceCollection = new ServiceCollection();
        serviceCollection.AddScopedFlexOptions<FlexFetcherOptions<PeopleEntity>>(options =>
        {
            options.Field(entity => entity.Name).Map(alias1);
        });
        serviceProvider = serviceCollection.BuildServiceProvider();
        var fetcherOptions1 = serviceProvider.GetRequiredService<FlexFetcherOptions<PeopleEntity>>();
        fetcherOptions1.Build();
        Assert.That(fetcherOptions1.FilterOptions.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(fetcherOptions1.FilterOptions.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias1));

        serviceCollection = new ServiceCollection();
        serviceCollection.AddScopedFlexOptions<FlexFetcherOptions<PeopleEntity, PeopleModel>>(options =>
        {
            options.Field(entity => entity.Name).Map(model => model.FirstName);
        });
        serviceProvider = serviceCollection.BuildServiceProvider();
        var fetcherOptions2 = serviceProvider.GetRequiredService<FlexFetcherOptions<PeopleEntity, PeopleModel>>();
        fetcherOptions2.Build();
        Assert.That(fetcherOptions2.SorterOptions.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(fetcherOptions2.SorterOptions.FieldBuilders.First().Aliases.First(), Is.EqualTo("FirstName"));
    }

    [Test]
    public void FlexOptionsWithNested()
    {
        string alias1 = "Test1";
        string alias2 = "Test2";

        // FilterOptions and Singleton
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingletonFlexOptions<FlexFilterOptions<AddressEntity>>(options =>
        {
            options.Field(entity => entity.City).Map(alias2);
        });
        serviceCollection.AddSingleton<FlexFilter<AddressEntity>>();
        serviceCollection.AddSingletonFlexOptions<FlexFilterOptions<PeopleEntity>>((provider, options) =>
        {
            options.AddNestedFlexFilter(provider.GetRequiredService<FlexFilter<AddressEntity>>());
            options.Field(entity => entity.Name).Map(alias1);
        });
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var peopleFilterOptions1 = serviceProvider.GetRequiredService<FlexFilterOptions<PeopleEntity>>();
        peopleFilterOptions1.Build();
        var addressFilterOptions1 = serviceProvider.GetRequiredService<FlexFilterOptions<AddressEntity>>();
        addressFilterOptions1.Build();
        Assert.That(peopleFilterOptions1.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(peopleFilterOptions1.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias1));
        Assert.That(peopleFilterOptions1.NestedFlexFilters.Count, Is.EqualTo(1));
        var addressFilter1 = (FlexFilter<AddressEntity>)peopleFilterOptions1.NestedFlexFilters.First();
        Assert.That(addressFilter1.Options.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias2));

        serviceCollection = new ServiceCollection();
        serviceCollection.AddSingletonFlexOptions<FlexFilterOptions<AddressEntity>>(options =>
        {
            options.Field(entity => entity.City).Map(alias2);
        });
        serviceCollection.AddSingleton<FlexFilter<AddressEntity>>();
        serviceCollection.AddSingletonFlexOptions<FlexFilterOptions<PeopleEntity, PeopleModel>>((provider, options) =>
        {
            options.AddNestedFlexFilter(provider.GetRequiredService<FlexFilter<AddressEntity>>());
            options.Field(entity => entity.Name).Map(model => model.FirstName);
        });
        serviceProvider = serviceCollection.BuildServiceProvider();
        var peopleFilterOptions2 = serviceProvider.GetRequiredService<FlexFilterOptions<PeopleEntity, PeopleModel>>();
        peopleFilterOptions2.Build();
        var addressFilterOptions2 = serviceProvider.GetRequiredService<FlexFilterOptions<AddressEntity>>();
        addressFilterOptions2.Build();
        Assert.That(peopleFilterOptions2.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(peopleFilterOptions2.FieldBuilders.First().Aliases.First(), Is.EqualTo("FirstName"));
        Assert.That(peopleFilterOptions1.NestedFlexFilters.Count, Is.EqualTo(1));
        var addressFilter2 = (FlexFilter<AddressEntity>)peopleFilterOptions1.NestedFlexFilters.First();
        Assert.That(addressFilter2.Options.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias2));

        // SorterOptions and Transient
        serviceCollection = new ServiceCollection();
        serviceCollection.AddTransientFlexOptions<FlexSorterOptions<AddressEntity>>(options =>
        {
            options.Field(entity => entity.City).Map(alias2);
        });
        serviceCollection.AddTransient<FlexSorter<AddressEntity>>();
        serviceCollection.AddTransientFlexOptions<FlexSorterOptions<PeopleEntity>>((provider, options) =>
        {
            options.AddNestedFlexSorter(provider.GetRequiredService<FlexSorter<AddressEntity>>());
            options.Field(entity => entity.Name).Map(alias1);
        });
        serviceCollection.AddTransient<FlexSorter<PeopleEntity>>();
        serviceProvider = serviceCollection.BuildServiceProvider();
        var peopleSorter1 = serviceProvider.GetRequiredService<FlexSorter<PeopleEntity>>();
        peopleSorter1.SortData(new List<PeopleEntity>(), new DataSorters
        {
            Sorters = new List<DataSorter>
            {
                new DataSorter
                {
                    Field = "Address." + alias2,
                    Direction = DataSorterDirection.Asc
                }
            }
        });
        var peopleSorterOptions1 = peopleSorter1.Options;
        Assert.That(peopleSorterOptions1.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(peopleSorterOptions1.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias1));
        Assert.That(peopleSorterOptions1.NestedFlexSorters.Count, Is.EqualTo(1));
        var addressSorter1 = (FlexSorter<AddressEntity>)peopleSorterOptions1.NestedFlexSorters.First();
        Assert.That(addressSorter1.Options.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias2));

        // FetcherOptions and Scoped
        serviceCollection = new ServiceCollection();
        serviceCollection.AddScopedFlexOptions<FlexFetcherOptions<AddressEntity>>(options =>
        {
            options.Field(entity => entity.City).Map(alias2);
        });
        serviceCollection.AddScoped<FlexFetcher<AddressEntity>>();
        serviceCollection.AddScopedFlexOptions<FlexFetcherOptions<PeopleEntity>>((provider, options) =>
        {
            options.AddNestedFlexFetcher(provider.GetRequiredService<FlexFetcher<AddressEntity>>());
            options.Field(entity => entity.Name).Map(alias1);
        });
        serviceCollection.AddScoped<FlexFetcher<PeopleEntity>>();
        serviceProvider = serviceCollection.BuildServiceProvider();
        var fetcherOptions1 = serviceProvider.GetRequiredService<FlexFetcherOptions<PeopleEntity>>();
        fetcherOptions1.Build();
        Assert.That(fetcherOptions1.FilterOptions.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(fetcherOptions1.FilterOptions.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias1));
        var peopleFetcher1 = serviceProvider.GetRequiredService<FlexFetcher<PeopleEntity>>();
        peopleFetcher1.FetchData(new List<PeopleEntity>(), null, new DataSorters
        {
            Sorters = new List<DataSorter>
            {
                new DataSorter
                {
                    Field = "Address." + alias2,
                    Direction = DataSorterDirection.Asc
                }
            }
        }, null);
        var peopleFetcherOptions1 = peopleFetcher1.Options;
        Assert.That(peopleFetcherOptions1.FilterOptions.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(peopleFetcherOptions1.FilterOptions.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias1));
        Assert.That(peopleFetcherOptions1.SorterOptions.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(peopleFetcherOptions1.SorterOptions.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias1));
        Assert.That(peopleFetcherOptions1.FilterOptions.NestedFlexFilters.Count, Is.EqualTo(1));
        Assert.That(peopleFetcherOptions1.SorterOptions.NestedFlexSorters.Count, Is.EqualTo(1));
        var nestedAddressFilter1 = (FlexFilter<AddressEntity>)peopleFetcherOptions1.FilterOptions.NestedFlexFilters.First();
        Assert.That(nestedAddressFilter1.Options.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias2));
        var nestedAddressSorter1 = (FlexSorter<AddressEntity>)peopleFetcherOptions1.SorterOptions.NestedFlexSorters.First();
        Assert.That(nestedAddressSorter1.Options.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias2));
    }

    private class PeopleModel
    {
        public string FirstName { get; set; } = null!;
    }
}