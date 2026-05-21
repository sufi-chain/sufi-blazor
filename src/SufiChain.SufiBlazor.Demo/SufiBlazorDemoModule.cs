using Microsoft.Extensions.DependencyInjection;
using SufiChain.SufiBlazor.Demo.Menus;
using SufiChain.SufiAbp.UI.Navigation;
using SufiChain.SufiAbp.UI.Routing;
using Volo.Abp.Modularity;

namespace SufiChain.SufiBlazor.Demo;

[DependsOn(typeof(SufiBlazorDemoLocalizationModule))]
public class SufiBlazorDemoModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<SufiAbpRouterOptions>(options =>
        {
            options.AdditionalAssemblies.Add(typeof(SufiBlazorDemoModule).Assembly);
        });

        Configure<SufiAbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new SufiBlazorDemoMenuContributor());
        });
    }
}
