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

file class DiffEditorStubStringLocalizer : IStringLocalizer<SufiBlazorResource>
{
    public LocalizedString this[string name] => new(name, name);
    public LocalizedString this[string name, params object[] arguments] => new(name, string.Format(CultureInfo.InvariantCulture, name, arguments));
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => Array.Empty<LocalizedString>();
}

public class SbDocumentDiffEditorTests : BunitContext
{
    public SbDocumentDiffEditorTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new DiffEditorStubStringLocalizer());
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void RendersDiffSurface()
    {
        var cut = Render<SbDocumentDiffEditor>(p => p
            .Add(x => x.OriginalValue, "one")
            .Add(x => x.SuggestedValue, "two")
            .Add(x => x.Language, SbCodeLanguage.Markdown));

        Assert.NotNull(cut.Find(".sb-diff-editor__content"));
    }

    [Fact]
    public void HidesApplyDiscardByDefault()
    {
        var cut = Render<SbDocumentDiffEditor>();

        Assert.Empty(cut.FindAll(".sb-diff-editor__actions"));
    }

    [Fact]
    public void ShowsApplyDiscardWhenEnabled()
    {
        var cut = Render<SbDocumentDiffEditor>(p => p.Add(x => x.ShowApplyDiscard, true));

        Assert.NotNull(cut.Find(".sb-diff-editor__actions"));
    }
}
