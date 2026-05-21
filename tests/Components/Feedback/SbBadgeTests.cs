using Microsoft.AspNetCore.Components;
using Bunit;
using SufiChain.SufiBlazor.Components;
using SufiChain.SufiBlazor.Components.Feedback;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Feedback;

public class SbBadgeTests : BunitContext
{
    private IRenderedComponent<SbBadge> RenderBadge(
        Action<ComponentParameterCollectionBuilder<SbBadge>>? configure = null)
    {
        return Render<SbBadge>(p =>
        {
            p.Add(x => x.Value, 5);
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersBadgeStructure()
    {
        // Arrange & Act
        var cut = RenderBadge();

        // Assert
        var badge = cut.Find(".sb-badge");
        Assert.NotNull(badge);
        Assert.Equal("span", badge.TagName.ToLowerInvariant());
    }

    [Fact]
    public void RendersValueWhenValueSet()
    {
        // Arrange & Act
        var cut = RenderBadge();

        // Assert
        Assert.Contains("5", cut.Markup);
    }

    [Fact]
    public void RendersChildContentOverValue()
    {
        // Arrange & Act
        var cut = Render<SbBadge>(p => p
            .Add(x => x.Value, 10)
            .AddChildContent("New"));

        // Assert - ChildContent takes precedence
        Assert.Contains("New", cut.Markup);
        Assert.DoesNotContain("10", cut.Markup);
    }

    [Fact]
    public void RendersMaxPlusWhenValueExceedsMax()
    {
        // Arrange & Act - use Render directly to avoid Value added twice
        var cut = Render<SbBadge>(p => p
            .Add(x => x.Value, 150)
            .Add(x => x.Max, 99));

        // Assert
        Assert.Contains("99+", cut.Markup);
    }

    [Fact]
    public void RendersExactValueWhenEqualToMax()
    {
        // Arrange & Act - use Render directly to avoid Value added twice
        var cut = Render<SbBadge>(p => p
            .Add(x => x.Value, 99)
            .Add(x => x.Max, 99));

        // Assert
        Assert.Contains("99", cut.Markup);
        Assert.DoesNotContain("99+", cut.Markup);
    }

    [Fact]
    public void RendersCustomMaxPlus()
    {
        // Arrange & Act - use Render directly to avoid Value added twice
        var cut = Render<SbBadge>(p => p
            .Add(x => x.Value, 50)
            .Add(x => x.Max, 10));

        // Assert
        Assert.Contains("10+", cut.Markup);
    }

    [Fact]
    public void RendersEmptyWhenValueNullAndNoChildContent()
    {
        // Arrange & Act
        var cut = Render<SbBadge>(p => p.Add(x => x.Value, (int?)null));

        // Assert - badge still renders, content is empty
        var badge = cut.Find(".sb-badge");
        Assert.NotNull(badge);
    }

    [Fact]
    public void RendersDotMode()
    {
        // Arrange & Act
        var cut = RenderBadge(p => p.Add(x => x.Dot, true));

        // Assert
        var badge = cut.Find(".sb-badge");
        Assert.Contains("sb-badge--dot", badge.ClassList);
        // Dot mode shows no value/child content inside
        Assert.DoesNotContain("5", cut.Markup);
    }

    [Fact]
    public void AppliesPrimaryColorByDefault()
    {
        // Arrange & Act
        var cut = RenderBadge();

        // Assert
        var badge = cut.Find(".sb-badge");
        Assert.Contains("sb-badge--primary", badge.ClassList);
    }

    [Fact]
    public void AppliesSuccessColor()
    {
        // Arrange & Act
        var cut = RenderBadge(p => p.Add(x => x.Color, SbColor.Success));

        // Assert
        var badge = cut.Find(".sb-badge");
        Assert.Contains("sb-badge--success", badge.ClassList);
    }

    [Fact]
    public void AppliesWarningColor()
    {
        // Arrange & Act
        var cut = RenderBadge(p => p.Add(x => x.Color, SbColor.Warning));

        // Assert
        var badge = cut.Find(".sb-badge");
        Assert.Contains("sb-badge--warning", badge.ClassList);
    }

    [Fact]
    public void AppliesDangerColor()
    {
        // Arrange & Act
        var cut = RenderBadge(p => p.Add(x => x.Color, SbColor.Danger));

        // Assert
        var badge = cut.Find(".sb-badge");
        Assert.Contains("sb-badge--danger", badge.ClassList);
    }

    [Fact]
    public void AppliesAriaLabel()
    {
        // Arrange & Act
        var cut = RenderBadge(p => p.Add(x => x.AriaLabel, "5 unread messages"));

        // Assert
        var badge = cut.Find(".sb-badge");
        Assert.Equal("5 unread messages", badge.GetAttribute("aria-label"));
    }

    [Fact]
    public void AppliesCustomClass()
    {
        // Arrange & Act
        var cut = RenderBadge(p => p.Add(x => x.Class, "my-badge"));

        // Assert
        var badge = cut.Find(".sb-badge");
        Assert.Contains("my-badge", badge.ClassList);
    }

    [Fact]
    public void AppliesInlineStyle()
    {
        // Arrange & Act
        var cut = RenderBadge(p => p.Add(x => x.Style, "top: 0;"));

        // Assert
        var badge = cut.Find(".sb-badge");
        Assert.Contains("top: 0", badge.GetAttribute("style"));
    }
}
