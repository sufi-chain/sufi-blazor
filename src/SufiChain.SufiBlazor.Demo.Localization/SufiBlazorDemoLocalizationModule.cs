using Microsoft.Extensions.DependencyInjection;
using SufiChain.SufiBlazor.Demo.Localization;
using SufiChain.SufiAbp.UI;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace SufiChain.SufiBlazor.Demo;

[DependsOn(typeof(SufiAbpUiDomainSharedModule))]
public class SufiBlazorDemoLocalizationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<SufiBlazorDemoLocalizationModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<SufiBlazorDemoResource>("en")
                .AddBaseTypes(typeof(SufiChain.SufiAbp.UI.Localization.SufiAbpFrameworkResource))
                .AddVirtualJson("/Localization/SufiBlazorDemo");
        });
    }
}
