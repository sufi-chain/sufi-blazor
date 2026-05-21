using Bunit;
using SufiChain.SufiBlazor.Components.Layout;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Layout;

public class SbStackTests : BunitContext
{
    private IRenderedComponent<SbStack> RenderStack(
        Action<ComponentParameterCollectionBuilder<SbStack>>? configure = null)
    {
        return Render<SbStack>(p => configure?.Invoke(p));
    }

    [Fact]
    public void RendersStackStructure()
    {
        // Arrange & Act
        var cut = RenderStack();

        // Assert
        var div = cut.Find(".sb-stack");
        Assert.NotNull(div);
        Assert.Equal("div", div.TagName.ToLowerInvariant());
    }

    [Fact]
    public void RendersChildContent()
    {
        // Arrange & Act
        var cut = RenderStack(p => p.AddChildContent("<span>Stack content</span>"));

        // Assert
        Assert.Contains("Stack content", cut.Markup);
        Assert.NotNull(cut.Find("span"));
    }

    [Fact]
    public void UsesColumnDirectionByDefault()
    {
        // Arrange & Act
        var cut = RenderStack();

        // Assert
        var div = cut.Find(".sb-stack");
        Assert.Contains("sb-stack--column", div.ClassList);
    }

    [Theory]
    [InlineData(SbStackDirection.Row, "row")]
    [InlineData(SbStackDirection.Column, "column")]
    public void AppliesDirectionClass(SbStackDirection direction, string expectedClass)
    {
        // Arrange & Act
        var cut = RenderStack(p => p.Add(x => x.Direction, direction));

        // Assert
        var div = cut.Find(".sb-stack");
        Assert.Contains($"sb-stack--{expectedClass}", div.ClassList);
    }

    [Theory]
    [InlineData(SbStackDirection.Row, "sb-stack--sm-row")]
    [InlineData(SbStackDirection.Column, "sb-stack--sm-column")]
    public void AppliesDirectionSmClass_WhenSet(SbStackDirection directionSm, string expectedClass)
    {
        // Arrange & Act
        var cut = RenderStack(p => p.Add(x => x.DirectionSm, directionSm));

        // Assert
        var div = cut.Find(".sb-stack");
        Assert.Contains(expectedClass, div.ClassList);
    }

    [Theory]
    [InlineData(SbStackDirection.Row, "sb-stack--md-row")]
    [InlineData(SbStackDirection.Column, "sb-stack--md-column")]
    public void AppliesDirectionMdClass_WhenSet(SbStackDirection directionMd, string expectedClass)
    {
        // Arrange & Act
        var cut = RenderStack(p => p.Add(x => x.DirectionMd, directionMd));

        // Assert
        var div = cut.Find(".sb-stack");
        Assert.Contains(expectedClass, div.ClassList);
    }

    [Theory]
    [InlineData(SbStackDirection.Row, "sb-stack--lg-row")]
    [InlineData(SbStackDirection.Column, "sb-stack--lg-column")]
    public void AppliesDirectionLgClass_WhenSet(SbStackDirection directionLg, string expectedClass)
    {
        // Arrange & Act
        var cut = RenderStack(p => p.Add(x => x.DirectionLg, directionLg));

        // Assert
        var div = cut.Find(".sb-stack");
        Assert.Contains(expectedClass, div.ClassList);
    }

    [Theory]
    [InlineData(SbStackDirection.Row, "sb-stack--xl-row")]
    [InlineData(SbStackDirection.Column, "sb-stack--xl-column")]
    public void AppliesDirectionXlClass_WhenSet(SbStackDirection directionXl, string expectedClass)
    {
        // Arrange & Act
        var cut = RenderStack(p => p.Add(x => x.DirectionXl, directionXl));

        // Assert
        var div = cut.Find(".sb-stack");
        Assert.Contains(expectedClass, div.ClassList);
    }

    [Fact]
    public void AppliesMultipleResponsiveDirectionClasses_WhenMultipleBreakpointsSet()
    {
        // Arrange & Act - Column on mobile, Row at sm, Column at md (e.g. for tablet), Row at lg+
        var cut = RenderStack(p => p
            .Add(x => x.Direction, SbStackDirection.Column)
            .Add(x => x.DirectionSm, SbStackDirection.Row)
            .Add(x => x.DirectionMd, SbStackDirection.Column)
            .Add(x => x.DirectionLg, SbStackDirection.Row));

        // Assert
        var div = cut.Find(".sb-stack");
        Assert.Contains("sb-stack--column", div.ClassList);
        Assert.Contains("sb-stack--sm-row", div.ClassList);
        Assert.Contains("sb-stack--md-column", div.ClassList);
        Assert.Contains("sb-stack--lg-row", div.ClassList);
    }

    [Fact]
    public void UsesDefaultGapInStyle()
    {
        // Arrange & Act
        var cut = RenderStack();

        // Assert - default Gap is 2
        var div = cut.Find(".sb-stack");
        var style = div.GetAttribute("style");
        Assert.Contains("gap: var(--sb-space-2)", style);
    }

