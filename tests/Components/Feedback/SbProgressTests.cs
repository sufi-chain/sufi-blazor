using Microsoft.AspNetCore.Components;
using Bunit;
using SufiChain.SufiBlazor.Components;
using SufiChain.SufiBlazor.Components.Feedback;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Feedback;

public class SbProgressTests : BunitContext
{
    private IRenderedComponent<SbProgress> RenderProgress(
        Action<ComponentParameterCollectionBuilder<SbProgress>>? configure = null)
    {
        return Render<SbProgress>(p =>
        {
            p.Add(x => x.Value, 50);
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersLinearStructureByDefault()
    {
        // Arrange & Act
        var cut = RenderProgress();

        // Assert
        var progress = cut.Find(".sb-progress");
        Assert.NotNull(progress);
        Assert.Equal("progressbar", progress.GetAttribute("role"));
        Assert.NotNull(cut.Find(".sb-progress__track"));
        Assert.NotNull(cut.Find(".sb-progress__bar"));
        Assert.Contains("sb-progress--linear", progress.ClassList);
    }

    [Fact]
    public void RendersCircularStructure()
    {
        // Arrange & Act
        var cut = RenderProgress(p => p.Add(x => x.Type, SbProgressType.Circular));

        // Assert
        var progress = cut.Find(".sb-progress");
        Assert.NotNull(progress);
        Assert.Equal("svg", progress.TagName.ToLowerInvariant());
        Assert.NotNull(cut.Find(".sb-progress__track-circle"));
        Assert.NotNull(cut.Find(".sb-progress__bar-circle"));
        Assert.Contains("sb-progress--circular", progress.ClassList);
    }

    [Fact]
    public void AppliesValueToLinearBar()
    {
        // Arrange & Act - use Render directly to avoid Value added twice
        var cut = Render<SbProgress>(p => p.Add(x => x.Value, 75));

        // Assert
        var bar = cut.Find(".sb-progress__bar");
        Assert.NotNull(bar);
        Assert.Contains("width: 75%", bar.GetAttribute("style"));
    }

    [Fact]
    public void ClampsValueTo100()
    {
        // Arrange & Act
        var cut = Render<SbProgress>(p => p.Add(x => x.Value, 150));

        // Assert
        var bar = cut.Find(".sb-progress__bar");
        Assert.NotNull(bar);
        Assert.Contains("width: 100%", bar.GetAttribute("style"));
    }

    [Fact]
    public void ClampsValueTo0()
    {
        // Arrange & Act
        var cut = Render<SbProgress>(p => p.Add(x => x.Value, -10));

        // Assert
        var bar = cut.Find(".sb-progress__bar");
        Assert.NotNull(bar);
        Assert.Contains("width: 0%", bar.GetAttribute("style"));
    }

    [Fact]
    public void SetsAriaValuenowWhenNotIndeterminate()
    {
        // Arrange & Act
        var cut = Render<SbProgress>(p => p.Add(x => x.Value, 60));

        // Assert
        var progress = cut.Find(".sb-progress");
        Assert.Equal("60", progress.GetAttribute("aria-valuenow"));
        Assert.Equal("0", progress.GetAttribute("aria-valuemin"));
        Assert.Equal("100", progress.GetAttribute("aria-valuemax"));
    }

    [Fact]
    public void SetsAriaValuenowNullWhenIndeterminate()
    {
        // Arrange & Act
        var cut = RenderProgress(p => p.Add(x => x.Indeterminate, true));

        // Assert
        var progress = cut.Find(".sb-progress");
        Assert.Null(progress.GetAttribute("aria-valuenow"));
    }

    [Fact]
    public void AppliesIndeterminateClass()
    {
        // Arrange & Act
        var cut = RenderProgress(p => p.Add(x => x.Indeterminate, true));

        // Assert
        var progress = cut.Find(".sb-progress");
        Assert.Contains("sb-progress--indeterminate", progress.ClassList);
    }

    [Fact]
    public void RendersLabelWhenShowLabelTrue()
    {
        // Arrange & Act
        var cut = Render<SbProgress>(p => p
            .Add(x => x.Value, 42)
            .Add(x => x.ShowLabel, true));

        // Assert
        var label = cut.Find(".sb-progress__label");
        Assert.NotNull(label);
        Assert.Contains("42%", label.TextContent);
    }

    [Fact]
    public void DoesNotRenderLabelWhenShowLabelFalse()
    {
        // Arrange & Act
        var cut = RenderProgress();

        // Assert
        Assert.Empty(cut.FindAll(".sb-progress__label"));
    }

    [Fact]
    public void DoesNotRenderLabelWhenIndeterminate()
    {
        // Arrange & Act
        var cut = RenderProgress(p => p
            .Add(x => x.ShowLabel, true)
            .Add(x => x.Indeterminate, true));

        // Assert
        Assert.Empty(cut.FindAll(".sb-progress__label"));
    }

    [Fact]
    public void AppliesPrimaryColorByDefault()
    {
        // Arrange & Act
        var cut = RenderProgress();

        // Assert
        var progress = cut.Find(".sb-progress");
        Assert.Contains("sb-progress--primary", progress.ClassList);
    }

    [Fact]
    public void AppliesSuccessColor()
    {
        // Arrange & Act
        var cut = RenderProgress(p => p.Add(x => x.Color, SbColor.Success));

        // Assert
        var progress = cut.Find(".sb-progress");
        Assert.Contains("sb-progress--success", progress.ClassList);
    }

    [Fact]
    public void AppliesMdSizeByDefault()
    {
        // Arrange & Act
        var cut = RenderProgress();

        // Assert
        var progress = cut.Find(".sb-progress");
        Assert.Contains("sb-progress--md", progress.ClassList);
    }

    [Fact]
    public void AppliesSmSize()
    {
        // Arrange & Act
        var cut = RenderProgress(p => p.Add(x => x.Size, SbSize.Sm));

        // Assert
        var progress = cut.Find(".sb-progress");
        Assert.Contains("sb-progress--sm", progress.ClassList);
    }

    [Fact]
    public void AppliesCustomClass()
    {
        // Arrange & Act
        var cut = RenderProgress(p => p.Add(x => x.Class, "my-progress"));

        // Assert
        var progress = cut.Find(".sb-progress");
        Assert.Contains("my-progress", progress.ClassList);
    }

    [Fact]
    public void AppliesInlineStyle()
    {
        // Arrange & Act
        var cut = RenderProgress(p => p.Add(x => x.Style, "height: 8px;"));

        // Assert
        var progress = cut.Find(".sb-progress");
        Assert.Contains("height: 8px", progress.GetAttribute("style"));
    }

    [Fact]
    public void CircularRendersLabelWhenShowLabelTrue()
    {
        // Arrange & Act
        var cut = Render<SbProgress>(p => p
            .Add(x => x.Type, SbProgressType.Circular)
            .Add(x => x.Value, 33)
            .Add(x => x.ShowLabel, true));

        // Assert
        Assert.Contains("33%", cut.Markup);
        var labelText = cut.Find(".sb-progress__label-text");
        Assert.NotNull(labelText);
    }
}
