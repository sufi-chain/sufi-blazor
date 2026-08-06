using Microsoft.AspNetCore.Components;
using Bunit;
using SufiChain.SufiBlazor.Components;
using SufiChain.SufiBlazor.Components.Forms;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Forms;

public class SbCheckboxTests : BunitContext
{
    private IRenderedComponent<SbCheckbox> RenderCheckbox(
        Action<ComponentParameterCollectionBuilder<SbCheckbox>>? configure = null)
    {
        return Render<SbCheckbox>(p =>
        {
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersCheckboxStructure()
    {
        // Arrange & Act
        var cut = RenderCheckbox();

        // Assert
        var label = cut.Find("label.sb-checkbox");
        Assert.NotNull(label);
        Assert.NotNull(cut.Find("input.sb-checkbox__input"));
        Assert.NotNull(cut.Find(".sb-checkbox__box"));
    }

    [Fact]
    public void RendersLabelWhenProvided()
    {
        // Arrange & Act
        var cut = RenderCheckbox(p => p.Add(x => x.Label, "Accept terms"));

        // Assert
        var labelSpan = cut.Find(".sb-checkbox__label");
        Assert.NotNull(labelSpan);
        Assert.Contains("Accept terms", labelSpan.TextContent);
    }

    [Fact]
    public void DoesNotRenderLabelSpanWhenLabelAndChildContentEmpty()
    {
        // Arrange & Act
        var cut = RenderCheckbox();

        // Assert - no label span when both Label and ChildContent are null/empty
        Assert.Empty(cut.FindAll(".sb-checkbox__label"));
    }

    [Fact]
    public void RendersChildContentInsteadOfLabelWhenBothProvided()
    {
        // Arrange & Act
        var cut = Render<SbCheckbox>(p => p
            .Add(x => x.Label, "Ignore me")
            .AddChildContent(b => b.AddMarkupContent(0, "<span>Custom content</span>")));

        // Assert - ChildContent takes precedence
        var labelSpan = cut.Find(".sb-checkbox__label");
        Assert.NotNull(labelSpan);
        Assert.Contains("Custom content", labelSpan.InnerHtml);
    }

    [Fact]
    public void RendersCheckedClassAndCheckIconWhenValueTrue()
    {
        // Arrange & Act
        var cut = RenderCheckbox(p => p.Add(x => x.Value, true));

        // Assert
        var label = cut.Find("label.sb-checkbox");
        Assert.Contains("sb-checkbox--checked", label.ClassList);
        Assert.NotNull(cut.Find(".sb-checkbox__check"));
    }

    [Fact]
    public void RendersIndeterminateClassAndIconWhenIndeterminateTrue()
    {
        // Arrange & Act
        var cut = RenderCheckbox(p => p
            .Add(x => x.Value, false)
            .Add(x => x.Indeterminate, true));

        // Assert
        var label = cut.Find("label.sb-checkbox");
        Assert.Contains("sb-checkbox--indeterminate", label.ClassList);
        Assert.NotNull(cut.Find(".sb-checkbox__indeterminate"));
    }

    [Fact]
    public void RendersDisabledClassWhenDisabledTrue()
    {
        // Arrange & Act
        var cut = RenderCheckbox(p => p.Add(x => x.Disabled, true));

        // Assert
        var label = cut.Find("label.sb-checkbox");
        Assert.Contains("sb-checkbox--disabled", label.ClassList);
        var input = cut.Find("input");
        Assert.True(input.GetAttribute("disabled") != null || input.OuterHtml.Contains("disabled"));
    }

    [Fact]
    public void AppliesColorClass()
    {
        // Arrange & Act
        var cut = RenderCheckbox(p => p.Add(x => x.Color, SbColor.Success));

        // Assert
        var label = cut.Find("label.sb-checkbox");
        Assert.Contains("sb-checkbox--success", label.ClassList);
    }

    [Fact]
    public async Task InvokesValueChangedWhenChecked()
    {
        // Arrange
        var received = false;
        var cut = RenderCheckbox(p => p
            .Add(x => x.Value, false)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<bool>(this, v => received = v)));

        // Act - SbCheckbox uses @onchange
        var input = cut.Find("input.sb-checkbox__input");
        await cut.InvokeAsync(() => input!.Change(true));

        // Assert
        Assert.True(received);
    }

    [Fact]
    public async Task InvokesValueChangedWhenUnchecked()
    {
        // Arrange
        var received = true;
        var cut = RenderCheckbox(p => p
            .Add(x => x.Value, true)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<bool>(this, v => received = v)));

        // Act
        var input = cut.Find("input.sb-checkbox__input");
        await cut.InvokeAsync(() => input!.Change(false));

        // Assert
        Assert.False(received);
    }

    [Fact]
    public async Task DoesNotInvokeValueChangedWhenDisabled()
    {
        // Arrange
        var received = (bool?)null;
        var cut = RenderCheckbox(p => p
            .Add(x => x.Value, false)
            .Add(x => x.Disabled, true)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<bool>(this, v => received = v)));

        // Act
        var input = cut.Find("input.sb-checkbox__input");
        await cut.InvokeAsync(() => input!.Change(true));

        // Assert - callback should not be invoked when disabled
        Assert.Null(received);
    }

