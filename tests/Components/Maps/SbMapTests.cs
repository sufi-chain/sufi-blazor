using System.Globalization;
using Bunit;
using Bunit.JSInterop;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using SufiChain.SufiBlazor.Components.Maps;
using SufiChain.SufiBlazor.Localization;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Maps;

file class StubStringLocalizer : IStringLocalizer<SufiBlazorResource>
{
    public LocalizedString this[string name] => new(name, name);
    public LocalizedString this[string name, params object[] arguments] =>
        new(name, string.Format(CultureInfo.InvariantCulture, name, arguments));
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => Array.Empty<LocalizedString>();
}

public class SbMapPreviewTests : BunitContext
{
    public SbMapPreviewTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new StubStringLocalizer());
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void RendersPreviewStructure()
    {
        var cut = Render<SbMapPreview>(p => p
            .Add(x => x.Latitude, 35.6892)
            .Add(x => x.Longitude, 51.3890)
            .Add(x => x.Label, "Tehran")
            .Add(x => x.Height, "160px"));

        var root = cut.Find(".sb-map-preview");
        Assert.NotNull(root);
        Assert.NotNull(cut.Find(".sb-map"));
        Assert.Contains("Tehran", cut.Markup);
        Assert.Contains("openstreetmap.org", cut.Markup);
    }

    [Fact]
    public void HidesExternalLinkWhenDisabled()
    {
        var cut = Render<SbMapPreview>(p => p
            .Add(x => x.Latitude, 1)
            .Add(x => x.Longitude, 2)
            .Add(x => x.OpenExternalOnClick, false)
            .Add(x => x.Label, (string?)null));

        Assert.Empty(cut.FindAll(".sb-map-preview__link"));
        Assert.Empty(cut.FindAll(".sb-map-preview__overlay"));
    }

    [Fact]
    public void BuildsOpenStreetMapUrl()
    {
        var url = SbGeoPosition.GetOpenStreetMapUrl(35.6892, 51.3890, 16);
        Assert.Contains("mlat=35.6892", url);
        Assert.Contains("mlon=51.3890", url);
        Assert.Contains("#map=16/", url);
    }
}

public class SbMapTests : BunitContext
{
    public SbMapTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void RendersMapContainer()
    {
        var cut = Render<SbMap>(p => p
            .Add(x => x.Height, "200px")
            .Add(x => x.Class, "demo-map"));

        var map = cut.Find(".sb-map");
        Assert.Contains("sb-map", map.ClassList);
        Assert.Contains("demo-map", map.ClassList);
        Assert.Contains("height:200px", map.GetAttribute("style") ?? "");
    }

    [Fact]
    public void AppliesReadonlyClassWhenNotInteractive()
    {
        var cut = Render<SbMap>(p => p.Add(x => x.Interactive, false));
        Assert.Contains("sb-map--readonly", cut.Find(".sb-map").ClassList);
    }
}

public class SbGeolocateButtonTests : BunitContext
{
    public SbGeolocateButtonTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new StubStringLocalizer());
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void RendersIconButton()
    {
        var cut = Render<SbGeolocateButton>();
        var button = cut.Find("button");
        Assert.Contains("sb-geolocate-button", button.ClassList);
        Assert.Contains("sb-icon-button", button.ClassList);
    }
}

public class SbPlaceSearchTests : BunitContext
{
    public SbPlaceSearchTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new StubStringLocalizer());
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void RendersAutocompleteWrapper()
    {
        var cut = Render<SbPlaceSearch>();
        Assert.NotNull(cut.Find(".sb-place-search"));
        Assert.NotNull(cut.Find(".sb-autocomplete"));
    }
}

public class SbMapPickerTests : BunitContext
{
    public SbMapPickerTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new StubStringLocalizer());
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void RendersDialogWhenOpen()
    {
        var cut = Render<SbMapPicker>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.ShowSearch, true)
            .Add(x => x.ShowGeolocate, true));

        Assert.NotNull(cut.Find(".sb-map-picker"));
        Assert.NotNull(cut.Find(".sb-place-search"));
        Assert.NotNull(cut.Find(".sb-geolocate-button"));
    }
}
