using Microsoft.AspNetCore.Components;
using Bunit;
using SufiChain.SufiBlazor.Components.Feedback;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Feedback;

public class SbSkeletonTests : BunitContext
{
    private IRenderedComponent<SbSkeleton> RenderSkeleton(
        Action<ComponentParameterCollectionBuilder<SbSkeleton>>? configure = null)
    {
        return Render<SbSkeleton>(p => configure?.Invoke(p));
    }

    [Fact]
    public void RendersSkeletonStructure()
    {
        // Arrange & Act
        var cut = RenderSkeleton();

        // Assert
        var skeleton = cut.Find(".sb-skeleton");
        Assert.NotNull(skeleton);
        Assert.Equal("div", skeleton.TagName.ToLowerInvariant());
        Assert.Equal("true", skeleton.GetAttribute("aria-hidden"));
    }

    [Fact]
    public void AppliesTextVariantByDefault()
    {
        // Arrange & Act
        var cut = RenderSkeleton();

        // Assert
        var skeleton = cut.Find(".sb-skeleton");
        Assert.Contains("sb-skeleton--text", skeleton.ClassList);
    }

    [Fact]
    public void AppliesCircularVariant()
    {
        // Arrange & Act
        var cut = RenderSkeleton(p => p.Add(x => x.Variant, SbSkeletonVariant.Circular));

        // Assert
        var skeleton = cut.Find(".sb-skeleton");
        Assert.Contains("sb-skeleton--circular", skeleton.ClassList);
    }

    [Fact]
    public void AppliesRectangularVariant()
    {
        // Arrange & Act
        var cut = RenderSkeleton(p => p.Add(x => x.Variant, SbSkeletonVariant.Rectangular));

        // Assert
        var skeleton = cut.Find(".sb-skeleton");
        Assert.Contains("sb-skeleton--rectangular", skeleton.ClassList);
    }

    [Fact]
    public void AppliesRoundedVariant()
    {
        // Arrange & Act
        var cut = RenderSkeleton(p => p.Add(x => x.Variant, SbSkeletonVariant.Rounded));

        // Assert
        var skeleton = cut.Find(".sb-skeleton");
        Assert.Contains("sb-skeleton--rounded", skeleton.ClassList);
    }

    [Fact]
    public void AppliesAnimatedClassByDefault()
    {
        // Arrange & Act
        var cut = RenderSkeleton();

        // Assert
        var skeleton = cut.Find(".sb-skeleton");
        Assert.Contains("sb-skeleton--animated", skeleton.ClassList);
    }

    [Fact]
    public void DoesNotApplyAnimatedClassWhenAnimationFalse()
    {
        // Arrange & Act
        var cut = RenderSkeleton(p => p.Add(x => x.Animation, false));

        // Assert
        var skeleton = cut.Find(".sb-skeleton");
        Assert.DoesNotContain("sb-skeleton--animated", skeleton.ClassList);
    }

    [Fact]
    public void AppliesWidthStyle()
    {
        // Arrange & Act
        var cut = RenderSkeleton(p => p.Add(x => x.Width, "200px"));

        // Assert
        var skeleton = cut.Find(".sb-skeleton");
        Assert.Contains("width: 200px", skeleton.GetAttribute("style"));
    }

    [Fact]
    public void AppliesHeightStyle()
    {
        // Arrange & Act
        var cut = RenderSkeleton(p => p.Add(x => x.Height, "24px"));

        // Assert
        var skeleton = cut.Find(".sb-skeleton");
        Assert.Contains("height: 24px", skeleton.GetAttribute("style"));
    }

    [Fact]
    public void AppliesBorderRadiusStyle()
    {
        // Arrange & Act
        var cut = RenderSkeleton(p => p.Add(x => x.BorderRadius, "8px"));

        // Assert
        var skeleton = cut.Find(".sb-skeleton");
        Assert.Contains("border-radius: 8px", skeleton.GetAttribute("style"));
    }

    [Fact]
    public void AppliesMultipleDimensionStyles()
    {
        // Arrange & Act
        var cut = RenderSkeleton(p => p
            .Add(x => x.Width, "100%")
            .Add(x => x.Height, "200px")
            .Add(x => x.BorderRadius, "4px"));

        // Assert
        var style = cut.Find(".sb-skeleton").GetAttribute("style");
        Assert.Contains("width: 100%", style);
        Assert.Contains("height: 200px", style);
        Assert.Contains("border-radius: 4px", style);
    }

    [Fact]
    public void AppliesCustomClass()
    {
        // Arrange & Act
        var cut = RenderSkeleton(p => p.Add(x => x.Class, "my-skeleton"));

        // Assert
        var skeleton = cut.Find(".sb-skeleton");
        Assert.Contains("my-skeleton", skeleton.ClassList);
    }
}