    [Fact]
    public void HasCorrectAriaCheckedWhenChecked()
    {
        // Arrange & Act
        var cut = RenderCheckbox(p => p.Add(x => x.Value, true));

        // Assert
        var input = cut.Find("input");
        Assert.Equal("true", input.GetAttribute("aria-checked"));
    }

    [Fact]
    public void HasCorrectAriaCheckedWhenUnchecked()
    {
        // Arrange & Act
        var cut = RenderCheckbox(p => p.Add(x => x.Value, false));

        // Assert
        var input = cut.Find("input");
        Assert.Equal("false", input.GetAttribute("aria-checked"));
    }

    [Fact]
    public void HasAriaCheckedMixedWhenIndeterminate()
    {
        // Arrange & Act
        var cut = RenderCheckbox(p => p
            .Add(x => x.Value, false)
            .Add(x => x.Indeterminate, true));

        // Assert
        var input = cut.Find("input");
        Assert.Equal("mixed", input.GetAttribute("aria-checked"));
    }

    [Fact]
    public void AppliesCustomClass()
    {
        // Arrange & Act
        var cut = RenderCheckbox(p => p.Add(x => x.Class, "my-checkbox"));

        // Assert
        var label = cut.Find("label.sb-checkbox");
        Assert.Contains("my-checkbox", label.ClassList);
    }

    [Fact]
    public void AppliesInlineStyle()
    {
        // Arrange & Act
        var cut = RenderCheckbox(p => p.Add(x => x.Style, "margin: 8px;"));

        // Assert
        var label = cut.Find("label.sb-checkbox");
        Assert.Contains("margin: 8px", label.GetAttribute("style"));
    }

    [Fact]
    public void AppliesIdToInput()
    {
        // Arrange & Act
        var cut = RenderCheckbox(p => p.Add(x => x.Id, "agree-checkbox"));

        // Assert
        var input = cut.Find("input");
        Assert.Equal("agree-checkbox", input.GetAttribute("id"));
    }

    [Fact]
    public void AppliesAdditionalAttributes()
    {
        // Arrange & Act
        var cut = RenderCheckbox(p => p.Add(x => x.AdditionalAttributes, new Dictionary<string, object>
        {
            { "data-testid", "accept-checkbox" },
            { "aria-label", "Accept terms and conditions" }
        }));

        // Assert
        var input = cut.Find("input");
        Assert.Equal("accept-checkbox", input.GetAttribute("data-testid"));
        Assert.Equal("Accept terms and conditions", input.GetAttribute("aria-label"));
    }
}
