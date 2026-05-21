using Microsoft.AspNetCore.Components;
using Bunit;
using SufiChain.SufiBlazor.Components.Overlays;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Overlays;

public class SbTooltipTests : BunitContext
{
    private IRenderedComponent<SbTooltip> RenderTooltip(
        Action<ComponentParameterCollectionBuilder<SbTooltip>>? configure = null)
    {
        return Render<SbTooltip>(p =>
        {
            p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "<span class=\"trigger\">Hover me</span>")));
            p.Add(x => x.Text, "Tooltip text");
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersWrapperAndChildContent()
    {
        // Arrange & Act
        var cut = RenderTooltip();

        // Assert
        var wrapper = cut.Find(".sb-tooltip-wrapper");
        Assert.NotNull(wrapper);
        Assert.Contains("Hover me", cut.Markup);
        Assert.NotNull(cut.Find(".trigger"));
    }

    [Fact]
    public void DoesNotRenderTooltipWhenNotTriggered()
    {
        // Arrange & Act
        var cut = RenderTooltip();

        // Assert
        Assert.Empty(cut.FindAll(".sb-tooltip"));
    }

    [Fact]
    public async Task RendersTextParameter()
    {
        // Arrange & Act - with Delay=0, mouseenter shows immediately
        var cut = RenderTooltip(p => p.Add(x => x.Delay, 0));
        var wrapper = cut.Find(".sb-tooltip-wrapper");

        await cut.InvokeAsync(() => wrapper.TriggerEventAsync("onmouseenter", EventArgs.Empty));
        cut.Render();

        // Assert
        var tooltip = cut.Find(".sb-tooltip");
        Assert.NotNull(tooltip);
        Assert.Contains("Tooltip text", cut.Markup);
    }

    [Fact]
    public async Task RendersContentWhenProvided()
    {
        // Arrange & Act - use Content instead of Text
        var cut = Render<SbTooltip>(p =>
        {
            p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "<span class=\"trigger\">Hover</span>")));
            p.Add(x => x.Delay, 0);
            p.Add(x => x.Content, (RenderFragment)(b => b.AddMarkupContent(0, "<strong>Rich content</strong>")));
        });
        var wrapper = cut.Find(".sb-tooltip-wrapper");

        await cut.InvokeAsync(() => wrapper.TriggerEventAsync("onmouseenter", EventArgs.Empty));
        cut.Render();

        // Assert
        Assert.Contains("Rich content", cut.Markup);
        var tooltip = cut.Find(".sb-tooltip");
        Assert.Contains("sb-tooltip--rich", tooltip.ClassList);
    }

    [Theory]
    [InlineData(SbPlacement.Top, "top")]
    [InlineData(SbPlacement.Bottom, "bottom")]
    [InlineData(SbPlacement.End, "end")]
    [InlineData(SbPlacement.Start, "start")]
    public async Task AppliesPlacementClass(SbPlacement placement, string expectedClass)
    {
        // Arrange & Act
        var cut = RenderTooltip(p => p
            .Add(x => x.Delay, 0)
            .Add(x => x.Placement, placement));
        var wrapper = cut.Find(".sb-tooltip-wrapper");

        await cut.InvokeAsync(() => wrapper.TriggerEventAsync("onmouseenter", EventArgs.Empty));
        cut.Render();

        // Assert
        var tooltip = cut.Find(".sb-tooltip");
        Assert.Contains($"sb-tooltip--{expectedClass}", tooltip.ClassList);
    }

    [Fact]
    public async Task TooltipHasRoleAndId()
    {
        // Arrange & Act
        var cut = RenderTooltip(p => p.Add(x => x.Delay, 0));
        var wrapper = cut.Find(".sb-tooltip-wrapper");
        await cut.InvokeAsync(() => wrapper.TriggerEventAsync("onmouseenter", EventArgs.Empty));
        cut.Render();

        // Assert
        var tooltip = cut.Find(".sb-tooltip");
        Assert.Equal("tooltip", tooltip.GetAttribute("role"));
        Assert.StartsWith("sb-tooltip-", tooltip.GetAttribute("id") ?? "");
    }

    [Fact]
    public async Task MouseLeaveHidesTooltipWhenHideDelayZero()
    {
        // Arrange
        var cut = RenderTooltip(p => p.Add(x => x.Delay, 0));
        var wrapper = cut.Find(".sb-tooltip-wrapper");

        await cut.InvokeAsync(() => wrapper.TriggerEventAsync("onmouseenter", EventArgs.Empty));
        cut.Render();
        Assert.NotNull(cut.Find(".sb-tooltip"));

        // Act
        await cut.InvokeAsync(() => wrapper.TriggerEventAsync("onmouseleave", EventArgs.Empty));
        cut.Render();

        // Assert
        Assert.Empty(cut.FindAll(".sb-tooltip"));
    }

    [Fact]
    public async Task FocusShowsTooltipWhenDelayZero()
    {
        // Arrange & Act
        var cut = RenderTooltip(p => p.Add(x => x.Delay, 0));
        var wrapper = cut.Find(".sb-tooltip-wrapper");

        await cut.InvokeAsync(() => wrapper.TriggerEventAsync("onfocus", EventArgs.Empty));
        cut.Render();

        // Assert
        Assert.NotNull(cut.Find(".sb-tooltip"));
    }

    [Fact]
    public async Task BlurHidesTooltipWhenHideDelayZero()
    {
        // Arrange
        var cut = RenderTooltip(p => p.Add(x => x.Delay, 0));
        var wrapper = cut.Find(".sb-tooltip-wrapper");
        await cut.InvokeAsync(() => wrapper.TriggerEventAsync("onfocus", EventArgs.Empty));
        cut.Render();
        Assert.NotNull(cut.Find(".sb-tooltip"));

        // Act
        await cut.InvokeAsync(() => wrapper.TriggerEventAsync("onblur", EventArgs.Empty));
        cut.Render();

        // Assert
        Assert.Empty(cut.FindAll(".sb-tooltip"));
    }

    [Fact]
    public void PassesAdditionalAttributesToWrapper()
    {
        // Arrange & Act
        var cut = RenderTooltip(p => p
            .Add(x => x.AdditionalAttributes, new Dictionary<string, object> { ["data-test"] = "tooltip-wrapper" }));

        // Assert
        var wrapper = cut.Find(".sb-tooltip-wrapper");
        Assert.Equal("tooltip-wrapper", wrapper.GetAttribute("data-test"));
    }
}
