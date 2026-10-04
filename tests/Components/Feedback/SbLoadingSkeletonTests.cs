using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Bunit;
using SufiChain.SufiBlazor.Components.Feedback;
using SufiChain.SufiBlazor.Localization;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Feedback;

file sealed class MapLocalizer : IStringLocalizer<SufiBlazorResource>
{
    private readonly IReadOnlyDictionary<string, string> _values;

    public MapLocalizer(IReadOnlyDictionary<string, string> values)
    {
        _values = values;
    }

    public LocalizedString this[string name]
    {
        get
        {
            if (_values.TryGetValue(name, out var value))
            {
                return new LocalizedString(name, value, resourceNotFound: false);
            }

            return new LocalizedString(name, name, resourceNotFound: true);
        }
    }

    public LocalizedString this[string name, params object[] arguments] =>
        new(name, string.Format(CultureInfo.InvariantCulture, this[name].Value, arguments));

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
        _values.Select(pair => new LocalizedString(pair.Key, pair.Value, resourceNotFound: false));
}

public class SbLoadingSkeletonTests : BunitContext
{
    public SbLoadingSkeletonTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new MapLocalizer(new Dictionary<string, string>
        {
            ["Loading"] = "Loading…",
            ["NoDataAvailable"] = "No data available"
        }));
    }

    [Fact]
    public void AnnouncesLoadingOnceAndHidesSkeletonFromTheAccessibilityTree()
    {
        var cut = Render<SbLoadingSkeleton>(p => p
            .Add(x => x.Preset, SbLoadingSkeletonPreset.Kpi)
            .Add(x => x.Count, 4));

        var status = cut.Find("[role='status']");
        Assert.Equal("polite", status.GetAttribute("aria-live"));
        Assert.Null(status.GetAttribute("aria-busy"));
        Assert.Equal("true", cut.Find(".sb-loading-skeleton").GetAttribute("aria-busy"));
        Assert.Equal("", status.TextContent);

        cut.WaitForState(() => cut.FindAll(".sb-loading-skeleton__label").Count == 1, TimeSpan.FromSeconds(2));
        Assert.Equal("Loading…", cut.Find(".sb-loading-skeleton__label").TextContent);
        Assert.Equal("Loading…", cut.Find("[role='status']").TextContent);
        Assert.Equal(4, cut.FindAll(".sb-skeleton").Count);
        Assert.All(cut.FindAll(".sb-skeleton"), skeleton => Assert.Equal("true", skeleton.GetAttribute("aria-hidden")));
        Assert.DoesNotContain(">0<", cut.Markup);
        Assert.DoesNotContain("No data available", cut.Markup);
    }

    [Fact]
    public void AnnounceFalseOmitsTheLiveRegion()
    {
        var cut = Render<SbLoadingSkeleton>(p => p.Add(x => x.Announce, false));

        Assert.Empty(cut.FindAll("[role='status']"));
        Assert.Equal("true", cut.Find(".sb-loading-skeleton").GetAttribute("aria-busy"));
    }

    [Fact]
    public void TablePresetDoesNotUseTheHardCodedEnglishLoadingString()
    {
        var cut = Render<SbLoadingSkeleton>(p => p.Add(x => x.Preset, SbLoadingSkeletonPreset.Table));

        cut.WaitForState(() => cut.Markup.Contains("Loading…"), TimeSpan.FromSeconds(2));
        Assert.DoesNotContain("Loading...", cut.Markup);
        Assert.Contains("sb-loading-skeleton--table", cut.Markup);
        Assert.Contains("sb-loading-skeleton__row", cut.Markup);
    }
}

public class SbLoadingSkeletonPersianTests : BunitContext
{
    public SbLoadingSkeletonPersianTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new MapLocalizer(new Dictionary<string, string>
        {
            ["Loading"] = "در حال بارگذاری…",
            ["NoDataAvailable"] = "داده‌ای موجود نیست"
        }));
    }

    [Fact]
    public void PersianLoadingTextIsThePrimaryLabel()
    {
        var cut = Render<SbLoadingSkeleton>(p => p.Add(x => x.Preset, SbLoadingSkeletonPreset.Text));

        cut.WaitForState(() => cut.FindAll(".sb-loading-skeleton__label").Count == 1, TimeSpan.FromSeconds(2));
        Assert.Contains("در حال بارگذاری…", cut.Find(".sb-loading-skeleton__label").TextContent);
        Assert.Contains("در حال بارگذاری…", cut.Find("[role='status']").TextContent);
        Assert.DoesNotContain("Loading", cut.Markup);
        Assert.Null(cut.Find(".sb-loading-skeleton").GetAttribute("role"));
        Assert.Null(cut.Find("[role='status']").GetAttribute("aria-busy"));
    }
}
