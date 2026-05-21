using Microsoft.AspNetCore.Components;
using Bunit;
using SufiChain.SufiBlazor.Components.Surfaces;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Surfaces;

public class SbSurfaceTests : BunitContext
{
    private IRenderedComponent<SbSurface> RenderSurface(
        Action<ComponentParameterCollectionBuilder<SbSurface>>? configure = null)
    {
        return Render<SbSurface>(p =>
        {
            p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "<span>Surface content</span>")));
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersSurfaceStructure()
    {
        // Arrange & Act
        var cut = RenderSurface();

        // Assert
        var surface = cut.Find(".sb-surface");
        Assert.NotNull(surface);
        Assert.Equal("div", surface.TagName.ToLowerInvariant());
    }

    [Fact]
    public void RendersChildContent()
    {
        // Arrange & Act
        var cut = RenderSurface();

        // Assert
        Assert.Contains("Surface content", cut.Markup);
    }

    [Fact]
    public void AppliesElevationClass()
    {
        // Arrange & Act
        var cut = RenderSurface(p => p.Add(x => x.Elevation, 2));

        // Assert
        var surface = cut.Find(".sb-surface");
        Assert.Contains("sb-surface--elevation-2", surface.ClassList);
    }

    [Theory]
    [InlineData(0, "0")]
    [InlineData(1, "1")]
    [InlineData(3, "3")]
    [InlineData(5, "5")]
    public void AppliesCorrectElevationClass(int elevation, string expectedSuffix)
    {
        // Arrange & Act
        var cut = RenderSurface(p => p.Add(x => x.Elevation, elevation));

        // Assert
        var surface = cut.Find(".sb-surface");
        Assert.Contains($"sb-surface--elevation-{expectedSuffix}", surface.ClassList);
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderSurface(p => p.Add(x => x.Class, "my-surface"));

        // Assert
        var surface = cut.Find(".sb-surface");
        Assert.Contains("my-surface", surface.ClassList);
    }

    [Fact]
    public void AppliesBackgroundStyle()
    {
        // Arrange & Act
        var cut = RenderSurface(p => p.Add(x => x.Background, "#f0f0f0"));

        // Assert
        var surface = cut.Find(".sb-surface");
        var style = surface.GetAttribute("style") ?? "";
        Assert.Contains("background-color: #f0f0f0", style);
    }

    [Fact]
    public void AppliesBorderRadiusStyle()
    {
        // Arrange & Act
        var cut = RenderSurface(p => p.Add(x => x.BorderRadius, "8px"));

        // Assert
        var surface = cut.Find(".sb-surface");
        var style = surface.GetAttribute("style") ?? "";
        Assert.Contains("border-radius: 8px", style);
    }

    [Fact]
    public void AppliesPaddingStyle()
    {
        // Arrange & Act
        var cut = RenderSurface(p => p.Add(x => x.Padding, "16px"));

        // Assert
        var surface = cut.Find(".sb-surface");
        var style = surface.GetAttribute("style") ?? "";
        Assert.Contains("padding: 16px", style);
    }

    [Fact]
    public void AppliesBorderWhenOutlined()
    {
        // Arrange & Act
        var cut = RenderSurface(p => p.Add(x => x.Outlined, true));

        // Assert
        var surface = cut.Find(".sb-surface");
        var style = surface.GetAttribute("style") ?? "";
        Assert.Contains("border: 1px solid var(--sb-color-border)", style);
    }

    [Fact]
    public void CombinesMultipleStyles()
    {
        // Arrange & Act
        var cut = RenderSurface(p => p
            .Add(x => x.Background, "var(--sb-color-primary)")
            .Add(x => x.Padding, "24px")
            .Add(x => x.Outlined, true));

        // Assert
        var surface = cut.Find(".sb-surface");
        var style = surface.GetAttribute("style") ?? "";
        Assert.Contains("background-color: var(--sb-color-primary)", style);
        Assert.Contains("padding: 24px", style);
        Assert.Contains("border: 1px solid var(--sb-color-border)", style);
    }
}
