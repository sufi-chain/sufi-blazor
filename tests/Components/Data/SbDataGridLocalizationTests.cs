using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Bunit;
using Bunit.JSInterop;
using SufiChain.SufiBlazor.Components.Data;
using SufiChain.SufiBlazor.Localization;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Data;

file sealed class PersianGridLocalizer : IStringLocalizer<SufiBlazorResource>
{
    private static readonly Dictionary<string, string> Values = new(StringComparer.Ordinal)
    {
        ["NoDataAvailable"] = "داده‌ای موجود نیست",
        ["Loading"] = "در حال بارگذاری…",
        ["DataLoadFailed"] = "فهرست بارگذاری نشد.",
        ["Retry"] = "تلاش دوباره"
    };

    public LocalizedString this[string name]
    {
        get
        {
            if (Values.TryGetValue(name, out var value))
            {
                return new LocalizedString(name, value, resourceNotFound: false);
            }

            return new LocalizedString(name, name, resourceNotFound: true);
        }
    }

    public LocalizedString this[string name, params object[] arguments] =>
        new(name, string.Format(CultureInfo.InvariantCulture, this[name].Value, arguments));

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => Array.Empty<LocalizedString>();
}

public class SbDataGridLocalizationTests : BunitContext
{
    public SbDataGridLocalizationTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        JSInterop.SetupVoid("SufiBlazor.clickAway.unregister", _ => true);
        JSInterop.SetupVoid("SufiBlazor.clickAway.register", _ => true);
        JSInterop.Setup<bool>("SufiBlazor.viewport.watchCompact", _ => true).SetResult(false);
        JSInterop.SetupVoid("SufiBlazor.viewport.unwatchCompact", _ => true);
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new PersianGridLocalizer());
    }

    private static RenderFragment Columns => builder =>
    {
        builder.OpenComponent<SbColumn<string>>(0);
        builder.AddAttribute(1, "Field", "Length");
        builder.AddAttribute(2, "Title", "Name");
        builder.CloseComponent();
    };

    [Fact]
    public void PersianEmptyAndLoadingTextReplaceTheHardCodedEnglishStrings()
    {
        var loading = Render<SbDataGrid<string>>(p => p
            .Add(x => x.Items, Array.Empty<string>())
            .Add(x => x.Loading, true)
            .Add(x => x.ShowPagination, false)
            .AddChildContent(Columns));

        loading.WaitForState(() => loading.Markup.Contains("در حال بارگذاری…"), TimeSpan.FromSeconds(2));
        Assert.DoesNotContain("Loading...", loading.Markup);
        Assert.DoesNotContain("No data available", loading.Markup);
        Assert.DoesNotContain("داده‌ای موجود نیست", loading.Markup);

        var empty = Render<SbDataGrid<string>>(p => p
            .Add(x => x.Items, Array.Empty<string>())
            .Add(x => x.Loading, false)
            .Add(x => x.ShowPagination, false)
            .AddChildContent(Columns));

        Assert.Contains("داده‌ای موجود نیست", empty.Markup);
        Assert.DoesNotContain("No data available", empty.Markup);
        Assert.Equal("false", empty.Find("table.sb-datagrid__table").GetAttribute("aria-busy"));
    }

    [Fact]
    public void PersianCardEmptyStateUsesTheSameKey()
    {
        var cut = Render<SbDataGrid<string>>(p => p
            .Add(x => x.Items, Array.Empty<string>())
            .Add(x => x.Loading, false)
            .Add(x => x.CardLayout, SbDataGridCardLayout.Always)
            .Add(x => x.ShowPagination, false)
            .AddChildContent(Columns));

        Assert.Contains("داده‌ای موجود نیست", cut.Find(".sb-datagrid__cards-empty").TextContent);
        Assert.DoesNotContain("No data available", cut.Markup);
    }
}
