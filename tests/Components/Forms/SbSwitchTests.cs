using Microsoft.AspNetCore.Components;
using Bunit;
using SufiChain.SufiBlazor.Components;
using SufiChain.SufiBlazor.Components.Forms;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Forms;

public class SbSwitchTests : BunitContext
{
    private IRenderedComponent<SbSwitch> RenderSwitch(
        Action<ComponentParameterCollectionBuilder<SbSwitch>>? configure = null)
    {
        return Render<SbSwitch>(p => configure?.Invoke(p));
    }

    [Fact]
    public void RendersSwitchStructure()
    {
        // Arrange & Act
        var cut = RenderSwitch();

        // Assert
        var label = cut.Find("label.sb-switch");
        Assert.NotNull(label);
        Assert.NotNull(cut.Find(".sb-switch__input-wrapper"));
        Assert.NotNull(cut.Find("input.sb-switch__input"));
        Assert.NotNull(cut.Find(".sb-switch__track"));
        Assert.NotNull(cut.Find(".sb-switch__thumb"));
    }

    [Fact]
    public void InputHasSwitchRole()
    {
        // Arrange & Act
        var cut = RenderSwitch();

        // Assert
        var input = cut.Find("input");
        Assert.Equal("switch", input.GetAttribute("role"));
        Assert.Equal("checkbox", input.GetAttribute("type"));
    }

    [Fact]
    public void RendersLabelWhenProvided()
    {
        // Arrange & Act
        var cut = RenderSwitch(p => p.Add(x => x.Label, "Enable notifications"));

        // Assert
        var labelSpan = cut.Find(".sb-switch__label");
        Assert.NotNull(labelSpan);
        Assert.Contains("Enable notifications", labelSpan.TextContent);
    }

    [Fact]
    public void DoesNotRenderTextSpanWhenLabelAndChildContentAndHelperEmpty()
    {
        // Arrange & Act
        var cut = RenderSwitch();

        // Assert - no sb-switch__text when Label, ChildContent, and EffectiveHelperText are all empty
        Assert.Empty(cut.FindAll(".sb-switch__text"));
    }

    [Fact]
    public void RendersChildContentInsteadOfLabelWhenBothProvided()
    {
        // Arrange & Act
        var cut = Render<SbSwitch>(p => p
            .Add(x => x.Label, "Ignore me")
            .AddChildContent(b => b.AddMarkupContent(0, "<span>Custom content</span>")));

        // Assert - ChildContent takes precedence
        var labelSpan = cut.Find(".sb-switch__label");
        Assert.NotNull(labelSpan);
        Assert.Contains("Custom content", labelSpan.InnerHtml);
    }

    [Fact]
    public void RendersCheckedClassWhenValueTrue()
    {
        // Arrange & Act
        var cut = RenderSwitch(p => p.Add(x => x.Value, true));

        // Assert
        var label = cut.Find("label.sb-switch");
        Assert.Contains("sb-switch--checked", label.ClassList);
    }

    [Fact]
    public void InputIsCheckedWhenValueTrue()
    {
        // Arrange & Act
        var cut = RenderSwitch(p => p.Add(x => x.Value, true));

        // Assert
        var input = cut.Find("input");
        Assert.NotNull(input.GetAttribute("checked"));
    }

    [Fact]
    public void RendersDisabledClassWhenDisabledTrue()
    {
        // Arrange & Act
        var cut = RenderSwitch(p => p.Add(x => x.Disabled, true));

        // Assert
        var label = cut.Find("label.sb-switch");
        Assert.Contains("sb-switch--disabled", label.ClassList);
        var input = cut.Find("input");
        Assert.NotNull(input.GetAttribute("disabled"));
    }

    [Fact]
    public void AppliesColorClass()
    {
        // Arrange & Act
        var cut = RenderSwitch(p => p.Add(x => x.Color, SbColor.Success));

        // Assert
        var label = cut.Find("label.sb-switch");
        Assert.Contains("sb-switch--success", label.ClassList);
    }

    [Fact]
    public void AppliesSizeClass()
    {
        // Arrange & Act
        var cut = RenderSwitch(p => p.Add(x => x.Size, SbSize.Lg));

        // Assert
        var label = cut.Find("label.sb-switch");
        Assert.Contains("sb-switch--lg", label.ClassList);
    }

