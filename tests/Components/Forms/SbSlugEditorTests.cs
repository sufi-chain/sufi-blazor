using System.Globalization;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using SufiChain.SufiBlazor.Components.Forms;
using SufiChain.SufiBlazor.Localization;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Forms;

file class SlugEditorStringLocalizer : IStringLocalizer<SufiBlazorResource>
{
    public LocalizedString this[string name] => new(name, name);

    public LocalizedString this[string name, params object[] arguments] =>
        new(name, string.Format(CultureInfo.InvariantCulture, name, arguments));

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
        Array.Empty<LocalizedString>();
}

public class SbSlugEditorTests : BunitContext
{
    public SbSlugEditorTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new SlugEditorStringLocalizer());
    }

    [Fact]
    public void RapidInputReportsCompleteNormalizedBrowserValue()
    {
        var reportedValues = new List<string?>();
        var cut = Render<SbSlugEditor>(parameters => parameters
            .Add(p => p.ShowGenerateButton, false)
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(
                this,
                value => reportedValues.Add(value))));

        var input = cut.Find(".sb-slug-editor__input");
        input.Input("A");
        input.Input("Ar");
        input.Input("Ari");
        input.Input("Aria Test");

        Assert.Equal(["a", "ar", "ari", "aria-test"], reportedValues);
        Assert.Equal("aria-test", input.GetAttribute("value"));
    }
}
