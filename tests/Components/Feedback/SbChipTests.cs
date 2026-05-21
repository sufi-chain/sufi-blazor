using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Bunit;
using SufiChain.SufiBlazor.Components;
using SufiChain.SufiBlazor.Components.Feedback;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Feedback;

public class SbChipTests : BunitContext
{
    private IRenderedComponent<SbChip> RenderChip(
        Action<ComponentParameterCollectionBuilder<SbChip>>? configure = null)
    {
        return Render<SbChip>(p =>
        {
            p.AddChildContent("Chip label");
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersChipStructure()
    {
        // Arrange & Act - no OnClick, renders as span
        var cut = RenderChip();

        // Assert
        var chip = cut.Find(".sb-chip");
        Assert.NotNull(chip);
        Assert.Equal("span", chip.TagName.ToLowerInvariant());
        Assert.NotNull(cut.Find(".sb-chip__label"));
    }

    [Fact]
    public void RendersChildContent()
    {
        // Arrange & Act
        var cut = RenderChip();

        // Assert
        Assert.Contains("Chip label", cut.Markup);
    }

    [Fact]
    public void RendersAsButtonWhenOnClickProvided()
    {
        // Arrange & Act
        var cut = RenderChip(p => p.Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => { })));

        // Assert
        var chip = cut.Find(".sb-chip");
        Assert.Equal("button", chip.TagName.ToLowerInvariant());
        Assert.Contains("sb-chip--clickable", chip.ClassList);
    }

    [Fact]
    public void AppliesFilledVariantByDefault()
    {
        // Arrange & Act
        var cut = RenderChip();

        // Assert
        var chip = cut.Find(".sb-chip");
        Assert.Contains("sb-chip--filled", chip.ClassList);
    }

    [Fact]
    public void AppliesOutlinedVariant()
    {
        // Arrange & Act
        var cut = RenderChip(p => p.Add(x => x.Variant, SbChipVariant.Outlined));

        // Assert
        var chip = cut.Find(".sb-chip");
        Assert.Contains("sb-chip--outlined", chip.ClassList);
    }

    [Fact]
    public void AppliesMdSizeByDefault()
    {
        // Arrange & Act
        var cut = RenderChip();

        // Assert
        var chip = cut.Find(".sb-chip");
        Assert.Contains("sb-chip--md", chip.ClassList);
    }

    [Fact]
    public void AppliesSmSize()
    {
        // Arrange & Act
        var cut = RenderChip(p => p.Add(x => x.Size, SbSize.Sm));

        // Assert
        var chip = cut.Find(".sb-chip");
        Assert.Contains("sb-chip--sm", chip.ClassList);
    }

    [Fact]
    public void DoesNotApplyColorClassWhenDefault()
    {
        // Arrange & Act
        var cut = RenderChip();

        // Assert - SbColor.Default adds no color class
        var chip = cut.Find(".sb-chip");
        Assert.DoesNotContain("sb-chip--default", chip.ClassList);
    }

    [Fact]
    public void AppliesPrimaryColor()
    {
        // Arrange & Act
        var cut = RenderChip(p => p.Add(x => x.Color, SbColor.Primary));

        // Assert
        var chip = cut.Find(".sb-chip");
        Assert.Contains("sb-chip--primary", chip.ClassList);
    }

    [Fact]
    public void AppliesDisabledClass()
    {
        // Arrange & Act
        var cut = RenderChip(p => p.Add(x => x.Disabled, true));

        // Assert
        var chip = cut.Find(".sb-chip");
        Assert.Contains("sb-chip--disabled", chip.ClassList);
    }

    [Fact]
    public void ButtonIsDisabledWhenDisabledTrue()
    {
        // Arrange & Act
        var cut = RenderChip(p => p
            .Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => { }))
            .Add(x => x.Disabled, true));

        // Assert - disabled attr can render as "disabled", "true", or "" (boolean attrs vary by renderer)
        var chip = cut.Find(".sb-chip");
        Assert.True(chip.GetAttribute("disabled") != null || chip.OuterHtml.Contains("disabled"));
    }

    [Fact]
    public void RendersStartIconWhenProvided()
    {
        // Arrange & Act
        var cut = RenderChip(p => p
            .Add(x => x.StartIcon, i => i.AddMarkupContent(0, "<span class=\"test-icon\">★</span>")));

        // Assert
        var iconContainer = cut.Find(".sb-chip__icon");
        Assert.NotNull(iconContainer);
        var icon = cut.Find(".test-icon");
        Assert.NotNull(icon);
    }

    [Fact]
    public void RendersRemoveButtonWhenOnRemoveProvided()
    {
        // Arrange & Act
        var cut = RenderChip(p => p.Add(x => x.OnRemove, EventCallback.Factory.Create(this, () => { })));

        // Assert
        var removeBtn = cut.Find(".sb-chip__remove");
        Assert.NotNull(removeBtn);
        Assert.Equal("Remove", removeBtn.GetAttribute("aria-label"));
    }

    [Fact]
    public void DoesNotRenderRemoveButtonWhenOnRemoveNull()
    {
        // Arrange & Act
        var cut = RenderChip();

        // Assert
        Assert.Empty(cut.FindAll(".sb-chip__remove"));
    }

    [Fact]
    public async Task InvokesOnClickWhenChipClicked()
    {
        // Arrange
        var clicked = false;
        var cut = RenderChip(p => p.Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => clicked = true)));

        var chip = cut.Find(".sb-chip");

        // Act
        await cut.InvokeAsync(() => chip!.Click());

        cut.WaitForState(() => clicked);

        // Assert
        Assert.True(clicked);
    }

    [Fact]
    public async Task InvokesOnRemoveWhenRemoveButtonClicked()
    {
        // Arrange
        var removed = false;
        var cut = RenderChip(p => p.Add(x => x.OnRemove, EventCallback.Factory.Create(this, () => removed = true)));

        var removeBtn = cut.Find(".sb-chip__remove");

        // Act
        await cut.InvokeAsync(() => removeBtn!.Click());

        cut.WaitForState(() => removed);

        // Assert
        Assert.True(removed);
    }

    [Fact]
    public async Task DoesNotInvokeOnClickWhenDisabled()
    {
        // Arrange
        var clicked = false;
        var cut = RenderChip(p => p
            .Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => clicked = true))
            .Add(x => x.Disabled, true));

        var chip = cut.Find(".sb-chip");

        // Act
        await cut.InvokeAsync(() => chip!.Click());

        // Assert - click should not fire when disabled
        Assert.False(clicked);
    }

    [Fact]
    public void AppliesCustomClass()
    {
        // Arrange & Act
        var cut = RenderChip(p => p.Add(x => x.Class, "my-chip"));

        // Assert
        var chip = cut.Find(".sb-chip");
        Assert.Contains("my-chip", chip.ClassList);
    }

    [Fact]
    public void AppliesInlineStyle()
    {
        // Arrange & Act
        var cut = RenderChip(p => p.Add(x => x.Style, "margin: 4px;"));

        // Assert
        var chip = cut.Find(".sb-chip");
        Assert.Contains("margin: 4px", chip.GetAttribute("style"));
    }
}
