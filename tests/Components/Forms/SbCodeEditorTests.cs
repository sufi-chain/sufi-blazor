using System.Globalization;
using Bunit;
using Bunit.JSInterop;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using SufiChain.SufiBlazor.Components.Forms;
using SufiChain.SufiBlazor.Contracts.Editors;
using SufiChain.SufiBlazor.Localization;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Forms;

file class CodeEditorStubStringLocalizer : IStringLocalizer<SufiBlazorResource>
{
    public LocalizedString this[string name] => new(name, name);
    public LocalizedString this[string name, params object[] arguments] => new(name, string.Format(CultureInfo.InvariantCulture, name, arguments));
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => Array.Empty<LocalizedString>();
}

public class SbCodeEditorTests : BunitContext
{
    public SbCodeEditorTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new CodeEditorStubStringLocalizer());
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void HidesToolbarByDefault()
    {
        var cut = Render<SbCodeEditor>();

        Assert.Empty(cut.FindAll(".sb-editor__toolbar--code"));
        Assert.NotNull(cut.Find(".sb-code-editor__content"));
    }

    [Fact]
    public void ShowsToolbarWhenHideToolbarFalse()
    {
        var cut = Render<SbCodeEditor>(p => p.Add(x => x.HideToolbar, false));

        Assert.NotNull(cut.Find(".sb-editor__toolbar--code"));
    }

    [Fact]
    public void AppliesReadonlyClass()
    {
        var cut = Render<SbCodeEditor>(p => p.Add(x => x.ReadOnly, true));

        Assert.Contains("sb-code-editor--readonly", cut.Find(".sb-code-editor").ClassList);
    }

    [Fact]
    public void SetsRtlDirection()
    {
        var cut = Render<SbCodeEditor>(p => p.Add(x => x.RightToLeft, true));

        Assert.Equal("rtl", cut.Find(".sb-code-editor").GetAttribute("dir"));
    }
}