    [Fact]
    public async Task InvokesValueChangedWhenChecked()
    {
        // Arrange
        var received = false;
        var cut = RenderSwitch(p => p
            .Add(x => x.Value, false)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<bool>(this, v => received = v)));

        // Act
        var input = cut.Find("input.sb-switch__input");
        await cut.InvokeAsync(() => input!.Change(true));

        // Assert
        Assert.True(received);
    }

    [Fact]
    public async Task InvokesValueChangedWhenUnchecked()
    {
        // Arrange
        var received = true;
        var cut = RenderSwitch(p => p
            .Add(x => x.Value, true)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<bool>(this, v => received = v)));

        // Act
        var input = cut.Find("input.sb-switch__input");
        await cut.InvokeAsync(() => input!.Change(false));

        // Assert
        Assert.False(received);
    }

    [Fact]
    public async Task DoesNotInvokeValueChangedWhenDisabled()
    {
        // Arrange
        var received = (bool?)null;
        var cut = RenderSwitch(p => p
            .Add(x => x.Value, false)
            .Add(x => x.Disabled, true)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<bool>(this, v => received = v)));

        // Act
        var input = cut.Find("input.sb-switch__input");
        await cut.InvokeAsync(() => input!.Change(true));

        // Assert
        Assert.Null(received);
    }

    [Fact]
    public void HasCorrectAriaCheckedWhenChecked()
    {
        // Arrange & Act
        var cut = RenderSwitch(p => p.Add(x => x.Value, true));

        // Assert
        var input = cut.Find("input");
        Assert.Equal("true", input.GetAttribute("aria-checked"));
    }

    [Fact]
    public void HasCorrectAriaCheckedWhenUnchecked()
    {
        // Arrange & Act
        var cut = RenderSwitch(p => p.Add(x => x.Value, false));

        // Assert
        var input = cut.Find("input");
        Assert.Equal("false", input.GetAttribute("aria-checked"));
    }

    [Fact]
    public void RendersHelperTextWhenProvided()
    {
        // Arrange & Act
        var cut = RenderSwitch(p => p.Add(x => x.HelperText, "Toggle to enable or disable"));

        // Assert
        var helper = cut.Find(".sb-switch__helper");
        Assert.NotNull(helper);
        Assert.Contains("Toggle to enable or disable", helper.TextContent);
    }

    [Fact]
    public void RendersHelperTextFromAdditionalAttributes()
    {
        // Arrange & Act
        var cut = RenderSwitch(p => p.Add(x => x.AdditionalAttributes, new Dictionary<string, object>
        {
            { "HelperText", "From additional attributes" }
        }));

        // Assert
        var helper = cut.Find(".sb-switch__helper");
        Assert.NotNull(helper);
        Assert.Contains("From additional attributes", helper.TextContent);
    }

    [Fact]
    public void AppliesCustomClass()
    {
        // Arrange & Act
        var cut = RenderSwitch(p => p.Add(x => x.Class, "my-switch"));

        // Assert
        var label = cut.Find("label.sb-switch");
        Assert.Contains("my-switch", label.ClassList);
    }

    [Fact]
    public void AppliesInlineStyle()
    {
        // Arrange & Act
        var cut = RenderSwitch(p => p.Add(x => x.Style, "margin: 8px;"));

        // Assert
        var label = cut.Find("label.sb-switch");
        Assert.Contains("margin: 8px", label.GetAttribute("style"));
    }

    [Fact]
    public void AppliesIdToInput()
    {
        // Arrange & Act
        var cut = RenderSwitch(p => p.Add(x => x.Id, "notify-switch"));

        // Assert
        var input = cut.Find("input");
        Assert.Equal("notify-switch", input.GetAttribute("id"));
    }

    [Fact]
    public void AppliesAdditionalAttributes()
    {
        // Arrange & Act
        var cut = RenderSwitch(p => p.Add(x => x.AdditionalAttributes, new Dictionary<string, object>
        {
            { "data-testid", "notify-switch" },
            { "aria-label", "Enable email notifications" }
        }));

        // Assert
        var input = cut.Find("input");
        Assert.Equal("notify-switch", input.GetAttribute("data-testid"));
        Assert.Equal("Enable email notifications", input.GetAttribute("aria-label"));
    }
}
