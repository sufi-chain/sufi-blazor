using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Bunit;
using Bunit.JSInterop;
using SufiChain.SufiBlazor.Components.Forms;
using SufiChain.SufiBlazor.Contracts.Editors;
using SufiChain.SufiBlazor.Localization;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Forms;

file class StubStringLocalizer : IStringLocalizer<SufiBlazorResource>
{
    public LocalizedString this[string name] => new(name, name);
    public LocalizedString this[string name, params object[] arguments] => new(name, string.Format(CultureInfo.InvariantCulture, name, arguments));
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => Array.Empty<LocalizedString>();
}

public class SbRichTextEditorTests : BunitContext
{
    public SbRichTextEditorTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new StubStringLocalizer());
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<SbRichTextEditor> RenderEditor(
        Action<ComponentParameterCollectionBuilder<SbRichTextEditor>>? configure = null)
    {
        return Render<SbRichTextEditor>(p =>
        {
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersEditorStructure()
    {
        // Arrange & Act
        var cut = RenderEditor();

        // Assert
        var container = cut.Find(".sb-editor");
        Assert.NotNull(container);
        Assert.NotNull(cut.Find(".sb-editor__content"));
    }

    [Fact]
    public void RendersToolbarByDefault()
    {
        // Arrange & Act
        var cut = RenderEditor();

        // Assert
        var toolbar = cut.Find(".sb-editor__toolbar");
        Assert.NotNull(toolbar);
        Assert.Equal("toolbar", toolbar.GetAttribute("role"));
        Assert.NotNull(cut.FindAll(".sb-editor__toolbar-btn"));
    }

    [Fact]
    public void HidesToolbarWhenHideToolbarTrue()
    {
        // Arrange & Act
        var cut = RenderEditor(p => p.Add(x => x.HideToolbar, true));

        // Assert
        Assert.Empty(cut.FindAll(".sb-editor__toolbar"));
    }

    [Fact]
    public void RendersEditorContentArea()
    {
        // Arrange & Act
        var cut = RenderEditor();

        // Assert
        var content = cut.Find(".sb-editor__content");
        Assert.NotNull(content);
        Assert.Equal("textbox", content.GetAttribute("role"));
        Assert.Equal("true", content.GetAttribute("aria-multiline"));
    }

    [Fact]
    public void ShowsCharacterCountWhenEnabled()
    {
        // Arrange & Act
        var cut = RenderEditor(p => p.Add(x => x.ShowCharacterCount, true));

        // Assert
        var footer = cut.Find(".sb-editor__footer");
        Assert.NotNull(footer);
        Assert.Contains("0 characters", cut.Markup);
    }

    [Fact]
    public void ShowsWordCountWhenEnabled()
    {
        // Arrange & Act
        var cut = RenderEditor(p => p.Add(x => x.ShowWordCount, true));

        // Assert
        var footer = cut.Find(".sb-editor__footer");
        Assert.NotNull(footer);
        Assert.Contains("0 words", cut.Markup);
    }

    [Fact]
    public void DoesNotRenderFooterWhenCountsDisabled()
    {
        // Arrange & Act
        var cut = RenderEditor();

        // Assert
        Assert.Empty(cut.FindAll(".sb-editor__footer"));
    }

    [Fact]
    public void AppliesDisabledClassWhenDisabled()
    {
        // Arrange & Act
        var cut = RenderEditor(p => p.Add(x => x.Disabled, true));

        // Assert
        var container = cut.Find(".sb-editor");
        Assert.Contains("sb-editor--disabled", container.ClassList);
    }

    [Fact]
    public void AppliesReadOnlyClassWhenReadOnly()
    {
        // Arrange & Act
        var cut = RenderEditor(p => p.Add(x => x.ReadOnly, true));

        // Assert
        var container = cut.Find(".sb-editor");
        Assert.Contains("sb-editor--readonly", container.ClassList);
    }

    [Fact]
    public void AppliesRtlClassWhenRightToLeftTrue()
    {
        // Arrange & Act
        var cut = RenderEditor(p => p.Add(x => x.RightToLeft, true));

        // Assert
        var container = cut.Find(".sb-editor");
        Assert.Contains("sb-editor--rtl", container.ClassList);
    }

    [Fact]
    public void SetsDirAttributeRtlWhenRightToLeftTrue()
    {
        // Arrange & Act
        var cut = RenderEditor(p => p.Add(x => x.RightToLeft, true));

        // Assert
        var container = cut.Find(".sb-editor");
        Assert.Equal("rtl", container.GetAttribute("dir"));
    }

    [Fact]
    public void SetsDirAttributeLtrWhenRightToLeftFalse()
    {
        // Arrange & Act
        var cut = RenderEditor(p => p.Add(x => x.RightToLeft, false));

        // Assert
        var container = cut.Find(".sb-editor");
        Assert.Equal("ltr", container.GetAttribute("dir"));
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderEditor(p => p.Add(x => x.Class, "my-editor"));

        // Assert
        var container = cut.Find(".sb-editor");
        Assert.Contains("my-editor", container.ClassList);
    }

    [Fact]
    public void AppliesHeightStyle()
    {
        // Arrange & Act
        var cut = RenderEditor(p => p.Add(x => x.Height, "400px"));

        // Assert
        var container = cut.Find(".sb-editor");
        var style = container.GetAttribute("style");
        Assert.NotNull(style);
        Assert.Contains("--sb-editor-height: 400px", style);
    }

    [Fact]
    public void UsesPlaceholderInContentAriaLabel()
    {
        // Arrange & Act
        var cut = RenderEditor(p => p.Add(x => x.Placeholder, "Type your content..."));

        // Assert
        var content = cut.Find(".sb-editor__content");
        Assert.Equal("Type your content...", content.GetAttribute("aria-label"));
    }

    [Fact]
    public void ToolbarHasAriaLabel()
    {
        // Arrange & Act
        var cut = RenderEditor(p => p.Add(x => x.ToolbarAriaLabel, "Format text"));

        // Assert
        var toolbar = cut.Find(".sb-editor__toolbar");
        Assert.Equal("Format text", toolbar.GetAttribute("aria-label"));
    }

    [Fact]
    public void ContentHasAriaReadonlyWhenReadOnly()
    {
        // Arrange & Act
        var cut = RenderEditor(p => p.Add(x => x.ReadOnly, true));

        // Assert
        var content = cut.Find(".sb-editor__content");
        Assert.Equal("true", content.GetAttribute("aria-readonly"));
    }

    [Fact]
    public void ContentHasAriaDisabledWhenDisabled()
    {
        // Arrange & Act
        var cut = RenderEditor(p => p.Add(x => x.Disabled, true));

        // Assert
        var content = cut.Find(".sb-editor__content");
        Assert.Equal("true", content.GetAttribute("aria-disabled"));
    }

    [Fact]
    public void UsesCustomToolbarItemsWhenProvided()
    {
        // Arrange
        var customItems = new List<SbEditorToolbarItem>
        {
            SbEditorToolbarItem.Bold,
            SbEditorToolbarItem.Separator,
            SbEditorToolbarItem.Italic
        };

        // Act
        var cut = RenderEditor(p => p.Add(x => x.ToolbarItems, customItems));

        // Assert - toolbar should have fewer buttons than default
        var buttons = cut.FindAll(".sb-editor__toolbar-btn");
        Assert.Equal(2, buttons.Count); // Bold and Italic
    }

    [Fact]
    public void RendersLinkAndImageDialogs()
    {
        // Arrange & Act
        var cut = RenderEditor();

        // Assert - dialogs are in the DOM (closed by default)
        Assert.NotNull(cut.FindComponent<SufiChain.SufiBlazor.Components.Overlays.SbDialog>());
    }

    [Fact]
    public void UsesCustomWordCountFormat()
    {
        // Arrange & Act
        var cut = RenderEditor(p => p
            .Add(x => x.ShowWordCount, true)
            .Add(x => x.WordCountFormat, "{0} mots"));

        // Assert
        Assert.Contains("0 mots", cut.Markup);
    }

    [Fact]
    public void UsesCustomCharacterCountFormat()
    {
        // Arrange & Act
        var cut = RenderEditor(p => p
            .Add(x => x.ShowCharacterCount, true)
            .Add(x => x.CharacterCountFormat, "{0} chars"));

        // Assert
        Assert.Contains("0 chars", cut.Markup);
    }
}
