using Microsoft.AspNetCore.Components;
using Bunit;
using SufiChain.SufiBlazor.Components.Layout;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Layout;

public class SbSpacerTests : BunitContext
{
    private IRenderedComponent<SbSpacer> RenderSpacer(
        Action<ComponentParameterCollectionBuilder<SbSpacer>>? configure = null)
    {
        return Render<SbSpacer>(p => configure?.Invoke(p));
    }

    [Fact]
    public void RendersSpacerStructure()
    {
        // Arrange & Act
        var cut = RenderSpacer();

        // Assert
        var div = cut.Find(".sb-spacer");
        Assert.NotNull(div);
        Assert.Equal("div", div.TagName.ToLowerInvariant());
        Assert.Equal("true", div.GetAttribute("aria-hidden"));
    }

    [Fact]
    public void UsesMediumHeightByDefault()
    {
        // Arrange & Act
        var cut = RenderSpacer();

        // Assert - Medium = 1rem
        var div = cut.Find(".sb-spacer");
        var style = div.GetAttribute("style");
        Assert.Contains("height: 1rem", style);
    }

    [Theory]
    [InlineData(SbSpacerSize.ExtraSmall, "0.25rem")]
    [InlineData(SbSpacerSize.Small, "0.5rem")]
    [InlineData(SbSpacerSize.Medium, "1rem")]
    [InlineData(SbSpacerSize.Large, "1.5rem")]
    [InlineData(SbSpacerSize.ExtraLarge, "2rem")]
    public void AppliesCorrectHeightForEachSize(SbSpacerSize size, string expectedHeight)
    {
        // Arrange & Act
        var cut = RenderSpacer(p => p.Add(x => x.Size, size));

        // Assert
        var div = cut.Find(".sb-spacer");
        var style = div.GetAttribute("style");
        Assert.Contains($"height: {expectedHeight}", style);
    }

    [Fact]
    public void HeightOverridesSize()
    {
        // Arrange & Act
        var cut = RenderSpacer(p => p
            .Add(x => x.Size, SbSpacerSize.Large)
            .Add(x => x.Height, "3rem"));

        // Assert - custom Height overrides Size
        var div = cut.Find(".sb-spacer");
        var style = div.GetAttribute("style");
        Assert.Contains("height: 3rem", style);
    }

    [Fact]
    public void AppliesWidthWhenProvided()
    {
        // Arrange & Act
        var cut = RenderSpacer(p => p.Add(x => x.Width, "2rem"));

        // Assert
        var div = cut.Find(".sb-spacer");
        var style = div.GetAttribute("style");
        Assert.Contains("width: 2rem", style);
    }

    [Fact]
    public void AppliesFlexGrowWhenGrowTrue()
    {
        // Arrange & Act
        var cut = RenderSpacer(p => p.Add(x => x.Grow, true));

        // Assert - Grow takes precedence, no height
        var div = cut.Find(".sb-spacer");
        var style = div.GetAttribute("style");
        Assert.Contains("flex-grow: 1", style);
        Assert.DoesNotContain("height:", style);
    }

    [Fact]
    public void GrowWithWidthAppliesBoth()
    {
        // Arrange & Act
        var cut = RenderSpacer(p => p
            .Add(x => x.Grow, true)
            .Add(x => x.Width, "1rem"));

        // Assert
        var div = cut.Find(".sb-spacer");
        var style = div.GetAttribute("style");
        Assert.Contains("flex-grow: 1", style);
        Assert.Contains("width: 1rem", style);
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderSpacer(p => p.Add(x => x.Class, "my-spacer"));

        // Assert
        var div = cut.Find(".sb-spacer");
        Assert.Contains("my-spacer", div.ClassList);
    }
}
