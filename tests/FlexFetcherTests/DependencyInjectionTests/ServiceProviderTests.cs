using System.Reflection;
using FlexFetcher;
using FlexFetcher.ExpressionBuilders;
using FlexFetcher.Models.FlexFetcherOptions;
using FlexFetcherTests.Stubs.Database;
using Microsoft.Extensions.DependencyInjection;

namespace FlexFetcherTests.DependencyInjectionTests;

public class ServiceProviderTests
{
    [Test]
    public void GenericFlexFilterUsage()
    {
        // Test transient service
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<FlexFilter<PeopleEntity>>();
        serviceCollection.AddTransient<GenericFlexFilterService>();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var testInstance1 = serviceProvider.GetRequiredService<GenericFlexFilterService>();
        Assert.That(testInstance1.FlexFilter, Is.Not.Null);

        var testInstance2 = serviceProvider.GetRequiredService<GenericFlexFilterService>();
        Assert.That(testInstance2.FlexFilter, Is.Not.Null);

        Assert.That(testInstance1.FlexFilter, Is.Not.EqualTo(testInstance2.FlexFilter));

        // Test singleton service
        serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<FlexFilter<PeopleEntity>>();
        serviceCollection.AddTransient<GenericFlexFilterService>();

        serviceProvider = serviceCollection.BuildServiceProvider();

        testInstance1 = serviceProvider.GetRequiredService<GenericFlexFilterService>();
        Assert.That(testInstance1.FlexFilter, Is.Not.Null);

        testInstance2 = serviceProvider.GetRequiredService<GenericFlexFilterService>();
        Assert.That(testInstance2.FlexFilter, Is.Not.Null);

        Assert.That(testInstance1.FlexFilter, Is.EqualTo(testInstance2.FlexFilter));
    }

    [Test]
    public void GenericFlexFilterWithExpressionBuilderUsage()
    {
        // Test explicitly set expression builder
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<FilterExpressionBuilder<PeopleEntity>>();
        serviceCollection.AddTransient<FlexFilter<PeopleEntity>>();
        serviceCollection.AddTransient<GenericFlexFilterService>();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var testInstance1 = serviceProvider.GetRequiredService<GenericFlexFilterService>();
        Assert.That(testInstance1.FlexFilter, Is.Not.Null);

        var expressionBuilderObject = testInstance1.FlexFilter.GetType()
                                                   .GetProperty("ExpressionBuilder",
                                                       BindingFlags.Instance | BindingFlags.NonPublic)!
                                                   .GetValue(testInstance1.FlexFilter)!;
        var expressionBuilder = expressionBuilderObject as FilterExpressionBuilder<PeopleEntity>;
        Assert.That(expressionBuilder, Is.Not.Null);

        var customExpressionBuilder = expressionBuilderObject as CustomExpressionBuilder;
        Assert.That(customExpressionBuilder, Is.Null);

        // Test custom expression builder
        serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<FilterExpressionBuilder<PeopleEntity>, CustomExpressionBuilder>();
        serviceCollection.AddSingleton<FlexFilterOptions<PeopleEntity>>();
        serviceCollection.AddTransient<FlexFilter<PeopleEntity>>();
        serviceCollection.AddTransient<GenericFlexFilterService>();

        serviceProvider = serviceCollection.BuildServiceProvider();
        var testInstance2 = serviceProvider.GetRequiredService<GenericFlexFilterService>();
        Assert.That(testInstance2.FlexFilter, Is.Not.Null);

        expressionBuilderObject = testInstance2.FlexFilter.GetType()
                                               .GetProperty("ExpressionBuilder",
                                                   BindingFlags.Instance | BindingFlags.NonPublic)!
                                               .GetValue(testInstance2.FlexFilter)!;
        expressionBuilder = expressionBuilderObject as FilterExpressionBuilder<PeopleEntity>;
        Assert.That(expressionBuilder, Is.Not.Null);

        customExpressionBuilder = expressionBuilderObject as CustomExpressionBuilder;
        Assert.That(customExpressionBuilder, Is.Not.Null);
    }

