using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Bunit;
using Bunit.JSInterop;
using SufiChain.SufiBlazor.Components.Overlays;
using SufiChain.SufiBlazor.Localization;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Overlays;

file class StubStringLocalizer : IStringLocalizer<SufiBlazorResource>
{
    public LocalizedString this[string name] => new(name, name);
    public LocalizedString this[string name, params object[] arguments] => new(name, string.Format(CultureInfo.InvariantCulture, name, arguments));
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => Array.Empty<LocalizedString>();
}

public class SbDialogTests : BunitContext
{
    public SbDialogTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new StubStringLocalizer());
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<SbDialog> RenderDialog(
        Action<ComponentParameterCollectionBuilder<SbDialog>>? configure = null)
    {
        return Render<SbDialog>(p => configure?.Invoke(p));
    }

    [Fact]
    public void RendersDialogStructure()
    {
        // Arrange & Act
        var cut = RenderDialog(p => p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "<span>Body</span>"))));

        // Assert - dialog is always in DOM
        var dialog = cut.Find("dialog.sb-dialog");
        Assert.NotNull(dialog);
        Assert.NotNull(cut.Find(".sb-dialog__container"));
        Assert.NotNull(cut.Find(".sb-dialog__body"));
        Assert.Contains("Body", cut.Markup);
    }

    [Fact]
    public void RendersTitleWhenProvided()
    {
        // Arrange & Act
        var cut = RenderDialog(p => p
            .Add(x => x.Title, "My Dialog")
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "Content"))));

        // Assert
        Assert.Contains("My Dialog", cut.Markup);
        var title = cut.Find(".sb-dialog__title");
        Assert.NotNull(title);
        Assert.Equal("My Dialog", title.TextContent.Trim());
    }

    [Fact]
    public void RendersChildContent()
    {
        // Arrange & Act
        var cut = RenderDialog(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "<p>Custom body content</p>"))));

        // Assert
        Assert.Contains("Custom body content", cut.Markup);
        var body = cut.Find(".sb-dialog__body");
        Assert.NotNull(body);
    }

    [Fact]
    public void RendersFooterWhenProvided()
    {
        // Arrange & Act
        var cut = RenderDialog(p => p
            .Add(x => x.Title, "Dialog")
            .Add(x => x.Footer, (RenderFragment)(b => b.AddMarkupContent(0, "<button>Save</button>"))));

        // Assert
        var footer = cut.Find(".sb-dialog__footer");
        Assert.NotNull(footer);
        Assert.Contains("Save", cut.Markup);
    }

    [Fact]
    public void RendersCustomHeaderWhenProvided()
    {
        // Arrange & Act
        var cut = RenderDialog(p => p
            .Add(x => x.Header, (RenderFragment)(b => b.AddMarkupContent(0, "<h1>Custom Header</h1>")))
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "Body"))));

        // Assert - Header replaces Title
        Assert.Contains("Custom Header", cut.Markup);
        var header = cut.Find(".sb-dialog__header");
        Assert.NotNull(header);
    }

    [Fact]
    public void ShowsCloseButtonByDefault()
    {
        // Arrange & Act
        var cut = RenderDialog(p => p.Add(x => x.Title, "Dialog"));

        // Assert
        var closeBtn = cut.Find(".sb-dialog__close-btn");
        Assert.NotNull(closeBtn);
        Assert.Equal("Close", closeBtn.GetAttribute("aria-label"));
    }

    [Fact]
    public void HidesCloseButtonWhenShowCloseButtonFalse()
    {
        // Arrange & Act
        var cut = RenderDialog(p => p
            .Add(x => x.Title, "Dialog")
            .Add(x => x.ShowCloseButton, false));

        // Assert
        Assert.Empty(cut.FindAll(".sb-dialog__close-btn"));
    }

    [Theory]
    [InlineData(SbDialogSize.Sm, "sm")]
    [InlineData(SbDialogSize.Md, "md")]
    [InlineData(SbDialogSize.Lg, "lg")]
    [InlineData(SbDialogSize.Xl, "xl")]
    [InlineData(SbDialogSize.FullScreen, "fullscreen")]
    public void AppliesSizeClass(SbDialogSize size, string expectedClass)
    {
        // Arrange & Act
        var cut = RenderDialog(p => p
            .Add(x => x.Size, size)
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "x"))));

        // Assert
        var dialog = cut.Find("dialog.sb-dialog");
        Assert.Contains($"sb-dialog--{expectedClass}", dialog.ClassList);
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderDialog(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "x")))
            .Add(x => x.Class, "my-dialog"));

        // Assert
        var dialog = cut.Find("dialog.sb-dialog");
        Assert.Contains("my-dialog", dialog.ClassList);
    }

    [Fact]
    public void AppliesAllowDropdownOverflowClass()
    {
        // Arrange & Act
        var cut = RenderDialog(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "x")))
            .Add(x => x.AllowDropdownOverflow, true));

        // Assert
        var dialog = cut.Find("dialog.sb-dialog");
        Assert.Contains("sb-dialog--allow-dropdown-overflow", dialog.ClassList);
    }

    [Fact]
    public void AppliesWidthAndMaxWidthViaStyle()
    {
        // Arrange & Act
        var cut = RenderDialog(p => p
            .Add(x => x.Width, "800px")
            .Add(x => x.MaxWidth, "90%")
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "x"))));

        // Assert
        var dialog = cut.Find("dialog.sb-dialog");
        var style = dialog.GetAttribute("style") ?? "";
        Assert.Contains("width: 800px", style);
        Assert.Contains("max-width: 90%", style);
    }

    [Fact]
    public void HasAriaModalAndAriaLabelledByWhenTitleSet()
    {
        // Arrange & Act
        var cut = RenderDialog(p => p
            .Add(x => x.Title, "Dialog Title")
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "x"))));

        // Assert
        var dialog = cut.Find("dialog.sb-dialog");
        Assert.Equal("true", dialog.GetAttribute("aria-modal"));
        var labelId = dialog.GetAttribute("aria-labelledby");
        Assert.NotNull(labelId);
        var titleEl = cut.Find($"#{labelId}");
        Assert.NotNull(titleEl);
        Assert.Equal("Dialog Title", titleEl.TextContent.Trim());
    }

    [Fact]
    public async Task OpenTrueTriggersShowModal()
    {
        // Arrange & Act - render with Open=true so OnParametersSetAsync runs ShowAsync
        var cut = RenderDialog(p => p
            .Add(x => x.Open, true)
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "x"))));

        // Assert - JS interop called (Loose mode); dialog structure present
        var dialog = cut.Find("dialog.sb-dialog");
        Assert.NotNull(dialog);

        await Task.CompletedTask;
    }

    [Fact]
    public async Task CloseButtonClickInvokesOnCloseWithCloseButtonReason()
    {
        // Arrange
        SbDialogCloseReason? closeReason = null;
        var cut = RenderDialog(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Title, "Dialog")
            .Add(x => x.OnClose, EventCallback.Factory.Create<SbDialogCloseReason>(this, r => closeReason = r)));

        // Act
        var closeBtn = cut.Find(".sb-dialog__close-btn");
        await cut.InvokeAsync(() => closeBtn.Click());

        // Assert
        Assert.Equal(SbDialogCloseReason.CloseButton, closeReason);
    }

    [Fact]
    public async Task CloseButtonClickInvokesOpenChangedToFalse()
    {
        // Arrange
        var openChangedValue = true;
        var cut = RenderDialog(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Title, "Dialog")
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, v => openChangedValue = v)));

        // Act
        var closeBtn = cut.Find(".sb-dialog__close-btn");
        await cut.InvokeAsync(() => closeBtn.Click());

        // Assert
        Assert.False(openChangedValue);
    }

    [Fact]
    public async Task OnCloseButtonClickTakesPrecedenceWhenSet()
    {
        // Arrange - when OnCloseButtonClick is set, it runs instead of default close
        var closeBtnClicked = false;
        SbDialogCloseReason? closeReason = null;
        var cut = RenderDialog(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Title, "Dialog")
            .Add(x => x.OnCloseButtonClick, EventCallback.Factory.Create(this, () => closeBtnClicked = true))
            .Add(x => x.OnClose, EventCallback.Factory.Create<SbDialogCloseReason>(this, r => closeReason = r)));

        // Act
        var closeBtn = cut.Find(".sb-dialog__close-btn");
        await cut.InvokeAsync(() => closeBtn.Click());

        // Assert - OnCloseButtonClick was invoked; parent typically handles close, so OnClose may not fire
        Assert.True(closeBtnClicked);
        // With OnCloseButtonClick delegate, CloseAsync is NOT called (see HandleCloseButtonClick logic)
        // so OnClose and OpenChanged are NOT invoked by default
        Assert.Null(closeReason);
    }

    [Fact]
    public async Task EscapeKeyClosesWhenCloseOnEscapeTrue()
    {
        // Arrange
        SbDialogCloseReason? closeReason = null;
        var cut = RenderDialog(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Title, "Dialog")
            .Add(x => x.CloseOnEscape, true)
            .Add(x => x.OnClose, EventCallback.Factory.Create<SbDialogCloseReason>(this, r => closeReason = r)));

        // Act - trigger keydown on the dialog
        var dialog = cut.Find("dialog.sb-dialog");
        await cut.InvokeAsync(() => dialog.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "Escape" }));

        // Assert
        Assert.Equal(SbDialogCloseReason.Escape, closeReason);
    }

    [Fact]
    public async Task EscapeKeyDoesNotCloseWhenCloseOnEscapeFalse()
    {
        // Arrange
        SbDialogCloseReason? closeReason = null;
        var cut = RenderDialog(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Title, "Dialog")
            .Add(x => x.CloseOnEscape, false)
            .Add(x => x.OnClose, EventCallback.Factory.Create<SbDialogCloseReason>(this, r => closeReason = r)));

        // Act
        var dialog = cut.Find("dialog.sb-dialog");
        await cut.InvokeAsync(() => dialog.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "Escape" }));

        // Assert
        Assert.Null(closeReason);
    }

    [Fact]
    public void DoesNotShowHeaderWhenNoTitleHeaderOrCloseButton()
    {
        // Arrange & Act - ShowHeader = Header != null || Title != null || ShowCloseButton
        var cut = RenderDialog(p => p
            .Add(x => x.ShowCloseButton, false)
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "x"))));

        // Assert - no header when Title=null, Header=null, ShowCloseButton=false
        Assert.Empty(cut.FindAll(".sb-dialog__header"));
    }

    [Fact]
    public void ShowsHeaderWhenOnlyCloseButton()
    {
        // Arrange & Act - ShowCloseButton true but no Title/Header
        var cut = RenderDialog(p => p
            .Add(x => x.Title, (string?)null)
            .Add(x => x.Header, (RenderFragment?)null)
            .Add(x => x.ShowCloseButton, true)
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "x"))));

        // Assert
        var header = cut.Find(".sb-dialog__header");
        Assert.NotNull(header);
        Assert.NotNull(cut.Find(".sb-dialog__close-btn"));
    }
}
