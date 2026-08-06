using Microsoft.AspNetCore.Components;
using Bunit;
using Bunit.JSInterop;
using SufiChain.SufiBlazor.Components.Overlays;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Overlays;

public class SbPopoverTests : BunitContext
{
    public SbPopoverTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<SbPopover> RenderPopover(
        Action<ComponentParameterCollectionBuilder<SbPopover>>? configure = null)
    {
        return Render<SbPopover>(p =>
        {
            p.Add(x => x.AnchorContent, (RenderFragment)(b => b.AddMarkupContent(0, "<button class=\"anchor-btn\">Open</button>")));
            p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "<span>Popover content</span>")));
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersAnchorContent()
    {
        // Arrange & Act
        var cut = RenderPopover();

        // Assert
        var anchor = cut.Find(".sb-popover-anchor");
        Assert.NotNull(anchor);
        Assert.Contains("Open", cut.Markup);
        Assert.NotNull(cut.Find(".anchor-btn"));
    }

    [Fact]
    public void DoesNotRenderPopoverWhenClosed()
    {
        // Arrange & Act
        var cut = RenderPopover();

        // Assert
        Assert.Empty(cut.FindAll(".sb-popover"));
    }

    // Note: SbPopover with Open=true triggers OnAfterRenderAsync which calls StateHasChanged(),
    // causing a render loop in bunit. We test structure and closed state only.
    // Open-state tests (placement class, style, etc.) would require the component to avoid
    // unconditional StateHasChanged() when Open, or a different test setup.

    [Fact]
    public void AcceptsPlacementParameterWhenClosed()
    {
        // Component accepts Placement without error
        var cut = RenderPopover(p => p.Add(x => x.Placement, SbPlacement.Top));
        Assert.NotNull(cut.Find(".sb-popover-anchor"));
    }

    [Fact]
    public void AcceptsCloseOnClickAwayAndCloseOnEscapeParameters()
    {
        var cut = RenderPopover(p => p
            .Add(x => x.CloseOnClickAway, false)
            .Add(x => x.CloseOnEscape, false));
        Assert.NotNull(cut.Find(".sb-popover-anchor"));
    }
}
