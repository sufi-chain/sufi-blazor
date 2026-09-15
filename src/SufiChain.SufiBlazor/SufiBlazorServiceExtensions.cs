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
    public static IServiceCollection AddSufiBlazor(
        this IServiceCollection services,
        Action<SbEditorOptions>? configureToolbar = null)
    {
        services.AddLocalization();
        services.AddScoped<IEditorToolbarService, EditorToolbarService>();

        if (configureToolbar != null)
        {
            services.Configure(configureToolbar);
        }
        else
        {
            services.Configure<SbEditorOptions>(_ => { });
        }

        return services;
    }

    /// <summary>
    /// Adds a unified editor toolbar contributor.
    /// </summary>
    public static IServiceCollection AddEditorToolbarContributor<T>(this IServiceCollection services)
        where T : class, IEditorToolbarContributor
    {
        services.AddScoped<T>();
        services.Configure<SbEditorOptions>(options =>
        {
            if (!options.Contributors.Contains(typeof(T)))
            {
                options.Contributors.Add(typeof(T));
            }
        });
        return services;
    }
}
