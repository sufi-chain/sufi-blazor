using System;
using Microsoft.Extensions.DependencyInjection;
using SufiChain.SufiBlazor.Contracts.Editors;

namespace SufiChain.SufiBlazor;

/// <summary>
/// Extension methods for registering SufiBlazor services.
/// </summary>
public static class SufiBlazorServiceExtensions
{
    /// <summary>
    /// Adds SufiBlazor services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureToolbar">Optional configuration for the RTE toolbar options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSufiBlazor(
        this IServiceCollection services,
        Action<RteToolbarOptions>? configureToolbar = null)
    {
        // Microsoft localization (no ABP dependency). Host apps can override via their own resources.
        services.AddLocalization();

        // Register toolbar services
        services.AddScoped<IRteToolbarService, RteToolbarService>();
        services.AddScoped<IMdToolbarService, MdToolbarService>();

        // Configure toolbar options
        if (configureToolbar != null)
        {
            services.Configure(configureToolbar);
        }
        else
        {
            // Register empty options if no configuration provided
            services.Configure<RteToolbarOptions>(_ => { });
        }

        services.Configure<MdToolbarOptions>(_ => { });

        return services;
    }

    /// <summary>
    /// Adds a toolbar contributor to the service collection.
    /// </summary>
    /// <typeparam name="T">The contributor type implementing IRteToolbarContributor.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRteToolbarContributor<T>(this IServiceCollection services)
        where T : class, IRteToolbarContributor
    {
        // Register the contributor type
        services.AddScoped<T>();

        // Add to options
        services.Configure<RteToolbarOptions>(options =>
        {
            if (!options.Contributors.Contains(typeof(T)))
            {
                options.Contributors.Add(typeof(T));
            }
        });

        return services;
    }

    /// <summary>
    /// Adds a toolbar contributor to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="contributorType">The contributor type implementing IRteToolbarContributor.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRteToolbarContributor(
        this IServiceCollection services,
        Type contributorType)
    {
        if (!typeof(IRteToolbarContributor).IsAssignableFrom(contributorType))
        {
            throw new ArgumentException(
                $"Type {contributorType.Name} does not implement IRteToolbarContributor",
                nameof(contributorType));
        }

        // Register the contributor type
        services.AddScoped(contributorType);

        // Add to options
        services.Configure<RteToolbarOptions>(options =>
        {
            if (!options.Contributors.Contains(contributorType))
            {
                options.Contributors.Add(contributorType);
            }
        });

        return services;
    }

    /// <summary>
    /// Adds a markdown toolbar contributor to the service collection.
    /// </summary>
    public static IServiceCollection AddMdToolbarContributor<T>(this IServiceCollection services)
        where T : class, IMdToolbarContributor
    {
        services.AddScoped<T>();
        services.Configure<MdToolbarOptions>(options =>
        {
            if (!options.Contributors.Contains(typeof(T)))
            {
                options.Contributors.Add(typeof(T));
            }
        });
        return services;
    }

    /// <summary>
    /// Adds a markdown toolbar contributor to the service collection.
    /// </summary>
    public static IServiceCollection AddMdToolbarContributor(
        this IServiceCollection services,
        Type contributorType)
    {
        if (!typeof(IMdToolbarContributor).IsAssignableFrom(contributorType))
        {
            throw new ArgumentException(
                $"Type {contributorType.Name} does not implement IMdToolbarContributor",
                nameof(contributorType));
        }

        services.AddScoped(contributorType);
        services.Configure<MdToolbarOptions>(options =>
        {
            if (!options.Contributors.Contains(contributorType))
            {
                options.Contributors.Add(contributorType);
            }
        });
        return services;
    }
}
