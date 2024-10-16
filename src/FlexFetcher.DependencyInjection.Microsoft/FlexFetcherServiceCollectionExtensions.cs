using System.Diagnostics.CodeAnalysis;
using FlexFetcher.Models.FlexFetcherOptions;
using Microsoft.Extensions.DependencyInjection;

namespace FlexFetcher.DependencyInjection.Microsoft;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class FlexFetcherServiceCollectionExtensions
{
    public static IServiceCollection AddSingletonFlexOptions<TFlexOptions>(this IServiceCollection services, Action<TFlexOptions> configure)
        where TFlexOptions : class, IFlexOptions
    {
        services.AddSingleton(provider =>
        {
            var instance = ActivatorUtilities.CreateInstance<TFlexOptions>(provider);

            configure(instance);

            return instance;
        });

        return services;
    }

    public static IServiceCollection AddSingletonFlexOptions<TFlexOptions>(this IServiceCollection services,
        Action<IServiceProvider, TFlexOptions> configure) where TFlexOptions : class, IFlexOptions
    {
        services.AddSingleton(provider =>
        {
            var instance = ActivatorUtilities.CreateInstance<TFlexOptions>(provider);

            configure(provider, instance);

            return instance;
        });

        return services;
    }

    public static IServiceCollection AddTransientFlexOptions<TFlexOptions>(this IServiceCollection services, Action<TFlexOptions> configure)
        where TFlexOptions : class, IFlexOptions
    {
        services.AddTransient(provider =>
        {
            var instance = ActivatorUtilities.CreateInstance<TFlexOptions>(provider);

            configure(instance);

            return instance;
        });

        return services;
    }

    public static IServiceCollection AddTransientFlexOptions<TFlexOptions>(this IServiceCollection services,
        Action<IServiceProvider, TFlexOptions> configure) where TFlexOptions : class, IFlexOptions
    {
        services.AddTransient(provider =>
        {
            var instance = ActivatorUtilities.CreateInstance<TFlexOptions>(provider);

            configure(provider, instance);

            return instance;
        });

        return services;
    }

    public static IServiceCollection AddScopedFlexOptions<TFlexOptions>(this IServiceCollection services, Action<TFlexOptions> configure)
        where TFlexOptions : class, IFlexOptions
    {
        services.AddScoped(provider =>
        {
            var instance = ActivatorUtilities.CreateInstance<TFlexOptions>(provider);

            configure(instance);

            return instance;
        });

        return services;
    }

    public static IServiceCollection AddScopedFlexOptions<TFlexOptions>(this IServiceCollection services,
        Action<IServiceProvider, TFlexOptions> configure) where TFlexOptions : class, IFlexOptions
    {
        services.AddScoped(provider =>
        {
            var instance = ActivatorUtilities.CreateInstance<TFlexOptions>(provider);

            configure(provider, instance);

            return instance;
        });

        return services;
    }
}