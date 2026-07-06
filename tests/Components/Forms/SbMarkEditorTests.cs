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

file class MarkEditorStubStringLocalizer : IStringLocalizer<SufiBlazorResource>
{
    public LocalizedString this[string name] => new(name, name);
    public LocalizedString this[string name, params object[] arguments] => new(name, string.Format(CultureInfo.InvariantCulture, name, arguments));
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => Array.Empty<LocalizedString>();
}

public class SbMarkEditorTests : BunitContext
{
    public SbMarkEditorTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new MarkEditorStubStringLocalizer());
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void JsonMode_ReportsInvalidJson()
    {
        var cut = Render<SbMarkEditor>(p => p
            .Add(x => x.Mode, SbMarkEditorMode.Json)
            .Add(x => x.Value, "{ invalid"));

        var editor = cut.Instance;
        Assert.False(editor.IsJsonValid);
    }

    [Fact]
    public void JsonMode_AcceptsValidJson()
    {
        var cut = Render<SbMarkEditor>(p => p
            .Add(x => x.Mode, SbMarkEditorMode.Json)
            .Add(x => x.Value, "{ \"ok\": true }"));

        var editor = cut.Instance;
        Assert.True(editor.IsJsonValid);
    }

    [Fact]
    public void MarkdownMode_DoesNotValidateJson()
    {
        var cut = Render<SbMarkEditor>(p => p
            .Add(x => x.Mode, SbMarkEditorMode.Markdown)
            .Add(x => x.Value, "{ invalid"));

        var editor = cut.Instance;
        Assert.True(editor.IsJsonValid);
    }
}
