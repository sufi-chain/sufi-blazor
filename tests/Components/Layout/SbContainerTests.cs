using Microsoft.AspNetCore.Components;
using Bunit;
using SufiChain.SufiBlazor.Components.Layout;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Layout;

public class SbContainerTests : BunitContext
{
    private IRenderedComponent<SbContainer> RenderContainer(
        Action<ComponentParameterCollectionBuilder<SbContainer>>? configure = null)
    {
        return Render<SbContainer>(p => configure?.Invoke(p));
    }

    [Fact]
    public void RendersContainerStructure()
    {
        // Arrange & Act
        var cut = RenderContainer();

        // Assert
        var div = cut.Find(".sb-container");
        Assert.NotNull(div);
        Assert.Equal("div", div.TagName.ToLowerInvariant());
    }

    [Fact]
    public void RendersChildContent()
    {
        // Arrange & Act
        var cut = RenderContainer(p => p.AddChildContent("<p>Content</p>"));

        // Assert
        Assert.Contains("Content", cut.Markup);
        Assert.NotNull(cut.Find("p"));
    }

    [Fact]
    public void UsesLgMaxWidthByDefault()
    {
        // Arrange & Act
        var cut = RenderContainer();

        // Assert
        var div = cut.Find(".sb-container");
        Assert.Contains("sb-container--lg", div.ClassList);
    }

    [Fact]
    public void AppliesMaxWidthClass()
    {
        // Arrange & Act
        var cut = RenderContainer(p => p.Add(x => x.MaxWidth, SbContainerMaxWidth.Sm));

        // Assert
        var div = cut.Find(".sb-container");
        Assert.Contains("sb-container--sm", div.ClassList);
    }

    [Theory]
    [InlineData(SbContainerMaxWidth.Sm, "sm")]
    [InlineData(SbContainerMaxWidth.Md, "md")]
    [InlineData(SbContainerMaxWidth.Lg, "lg")]
    [InlineData(SbContainerMaxWidth.Xl, "xl")]
    [InlineData(SbContainerMaxWidth.Xxl, "xxl")]
    [InlineData(SbContainerMaxWidth.Fluid, "fluid")]
    public void AppliesCorrectMaxWidthClassForEachPreset(SbContainerMaxWidth preset, string expectedClass)
    {
        // Arrange & Act
        var cut = RenderContainer(p => p.Add(x => x.MaxWidth, preset));

        // Assert
        var div = cut.Find(".sb-container");
        Assert.Contains($"sb-container--{expectedClass}", div.ClassList);
    }

    [Fact]
    public void AddsNoGuttersClassWhenDisableGuttersTrue()
    {
        // Arrange & Act
        var cut = RenderContainer(p => p.Add(x => x.DisableGutters, true));

        // Assert
        var div = cut.Find(".sb-container");
        Assert.Contains("sb-container--no-gutters", div.ClassList);
    }

    [Fact]
    public void DoesNotAddNoGuttersClassWhenDisableGuttersFalse()
    {
        // Arrange & Act
        var cut = RenderContainer(p => p.Add(x => x.DisableGutters, false));

        // Assert
        var div = cut.Find(".sb-container");
        Assert.DoesNotContain("sb-container--no-gutters", div.ClassList);
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderContainer(p => p.Add(x => x.Class, "my-container"));

        // Assert
        var div = cut.Find(".sb-container");
        Assert.Contains("my-container", div.ClassList);
    }

    [Fact]
    public void AppliesStyleParameter()
    {
        // Arrange & Act
        var cut = RenderContainer(p => p.Add(x => x.Style, "padding-top: 20px;"));

        // Assert
        var div = cut.Find(".sb-container");
        Assert.Contains("padding-top: 20px", div.GetAttribute("style"));
    }

    [Fact]
    public void AppliesAdditionalAttributes()
    {
        // Arrange & Act
        var cut = RenderContainer(p => p.Add(x => x.AdditionalAttributes, new Dictionary<string, object>
        {
            { "data-testid", "main-container" },
            { "role", "main" }
        }));

        // Assert
        var div = cut.Find(".sb-container");
        Assert.Equal("main-container", div.GetAttribute("data-testid"));
        Assert.Equal("main", div.GetAttribute("role"));
    }
}