    [Test]
    public void TwoParametersGenericFilterTest()
    {
        var options = new FlexFilterOptions<PeopleEntity>();
        options.Field(e => e.Name).Map("Test1").Map("Test2");
        options.Build();
        var modelOptions = new FlexFilterOptions<PeopleEntity, PeopleModel>();
        modelOptions.Field(e => e.Name).Map(m => m.FirstName);
        modelOptions.Build();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(options);
        serviceCollection.AddSingleton(modelOptions);
        serviceCollection.AddTransient<FlexFilter<PeopleEntity>>();
        serviceCollection.AddTransient<FlexFilter<PeopleEntity, PeopleModel>>();
        serviceCollection.AddTransient<FlexFilter<PeopleEntity, PeopleModelEmpty>>();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var filter = serviceProvider.GetRequiredService<FlexFilter<PeopleEntity>>();
        Assert.That(filter.Options.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(filter.Options.FieldBuilders.First().Aliases.Length, Is.EqualTo(2));

        var modelFilter = serviceProvider.GetRequiredService<FlexFilter<PeopleEntity, PeopleModel>>();
        Assert.That(modelFilter.Options.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(modelFilter.Options.FieldBuilders.First().Aliases.Length, Is.EqualTo(1));

        var modelFilterEmpty = serviceProvider.GetRequiredService<FlexFilter<PeopleEntity, PeopleModelEmpty>>();
        Assert.That(modelFilterEmpty.Options.FieldBuilders.Count, Is.EqualTo(0));
    }

    [Test]
    public void TwoParametersGenericSorterTest()
    {
        var options = new FlexSorterOptions<PeopleEntity>();
        options.Field(e => e.Name).Map("Test1").Map("Test2");
        options.Build();
        var modelOptions = new FlexSorterOptions<PeopleEntity, PeopleModel>();
        modelOptions.Field(e => e.Name).Map(m => m.FirstName);
        modelOptions.Build();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(options);
        serviceCollection.AddSingleton(modelOptions);
        serviceCollection.AddTransient<FlexSorter<PeopleEntity>>();
        serviceCollection.AddTransient<FlexSorter<PeopleEntity, PeopleModel>>();
        serviceCollection.AddTransient<FlexSorter<PeopleEntity, PeopleModelEmpty>>();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var sorter = serviceProvider.GetRequiredService<FlexSorter<PeopleEntity>>();
        Assert.That(sorter.Options.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(sorter.Options.FieldBuilders.First().Aliases.Length, Is.EqualTo(2));

        var modelSorter = serviceProvider.GetRequiredService<FlexSorter<PeopleEntity, PeopleModel>>();
        Assert.That(modelSorter.Options.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(modelSorter.Options.FieldBuilders.First().Aliases.Length, Is.EqualTo(1));

        var modelSorterEmpty = serviceProvider.GetRequiredService<FlexSorter<PeopleEntity, PeopleModelEmpty>>();
        Assert.That(modelSorterEmpty.Options.FieldBuilders.Count, Is.EqualTo(0));
    }

    [Test]
    public void FlexFetcherTest()
    {
        var options = new FlexFetcherOptions<PeopleEntity>();
        options.Field(e => e.Name).Map("Test1").Map("Test2");
        options.Build();
        var modelOptions = new FlexFetcherOptions<PeopleEntity, PeopleModel>();
        modelOptions.Field(e => e.Name).Map(m => m.FirstName);
        modelOptions.Build();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(options);
        serviceCollection.AddSingleton(modelOptions);
        serviceCollection.AddTransient<FlexFetcher<PeopleEntity>>();
        serviceCollection.AddTransient<FlexFetcher<PeopleEntity, PeopleModel>>();
        serviceCollection.AddTransient<FlexFetcher<PeopleEntity, PeopleModelEmpty>>();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var fetcher = serviceProvider.GetRequiredService<FlexFetcher<PeopleEntity>>();
        Assert.That(fetcher.Options.FilterOptions.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(fetcher.Options.FilterOptions.FieldBuilders.First().Aliases.Length, Is.EqualTo(2));
        Assert.That(fetcher.Options.SorterOptions.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(fetcher.Options.SorterOptions.FieldBuilders.First().Aliases.Length, Is.EqualTo(2));

        var modelSorter = serviceProvider.GetRequiredService<FlexFetcher<PeopleEntity, PeopleModel>>();
        Assert.That(modelSorter.Options.FilterOptions.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(modelSorter.Options.FilterOptions.FieldBuilders.First().Aliases.Length, Is.EqualTo(1));
        Assert.That(modelSorter.Options.SorterOptions.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(modelSorter.Options.SorterOptions.FieldBuilders.First().Aliases.Length, Is.EqualTo(1));

        var modelSorterEmpty = serviceProvider.GetRequiredService<FlexFetcher<PeopleEntity, PeopleModelEmpty>>();
        Assert.That(modelSorterEmpty.Options.FilterOptions.FieldBuilders.Count, Is.EqualTo(0));
        Assert.That(modelSorterEmpty.Options.SorterOptions.FieldBuilders.Count, Is.EqualTo(0));
    }

    [Test]
    public void FlexFetcherConstructorTest()
    {
        string alias1 = "Test1";
        string alias2 = "Test2";
        string alias3 = "Test3";
        // Parameterless constructor
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<FlexFetcher<PeopleEntity>>();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var fetcher = serviceProvider.GetRequiredService<FlexFetcher<PeopleEntity>>();
        Assert.That(fetcher, Is.Not.Null);

        // Options constructor
        serviceCollection = new ServiceCollection();
        var flexFetcherOptions = CreateFlexFetcherPeopleOptions(alias1);
        serviceCollection.AddSingleton(flexFetcherOptions);
        serviceCollection.AddTransient<FlexFetcher<PeopleEntity>>();
        serviceProvider = serviceCollection.BuildServiceProvider();
        fetcher = serviceProvider.GetRequiredService<FlexFetcher<PeopleEntity>>();
        Assert.That(fetcher, Is.Not.Null);
        Assert.That(fetcher.Options.FilterOptions.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(fetcher.Options.FilterOptions.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias1));
        Assert.That(fetcher.Options.SorterOptions.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(fetcher.Options.SorterOptions.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias1));

        // Constructor with filter
        serviceCollection = new ServiceCollection();
        var flexFilterOptions = CreateFlexFilterPeopleOptions(alias2);
        serviceCollection.AddSingleton(flexFilterOptions);
        serviceCollection.AddTransient<FlexFilter<PeopleEntity>>();
        serviceCollection.AddTransient<FlexFetcher<PeopleEntity>>();
        serviceProvider = serviceCollection.BuildServiceProvider();
        fetcher = serviceProvider.GetRequiredService<FlexFetcher<PeopleEntity>>();
        Assert.That(fetcher, Is.Not.Null);
        Assert.That(fetcher.Options.FilterOptions.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(fetcher.Options.FilterOptions.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias2));
        Assert.That(fetcher.Options.SorterOptions.FieldBuilders.Count, Is.EqualTo(0));

        // Constructor with filter and sorter
        serviceCollection = new ServiceCollection();
        flexFilterOptions = CreateFlexFilterPeopleOptions(alias2);
        var flexSorterOptions = CreateFlexSorterPeopleOptions(alias3);
        serviceCollection.AddSingleton(flexFilterOptions);
        serviceCollection.AddSingleton(flexSorterOptions);
        serviceCollection.AddTransient<FlexFilter<PeopleEntity>>();
        serviceCollection.AddTransient<FlexSorter<PeopleEntity>>();
        serviceCollection.AddTransient<FlexFetcher<PeopleEntity>>();
        serviceProvider = serviceCollection.BuildServiceProvider();
        fetcher = serviceProvider.GetRequiredService<FlexFetcher<PeopleEntity>>();
        Assert.That(fetcher, Is.Not.Null);
        Assert.That(fetcher.Options.FilterOptions.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(fetcher.Options.FilterOptions.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias2));
        Assert.That(fetcher.Options.SorterOptions.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(fetcher.Options.SorterOptions.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias3));

        // Constructor with filter, sorter and pager
        serviceCollection = new ServiceCollection();
        flexFilterOptions = CreateFlexFilterPeopleOptions(alias2);
        var flexPager = new FlexPager<PeopleEntity>();
        serviceCollection.AddSingleton(flexFilterOptions);
        serviceCollection.AddSingleton(flexSorterOptions);
        serviceCollection.AddSingleton(flexPager);
        serviceCollection.AddTransient<FlexFilter<PeopleEntity>>();
        serviceCollection.AddTransient<FlexSorter<PeopleEntity>>();
        serviceCollection.AddTransient<FlexFetcher<PeopleEntity>>();
        serviceProvider = serviceCollection.BuildServiceProvider();
        fetcher = serviceProvider.GetRequiredService<FlexFetcher<PeopleEntity>>();
        Assert.That(fetcher, Is.Not.Null);
        Assert.That(fetcher.Pager, Is.EqualTo(flexPager));
        Assert.That(fetcher.Options.FilterOptions.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(fetcher.Options.FilterOptions.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias2));
        Assert.That(fetcher.Options.SorterOptions.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(fetcher.Options.SorterOptions.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias3));
    }

    [Test]
    public void TwoParametersGenericFlexFetcherConstructorTest()
    {
        string alias1 = "Test1";
        string alias2 = "Test2";
        string alias3 = "Test3";
        // Parameterless constructor
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<FlexFetcher<PeopleEntity, PeopleModel>>();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var fetcher = serviceProvider.GetRequiredService<FlexFetcher<PeopleEntity, PeopleModel>>();
        Assert.That(fetcher, Is.Not.Null);

        // Options constructor
        serviceCollection = new ServiceCollection();
        var flexFetcherOptions = CreateTwoParametersGenericFlexFetcherPeopleOptions(alias1);
        serviceCollection.AddSingleton(flexFetcherOptions);
        serviceCollection.AddTransient<FlexFetcher<PeopleEntity, PeopleModel>>();
        serviceProvider = serviceCollection.BuildServiceProvider();
        fetcher = serviceProvider.GetRequiredService<FlexFetcher<PeopleEntity, PeopleModel>>();
        Assert.That(fetcher, Is.Not.Null);
        Assert.That(fetcher.Options.FilterOptions.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(fetcher.Options.FilterOptions.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias1));
        Assert.That(fetcher.Options.SorterOptions.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(fetcher.Options.SorterOptions.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias1));

        // Constructor with filter
        serviceCollection = new ServiceCollection();
        var flexFilterOptions = CreateTwoParametersGenericFlexFilterPeopleOptions(alias2);
        serviceCollection.AddSingleton(flexFilterOptions);
        serviceCollection.AddTransient<FlexFilter<PeopleEntity, PeopleModel>>();
        serviceCollection.AddTransient<FlexFetcher<PeopleEntity, PeopleModel>>();
        serviceProvider = serviceCollection.BuildServiceProvider();
        fetcher = serviceProvider.GetRequiredService<FlexFetcher<PeopleEntity, PeopleModel>>();
        Assert.That(fetcher, Is.Not.Null);
        Assert.That(fetcher.Options.FilterOptions.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(fetcher.Options.FilterOptions.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias2));
        Assert.That(fetcher.Options.SorterOptions.FieldBuilders.Count, Is.EqualTo(0));

        // Constructor with filter and sorter
        serviceCollection = new ServiceCollection();
        flexFilterOptions = CreateTwoParametersGenericFlexFilterPeopleOptions(alias2);
        var flexSorterOptions = CreateTwoParametersGenericFlexSorterPeopleOptions(alias3);
        serviceCollection.AddSingleton(flexFilterOptions);
        serviceCollection.AddSingleton(flexSorterOptions);
        serviceCollection.AddTransient<FlexFilter<PeopleEntity, PeopleModel>>();
        serviceCollection.AddTransient<FlexSorter<PeopleEntity, PeopleModel>>();
        serviceCollection.AddTransient<FlexFetcher<PeopleEntity, PeopleModel>>();
        serviceProvider = serviceCollection.BuildServiceProvider();
        fetcher = serviceProvider.GetRequiredService<FlexFetcher<PeopleEntity, PeopleModel>>();
        Assert.That(fetcher, Is.Not.Null);
        Assert.That(fetcher.Options.FilterOptions.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(fetcher.Options.FilterOptions.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias2));
        Assert.That(fetcher.Options.SorterOptions.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(fetcher.Options.SorterOptions.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias3));

        // Constructor with filter, sorter and pager
        serviceCollection = new ServiceCollection();
        flexFilterOptions = CreateTwoParametersGenericFlexFilterPeopleOptions(alias2);
        var flexPager = new FlexPager<PeopleEntity, PeopleModel>();
        serviceCollection.AddSingleton(flexFilterOptions);
        serviceCollection.AddSingleton(flexSorterOptions);
        serviceCollection.AddSingleton(flexPager);
        serviceCollection.AddTransient<FlexFilter<PeopleEntity, PeopleModel>>();
        serviceCollection.AddTransient<FlexSorter<PeopleEntity, PeopleModel>>();
        serviceCollection.AddTransient<FlexFetcher<PeopleEntity, PeopleModel>>();
        serviceProvider = serviceCollection.BuildServiceProvider();
        fetcher = serviceProvider.GetRequiredService<FlexFetcher<PeopleEntity, PeopleModel>>();
        Assert.That(fetcher, Is.Not.Null);
        Assert.That(fetcher.Pager, Is.EqualTo(flexPager));
        Assert.That(fetcher.Options.FilterOptions.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(fetcher.Options.FilterOptions.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias2));
        Assert.That(fetcher.Options.SorterOptions.FieldBuilders.Count, Is.EqualTo(1));
        Assert.That(fetcher.Options.SorterOptions.FieldBuilders.First().Aliases.First(), Is.EqualTo(alias3));
    }

    private FlexFetcherOptions<PeopleEntity> CreateFlexFetcherPeopleOptions(string alias1)
    {
        var flexFetcherOptions = new FlexFetcherOptions<PeopleEntity>();
        flexFetcherOptions.Field(e => e.Name).Map(alias1);
        flexFetcherOptions.Build();
        return flexFetcherOptions;
    }

    private FlexFetcherOptions<PeopleEntity, PeopleModel> CreateTwoParametersGenericFlexFetcherPeopleOptions(string alias1)
    {
        var flexFetcherOptions = new FlexFetcherOptions<PeopleEntity, PeopleModel>();
        flexFetcherOptions.Field(e => e.Name).Map(alias1);
        flexFetcherOptions.Build();
        return flexFetcherOptions;
    }

    private FlexFilterOptions<PeopleEntity> CreateFlexFilterPeopleOptions(string alias2)
    {
        var flexFilterOptions = new FlexFilterOptions<PeopleEntity>();
        flexFilterOptions.Field(e => e.Name).Map(alias2);
        flexFilterOptions.Build();
        return flexFilterOptions;
    }

    private FlexFilterOptions<PeopleEntity, PeopleModel> CreateTwoParametersGenericFlexFilterPeopleOptions(string alias2)
    {
        var flexFilterOptions = new FlexFilterOptions<PeopleEntity, PeopleModel>();
        flexFilterOptions.Field(e => e.Name).Map(alias2);
        flexFilterOptions.Build();
        return flexFilterOptions;
    }

    private FlexSorterOptions<PeopleEntity> CreateFlexSorterPeopleOptions(string alias3)
    {
        var flexSorterOptions = new FlexSorterOptions<PeopleEntity>();
        flexSorterOptions.Field(e => e.Name).Map(alias3);
        flexSorterOptions.Build();
        return flexSorterOptions;
    }

    private FlexSorterOptions<PeopleEntity, PeopleModel> CreateTwoParametersGenericFlexSorterPeopleOptions(string alias3)
    {
        var flexSorterOptions = new FlexSorterOptions<PeopleEntity, PeopleModel>();
        flexSorterOptions.Field(e => e.Name).Map(alias3);
        flexSorterOptions.Build();
        return flexSorterOptions;
    }

    private class GenericFlexFilterService(FlexFilter<PeopleEntity> flexFilter)
    {
        public FlexFilter<PeopleEntity> FlexFilter { get; } = flexFilter;
    }

    private class CustomExpressionBuilder : FilterExpressionBuilder<PeopleEntity>;

    private class PeopleModel
    {
        public string FirstName { get; set; } = null!;
    }

    private class PeopleModelEmpty;
}