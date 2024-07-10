using System.Reflection;
using FlexFetcher;
using FlexFetcher.ExpressionBuilders;
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

    private class GenericFlexFilterService(FlexFilter<PeopleEntity> flexFilter)
    {
        public FlexFilter<PeopleEntity> FlexFilter { get; } = flexFilter;
    }

    private class CustomExpressionBuilder : FilterExpressionBuilder<PeopleEntity>;
}