    [Theory]
    [InlineData(0, "0")]
    [InlineData(4, "4")]
    [InlineData(8, "8")]
    [InlineData(16, "16")]
    public void AppliesGapInStyle(int gap, string expectedGap)
    {
        // Arrange & Act
        var cut = RenderStack(p => p.Add(x => x.Gap, gap));

        // Assert
        var div = cut.Find(".sb-stack");
        var style = div.GetAttribute("style");
        Assert.Contains($"gap: var(--sb-space-{expectedGap})", style);
    }

    [Theory]
    [InlineData(SbAlign.Start, "start")]
    [InlineData(SbAlign.Center, "center")]
    [InlineData(SbAlign.End, "end")]
    [InlineData(SbAlign.Stretch, "stretch")]
    [InlineData(SbAlign.Baseline, "baseline")]
    public void AppliesAlignClass(SbAlign align, string expectedClass)
    {
        // Arrange & Act
        var cut = RenderStack(p => p.Add(x => x.Align, align));

        // Assert
        var div = cut.Find(".sb-stack");
        Assert.Contains($"sb-stack--align-{expectedClass}", div.ClassList);
    }

    [Theory]
    [InlineData(SbAlign.Start, "sb-stack--sm-align-start")]
    [InlineData(SbAlign.Center, "sb-stack--sm-align-center")]
    [InlineData(SbAlign.End, "sb-stack--sm-align-end")]
    [InlineData(SbAlign.Stretch, "sb-stack--sm-align-stretch")]
    [InlineData(SbAlign.Baseline, "sb-stack--sm-align-baseline")]
    public void AppliesAlignSmClass_WhenSet(SbAlign alignSm, string expectedClass)
    {
        // Arrange & Act
        var cut = RenderStack(p => p.Add(x => x.AlignSm, alignSm));

        // Assert
        var div = cut.Find(".sb-stack");
        Assert.Contains(expectedClass, div.ClassList);
    }

    [Theory]
    [InlineData(SbAlign.Center, "sb-stack--md-align-center")]
    public void AppliesAlignMdClass_WhenSet(SbAlign alignMd, string expectedClass)
    {
        // Arrange & Act
        var cut = RenderStack(p => p.Add(x => x.AlignMd, alignMd));

        // Assert
        var div = cut.Find(".sb-stack");
        Assert.Contains(expectedClass, div.ClassList);
    }

    [Fact]
    public void AppliesMultipleResponsiveAlignClasses_WhenMultipleBreakpointsSet()
    {
        // Arrange & Act
        var cut = RenderStack(p => p
            .Add(x => x.Align, SbAlign.Stretch)
            .Add(x => x.AlignSm, SbAlign.Center)
            .Add(x => x.AlignMd, SbAlign.Start)
            .Add(x => x.AlignLg, SbAlign.Center));

        // Assert
        var div = cut.Find(".sb-stack");
        Assert.Contains("sb-stack--align-stretch", div.ClassList);
        Assert.Contains("sb-stack--sm-align-center", div.ClassList);
        Assert.Contains("sb-stack--md-align-start", div.ClassList);
        Assert.Contains("sb-stack--lg-align-center", div.ClassList);
    }

    [Theory]
    [InlineData(SbJustify.Start, "start")]
    [InlineData(SbJustify.Center, "center")]
    [InlineData(SbJustify.End, "end")]
    [InlineData(SbJustify.SpaceBetween, "between")]
    [InlineData(SbJustify.SpaceAround, "around")]
    [InlineData(SbJustify.SpaceEvenly, "evenly")]
    public void AppliesJustifyClass(SbJustify justify, string expectedClass)
    {
        // Arrange & Act
        var cut = RenderStack(p => p.Add(x => x.Justify, justify));

        // Assert
        var div = cut.Find(".sb-stack");
        Assert.Contains($"sb-stack--justify-{expectedClass}", div.ClassList);
    }

    [Fact]
    public void AddsWrapClassWhenWrapTrue()
    {
        // Arrange & Act
        var cut = RenderStack(p => p.Add(x => x.Wrap, true));

        // Assert
        var div = cut.Find(".sb-stack");
        Assert.Contains("sb-stack--wrap", div.ClassList);
    }

    [Fact]
    public void DoesNotAddWrapClassWhenWrapFalse()
    {
        // Arrange & Act
        var cut = RenderStack(p => p.Add(x => x.Wrap, false));

        // Assert
        var div = cut.Find(".sb-stack");
        Assert.DoesNotContain("sb-stack--wrap", div.ClassList);
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderStack(p => p.Add(x => x.Class, "my-stack"));

        // Assert
        var div = cut.Find(".sb-stack");
        Assert.Contains("my-stack", div.ClassList);
    }

    [Fact]
    public void AppliesStyleParameter()
    {
        // Arrange & Act
        var cut = RenderStack(p => p.Add(x => x.Style, "min-width: 200px;"));

        // Assert
        var div = cut.Find(".sb-stack");
        var style = div.GetAttribute("style");
        Assert.Contains("gap: var(--sb-space-2)", style);
        Assert.Contains("min-width: 200px", style);
    }

    [Fact]
    public void PassesAdditionalAttributes()
    {
        // Arrange & Act
        var cut = RenderStack(p => p.Add(x => x.AdditionalAttributes, new Dictionary<string, object>
        {
            { "data-testid", "stack-root" }
        }));

        // Assert
        var div = cut.Find(".sb-stack");
        Assert.Equal("stack-root", div.GetAttribute("data-testid"));
    }
}
