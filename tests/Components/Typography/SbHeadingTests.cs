using Microsoft.AspNetCore.Components;
using Bunit;
using SufiChain.SufiBlazor.Components;
using SufiChain.SufiBlazor.Components.Typography;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Typography;

public class SbHeadingTests : BunitContext
{
    private IRenderedComponent<SbHeading> RenderHeading(
        Action<ComponentParameterCollectionBuilder<SbHeading>>? configure = null)
    {
        return Render<SbHeading>(p =>
        {
            p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "Heading content")));
            configure?.Invoke(p);
        });
    }

    [Theory]
    [InlineData(1, "h1")]
    [InlineData(2, "h2")]
    [InlineData(3, "h3")]
    [InlineData(4, "h4")]
    [InlineData(5, "h5")]
    [InlineData(6, "h6")]
    public void RendersCorrectTagForLevel(int level, string expectedTag)
    {
        // Arrange & Act
        var cut = RenderHeading(p => p.Add(x => x.Level, level));

        // Assert
        var heading = cut.Find(expectedTag);
        Assert.NotNull(heading);
        Assert.Equal(expectedTag, heading.TagName.ToLowerInvariant());
    }

    [Fact]
    public void DefaultLevelRendersH2()
    {
        // Arrange & Act
        var cut = RenderHeading();

        // Assert
        var h2 = cut.Find("h2");
        Assert.NotNull(h2);
    }

    [Fact]
    public void InvalidLevelRendersH2AsFallback()
    {
        // Arrange & Act
        var cut = RenderHeading(p => p.Add(x => x.Level, 0));

        // Assert
        var h2 = cut.Find("h2");
        Assert.NotNull(h2);
    }

    [Fact]
    public void RendersChildContent()
    {
        // Arrange & Act
        var cut = RenderHeading();

        // Assert
        Assert.Contains("Heading content", cut.Markup);
    }

    [Fact]
    public void HasBaseHeadingClass()
    {
        // Arrange & Act
        var cut = RenderHeading();

        // Assert
        var heading = cut.Find("h2");
        Assert.Contains("sb-heading", heading.ClassList);
    }

    [Theory]
    [InlineData(1, "h1")]
    [InlineData(2, "h2")]
    [InlineData(4, "h4")]
    [InlineData(6, "h6")]
    public void AppliesVisualSizeClass(int level, string expectedSuffix)
    {
        // Arrange & Act
        var cut = RenderHeading(p => p.Add(x => x.Level, level));

        // Assert
        var heading = cut.Find(expectedSuffix);
        Assert.Contains($"sb-heading--{expectedSuffix}", heading.ClassList);
    }

    [Fact]
    public void SizeOverrideAffectsVisualSizeClass()
    {
        // Arrange & Act - Level 3 but Size 1 renders h3 tag with sb-heading--h1
        var cut = RenderHeading(p => p
            .Add(x => x.Level, 3)
            .Add(x => x.Size, 1));

        // Assert
        var h3 = cut.Find("h3");
        Assert.NotNull(h3);
        Assert.Contains("sb-heading--h1", h3.ClassList);
    }

    [Theory]
    [InlineData(SbColor.Primary, "primary")]
    [InlineData(SbColor.Success, "success")]
    [InlineData(SbColor.Muted, "muted")]
    public void AppliesColorClass(SbColor color, string expectedSuffix)
    {
        // Arrange & Act
        var cut = RenderHeading(p => p.Add(x => x.Color, color));

        // Assert
        var heading = cut.Find("h2");
        Assert.Contains($"sb-heading--{expectedSuffix}", heading.ClassList);
    }

    [Theory]
    [InlineData(SbTextAlign.Center, "center")]
    [InlineData(SbTextAlign.End, "end")]
    [InlineData(SbTextAlign.Justify, "justify")]
    public void AppliesAlignClass(SbTextAlign align, string expectedSuffix)
    {
        // Arrange & Act
        var cut = RenderHeading(p => p.Add(x => x.Align, align));

        // Assert
        var heading = cut.Find("h2");
        Assert.Contains($"sb-heading--align-{expectedSuffix}", heading.ClassList);
    }

    [Fact]
    public void AppliesTruncateClass()
    {
        // Arrange & Act
        var cut = RenderHeading(p => p.Add(x => x.Truncate, true));

        // Assert
        var heading = cut.Find("h2");
        Assert.Contains("sb-heading--truncate", heading.ClassList);
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderHeading(p => p.Add(x => x.Class, "my-heading"));

        // Assert
        var heading = cut.Find("h2");
        Assert.Contains("my-heading", heading.ClassList);
    }

    [Fact]
    public void AppliesStyleParameter()
    {
        // Arrange & Act
        var cut = RenderHeading(p => p.Add(x => x.Style, "font-weight: 700"));

        // Assert
        var heading = cut.Find("h2");
        Assert.Contains("font-weight: 700", heading.GetAttribute("style") ?? "");
    }

    [Fact]
    public void PassesAdditionalAttributes()
    {
        // Arrange & Act
        var cut = RenderHeading(p => p
            .Add(x => x.AdditionalAttributes, new Dictionary<string, object> { ["data-test"] = "heading" }));

        // Assert
        var heading = cut.Find("h2");
        Assert.Equal("heading", heading.GetAttribute("data-test"));
    }
}
