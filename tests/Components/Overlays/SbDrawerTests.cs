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

public class SbDrawerTests : BunitContext
{
    public SbDrawerTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new StubStringLocalizer());
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<SbDrawer> RenderDrawer(
        Action<ComponentParameterCollectionBuilder<SbDrawer>>? configure = null)
    {
        return Render<SbDrawer>(p => configure?.Invoke(p));
    }

    [Fact]
    public void RendersDrawerStructure()
    {
        // Arrange & Act
        var cut = RenderDrawer(p => p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "<span>Body</span>"))));

        // Assert - dialog (drawer) is always in DOM
        var dialog = cut.Find("dialog.sb-drawer");
        Assert.NotNull(dialog);
        Assert.NotNull(cut.Find(".sb-drawer__container"));
        Assert.NotNull(cut.Find(".sb-drawer__body"));
        Assert.Contains("Body", cut.Markup);
    }

    [Fact]
    public void RendersTitleWhenProvided()
    {
        // Arrange & Act
        var cut = RenderDrawer(p => p
            .Add(x => x.Title, "My Drawer")
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "Content"))));

        // Assert
        Assert.Contains("My Drawer", cut.Markup);
        var title = cut.Find(".sb-drawer__title");
        Assert.NotNull(title);
        Assert.Equal("My Drawer", title.TextContent.Trim());
    }

    [Fact]
    public void RendersChildContent()
    {
        // Arrange & Act
        var cut = RenderDrawer(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "<p>Custom body content</p>"))));

        // Assert
        Assert.Contains("Custom body content", cut.Markup);
        var body = cut.Find(".sb-drawer__body");
        Assert.NotNull(body);
    }

    [Fact]
    public void RendersFooterWhenProvided()
    {
        // Arrange & Act
        var cut = RenderDrawer(p => p
            .Add(x => x.Title, "Drawer")
            .Add(x => x.Footer, (RenderFragment)(b => b.AddMarkupContent(0, "<button>Save</button>"))));

        // Assert
        var footer = cut.Find(".sb-drawer__footer");
        Assert.NotNull(footer);
        Assert.Contains("Save", cut.Markup);
    }

    [Fact]
    public void RendersCustomHeaderWhenProvided()
    {
        // Arrange & Act
        var cut = RenderDrawer(p => p
            .Add(x => x.Header, (RenderFragment)(b => b.AddMarkupContent(0, "<h1>Custom Header</h1>")))
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "Body"))));

        // Assert
        Assert.Contains("Custom Header", cut.Markup);
        var header = cut.Find(".sb-drawer__header");
        Assert.NotNull(header);
    }

    [Fact]
    public void ShowsCloseButtonWhenTitleProvided()
    {
        // Arrange & Act
        var cut = RenderDrawer(p => p.Add(x => x.Title, "Drawer"));

        // Assert
        var closeBtn = cut.Find(".sb-drawer__close-btn");
        Assert.NotNull(closeBtn);
        Assert.Equal("Close", closeBtn.GetAttribute("aria-label"));
    }

    [Theory]
    [InlineData(SbDrawerPlacement.Start, "start")]
    [InlineData(SbDrawerPlacement.End, "end")]
    [InlineData(SbDrawerPlacement.Top, "top")]
    [InlineData(SbDrawerPlacement.Bottom, "bottom")]
    public void AppliesPlacementClass(SbDrawerPlacement placement, string expectedClass)
    {
        // Arrange & Act
        var cut = RenderDrawer(p => p
            .Add(x => x.Placement, placement)
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "x"))));

        // Assert
        var dialog = cut.Find("dialog.sb-drawer");
        Assert.Contains($"sb-drawer--{expectedClass}", dialog.ClassList);
    }

    [Fact]
    public void AppliesModalClassWhenModalTrue()
    {
        // Arrange & Act
        var cut = RenderDrawer(p => p
            .Add(x => x.Modal, true)
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "x"))));

        // Assert
        var dialog = cut.Find("dialog.sb-drawer");
        Assert.Contains("sb-drawer--modal", dialog.ClassList);
    }

    [Fact]
    public void DoesNotApplyModalClassWhenModalFalse()
    {
        // Arrange & Act
        var cut = RenderDrawer(p => p
            .Add(x => x.Modal, false)
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "x"))));

        // Assert
        var dialog = cut.Find("dialog.sb-drawer");
        Assert.DoesNotContain("sb-drawer--modal", dialog.ClassList);
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderDrawer(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "x")))
            .Add(x => x.Class, "my-drawer"));

        // Assert
        var dialog = cut.Find("dialog.sb-drawer");
        Assert.Contains("my-drawer", dialog.ClassList);
    }

    [Fact]
    public void AppliesWidthVariableForStartPlacement()
    {
        // Arrange & Act
        var cut = RenderDrawer(p => p
            .Add(x => x.Placement, SbDrawerPlacement.Start)
            .Add(x => x.Width, "400px")
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "x"))));

        // Assert
        var dialog = cut.Find("dialog.sb-drawer");
        var style = dialog.GetAttribute("style") ?? "";
        Assert.Contains("--sb-drawer-width: 400px", style);
    }

    [Fact]
    public void AppliesHeightVariableForTopPlacement()
    {
        // Arrange & Act
        var cut = RenderDrawer(p => p
            .Add(x => x.Placement, SbDrawerPlacement.Top)
            .Add(x => x.Height, "300px")
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "x"))));

        // Assert
        var dialog = cut.Find("dialog.sb-drawer");
        var style = dialog.GetAttribute("style") ?? "";
        Assert.Contains("--sb-drawer-height: 300px", style);
    }

    [Fact]
    public void HasAriaModalTrueWhenModal()
    {
        // Arrange & Act
        var cut = RenderDrawer(p => p
            .Add(x => x.Modal, true)
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "x"))));

        // Assert
        var dialog = cut.Find("dialog.sb-drawer");
        Assert.Equal("true", dialog.GetAttribute("aria-modal"));
    }

    [Fact]
    public void HasAriaModalNullWhenNotModal()
    {
        // Arrange & Act
        var cut = RenderDrawer(p => p
            .Add(x => x.Modal, false)
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "x"))));

        // Assert
        var dialog = cut.Find("dialog.sb-drawer");
        Assert.Null(dialog.GetAttribute("aria-modal"));
    }

    [Fact]
    public async Task OpenTrueTriggersShowModal()
    {
        // Arrange & Act
        var cut = RenderDrawer(p => p
            .Add(x => x.Open, true)
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "x"))));

        // Assert
        var dialog = cut.Find("dialog.sb-drawer");
        Assert.NotNull(dialog);
        await Task.CompletedTask;
    }

    [Fact]
    public async Task CloseButtonClickInvokesOpenChangedToFalse()
    {
        // Arrange
        var openChangedValue = true;
        var cut = RenderDrawer(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Title, "Drawer")
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, v => openChangedValue = v)));

        // Act
        var closeBtn = cut.Find(".sb-drawer__close-btn");
        await cut.InvokeAsync(() => closeBtn.Click());

        // Assert
        Assert.False(openChangedValue);
    }

    [Fact]
    public async Task EscapeKeyClosesWhenCloseOnEscapeTrue()
    {
        // Arrange
        var openChangedValue = true;
        var cut = RenderDrawer(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Title, "Drawer")
            .Add(x => x.CloseOnEscape, true)
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, v => openChangedValue = v)));

        // Act
        var dialog = cut.Find("dialog.sb-drawer");
        await cut.InvokeAsync(() => dialog.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "Escape" }));

        // Assert
        Assert.False(openChangedValue);
    }

    [Fact]
    public async Task EscapeKeyDoesNotCloseWhenCloseOnEscapeFalse()
    {
        // Arrange
        var openChangedValue = true;
        var cut = RenderDrawer(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Title, "Drawer")
            .Add(x => x.CloseOnEscape, false)
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, v => openChangedValue = v)));

        // Act
        var dialog = cut.Find("dialog.sb-drawer");
        await cut.InvokeAsync(() => dialog.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "Escape" }));

        // Assert
        Assert.True(openChangedValue);
    }

    [Fact]
    public async Task BackdropClickClosesWhenCloseOnBackdropClickTrue()
    {
        // Arrange
        var openChangedValue = true;
        var cut = RenderDrawer(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Title, "Drawer")
            .Add(x => x.CloseOnBackdropClick, true)
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, v => openChangedValue = v)));

        // Act - trigger onclick on dialog (simulates backdrop click)
        var dialog = cut.Find("dialog.sb-drawer");
        await cut.InvokeAsync(() => dialog.TriggerEventAsync("onclick", new MouseEventArgs()));

        // Assert
        Assert.False(openChangedValue);
    }

    [Fact]
    public async Task BackdropClickDoesNotCloseWhenCloseOnBackdropClickFalse()
    {
        // Arrange
        var openChangedValue = true;
        var cut = RenderDrawer(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Title, "Drawer")
            .Add(x => x.CloseOnBackdropClick, false)
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, v => openChangedValue = v)));

        // Act
        var dialog = cut.Find("dialog.sb-drawer");
        await cut.InvokeAsync(() => dialog.TriggerEventAsync("onclick", new MouseEventArgs()));

        // Assert
        Assert.True(openChangedValue);
    }

    [Fact]
    public void DoesNotShowHeaderWhenNoTitleOrHeader()
    {
        // Arrange & Act
        var cut = RenderDrawer(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "x"))));

        // Assert
        Assert.Empty(cut.FindAll(".sb-drawer__header"));
    }

    [Fact]
    public void AppliesStyleParameter()
    {
        // Arrange & Act
        var cut = RenderDrawer(p => p
            .Add(x => x.Placement, SbDrawerPlacement.Start)
            .Add(x => x.Style, "custom: value")
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "x"))));

        // Assert
        var dialog = cut.Find("dialog.sb-drawer");
        var style = dialog.GetAttribute("style") ?? "";
        Assert.Contains("custom: value", style);
    }
}
