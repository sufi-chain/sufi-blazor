using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Bunit;
using Bunit.JSInterop;
using SufiChain.SufiBlazor.Components.Forms;
using SufiChain.SufiBlazor.Localization;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Forms;

/// <summary>
/// Stub localizer for SbColorPicker tests.
/// </summary>
file class StubStringLocalizer : IStringLocalizer<SufiBlazorResource>
{
    public LocalizedString this[string name] => new(name, name);
    public LocalizedString this[string name, params object[] arguments] => new(name, string.Format(CultureInfo.InvariantCulture, name, arguments));
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => Array.Empty<LocalizedString>();
}

public class SbColorPickerTests : BunitContext
{
    public SbColorPickerTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new StubStringLocalizer());
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<SbColorPicker> RenderColorPicker(
        Action<ComponentParameterCollectionBuilder<SbColorPicker>>? configure = null)
    {
        return Render<SbColorPicker>(p =>
        {
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersColorPickerStructure()
    {
        // Arrange & Act
        var cut = RenderColorPicker();

        // Assert
        var wrapper = cut.Find(".sb-colorpicker");
        Assert.NotNull(wrapper);
        Assert.NotNull(cut.Find(".sb-colorpicker__trigger"));
        Assert.NotNull(cut.Find(".sb-colorpicker__preview"));
        Assert.NotNull(cut.Find(".sb-colorpicker__value"));
    }

    [Fact]
    public void RendersLabelWhenProvided()
    {
        // Arrange & Act
        var cut = RenderColorPicker(p => p.Add(x => x.Label, "Color"));

        // Assert
        var label = cut.Find(".sb-colorpicker__label");
        Assert.NotNull(label);
        Assert.Contains("Color", label.TextContent);
    }

    [Fact]
    public void DoesNotRenderLabelWhenEmpty()
    {
        // Arrange & Act
        var cut = RenderColorPicker();

        // Assert
        Assert.Empty(cut.FindAll(".sb-colorpicker__label"));
    }

    [Fact]
    public void RendersRequiredAsteriskWhenRequired()
    {
        // Arrange & Act
        var cut = RenderColorPicker(p => p
            .Add(x => x.Label, "Color")
            .Add(x => x.Required, true));

        // Assert
        var required = cut.Find(".sb-colorpicker__required");
        Assert.NotNull(required);
        Assert.Contains("*", required.TextContent);
    }

    [Fact]
    public void DisplaysValueWhenValueSet()
    {
        // Arrange & Act
        var cut = RenderColorPicker(p => p.Add(x => x.Value, "#ff0000"));

        // Assert
        var preview = cut.Find(".sb-colorpicker__preview");
        Assert.Contains("#ff0000", preview.GetAttribute("style"));
        var valueSpan = cut.Find(".sb-colorpicker__value");
        Assert.Contains("#ff0000", valueSpan.TextContent);
    }

    [Fact]
    public void UsesLocalizedPlaceholderWhenPlaceholderNull()
    {
        // Arrange & Act
        var cut = RenderColorPicker();

        // Assert - StubStringLocalizer returns key as value
        var valueSpan = cut.Find(".sb-colorpicker__value");
        Assert.Equal("SelectColor_Placeholder", valueSpan.TextContent);
    }

    [Fact]
    public void RendersPlaceholderWhenProvided()
    {
        // Arrange & Act
        var cut = RenderColorPicker(p => p.Add(x => x.Placeholder, "Pick a color..."));

        // Assert
        var valueSpan = cut.Find(".sb-colorpicker__value");
        Assert.Equal("Pick a color...", valueSpan.TextContent);
    }

    [Fact]
    public void TriggerHasDisabledClassWhenDisabledTrue()
    {
        // Arrange & Act
        var cut = RenderColorPicker(p => p.Add(x => x.Disabled, true));

        // Assert
        var trigger = cut.Find(".sb-colorpicker__trigger");
        Assert.Contains("sb-colorpicker__trigger--disabled", trigger.ClassList);
    }

    [Fact]
    public void DoesNotRenderClearButtonWhenValueEmpty()
    {
        // Arrange & Act
        var cut = RenderColorPicker();

        // Assert
        Assert.Empty(cut.FindAll(".sb-colorpicker__clear"));
    }

    [Fact]
    public void RendersClearButtonWhenValueSetAndClearable()
    {
        // Arrange & Act
        var cut = RenderColorPicker(p => p
            .Add(x => x.Value, "#ff0000")
            .Add(x => x.Clearable, true));

        // Assert
        var clearBtn = cut.Find(".sb-colorpicker__clear");
        Assert.NotNull(clearBtn);
        Assert.Equal("ClearColor", clearBtn.GetAttribute("aria-label"));
    }

    [Fact]
    public void DoesNotRenderClearButtonWhenClearableFalse()
    {
        // Arrange & Act
        var cut = RenderColorPicker(p => p
            .Add(x => x.Value, "#ff0000")
            .Add(x => x.Clearable, false));

        // Assert
        Assert.Empty(cut.FindAll(".sb-colorpicker__clear"));
    }

    [Fact]
    public async Task InvokesValueChangedWhenClearClicked()
    {
        // Arrange
        string? received = null;
        var cut = RenderColorPicker(p => p
            .Add(x => x.Value, "#ff0000")
            .Add(x => x.Clearable, true)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => received = v)));

        // Act
        var clearBtn = cut.Find(".sb-colorpicker__clear");
        await cut.InvokeAsync(() => clearBtn!.Click());

        // Assert
        Assert.Null(received);
    }

    [Fact]
    public async Task OpensDropdownWhenTriggerClicked()
    {
        // Arrange & Act
        var cut = RenderColorPicker();
        var trigger = cut.Find(".sb-colorpicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        // Assert
        Assert.NotNull(cut.Find(".sb-colorpicker__dropdown"));
        Assert.NotNull(cut.Find(".sb-colorpicker__presets"));
        Assert.NotNull(cut.Find(".sb-colorpicker__custom"));
    }

    [Fact]
    public void AppliesCustomClass()
    {
        // Arrange & Act
        var cut = RenderColorPicker(p => p.Add(x => x.Class, "my-colorpicker"));

        // Assert
        var wrapper = cut.Find(".sb-colorpicker");
        Assert.Contains("my-colorpicker", wrapper.ClassList);
    }

    [Fact]
    public void AppliesInlineStyle()
    {
        // Arrange & Act
        var cut = RenderColorPicker(p => p.Add(x => x.Style, "max-width: 200px;"));

        // Assert
        var wrapper = cut.Find(".sb-colorpicker");
        Assert.Contains("max-width: 200px", wrapper.GetAttribute("style"));
    }

    [Fact]
    public async Task DoesNotOpenWhenDisabled()
    {
        // Arrange & Act
        var cut = RenderColorPicker(p => p.Add(x => x.Disabled, true));
        var trigger = cut.Find(".sb-colorpicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        // Assert - Toggle returns early when Disabled
        Assert.Empty(cut.FindAll(".sb-colorpicker__dropdown"));
    }

    [Fact]
    public async Task HidesPresetsWhenShowPresetsFalse()
    {
        // Arrange & Act
        var cut = RenderColorPicker(p => p.Add(x => x.ShowPresets, false));
        var trigger = cut.Find(".sb-colorpicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        // Assert - presets section not rendered when ShowPresets is false and Presets is empty/default
        // When ShowPresets is false, the condition is: @if (ShowPresets && Presets.Any())
        // So no presets div
        Assert.Empty(cut.FindAll(".sb-colorpicker__presets"));
    }

    [Fact]
    public async Task ShowsOpacitySliderWhenShowOpacityTrue()
    {
        // Arrange & Act
        var cut = RenderColorPicker(p => p.Add(x => x.ShowOpacity, true));
        var trigger = cut.Find(".sb-colorpicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        // Assert
        Assert.NotNull(cut.Find(".sb-colorpicker__opacity"));
        var slider = cut.Find(".sb-colorpicker__opacity-slider");
        Assert.NotNull(slider);
        Assert.Equal("0", slider.GetAttribute("min"));
        Assert.Equal("100", slider.GetAttribute("max"));
    }

    [Fact]
    public async Task InvokesValueChangedWhenApplyClickedAfterPresetSelected()
    {
        // Arrange
        string? received = null;
        var presets = new[] { "#ef4444", "#22c55e" };
        var cut = RenderColorPicker(p => p
            .Add(x => x.Presets, presets)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => received = v)));

        // Act - open dropdown, select preset, apply
        var trigger = cut.Find(".sb-colorpicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        var presetBtn = cut.FindAll(".sb-colorpicker__preset")
            .FirstOrDefault(b => b.GetAttribute("style")?.Contains("#22c55e") == true);
        Assert.NotNull(presetBtn);
        await cut.InvokeAsync(() => presetBtn!.Click());

        var applyBtn = cut.FindAll("button")
            .FirstOrDefault(b => b.TextContent?.Trim() == "Apply");
        Assert.NotNull(applyBtn);
        await cut.InvokeAsync(() => applyBtn!.Click());

        // Assert
        Assert.Equal("#22c55e", received);
    }

    [Fact]
    public async Task RapidHexInputAppliesCompleteBrowserValue()
    {
        string? received = null;
        var cut = RenderColorPicker(p => p.Add(
            x => x.ValueChanged,
            EventCallback.Factory.Create<string?>(this, value => received = value)));

        await cut.InvokeAsync(() => cut.Find(".sb-colorpicker__trigger").Click());
        var input = cut.Find(".sb-colorpicker__hex");
        await cut.InvokeAsync(() => input.Input("1"));
        await cut.InvokeAsync(() => input.Input("12"));
        await cut.InvokeAsync(() => input.Input("123"));
        await cut.InvokeAsync(() => input.Input("123456"));

        var apply = cut.FindAll("button")
            .Single(button => button.TextContent.Trim() == "Apply");
        await cut.InvokeAsync(() => apply.Click());

        Assert.Equal("#123456", received);
    }

    [Fact]
    public async Task CancelClosesDropdownWithoutInvokingValueChanged()
    {
        // Arrange
        string? received = "unchanged";
        var cut = RenderColorPicker(p => p
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => received = v)));

        // Act - open, select preset, cancel
        var trigger = cut.Find(".sb-colorpicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        var presetBtn = cut.Find(".sb-colorpicker__preset");
        Assert.NotNull(presetBtn);
        await cut.InvokeAsync(() => presetBtn!.Click());

        var cancelBtn = cut.FindAll("button")
            .FirstOrDefault(b => b.TextContent?.Trim() == "Cancel");
        Assert.NotNull(cancelBtn);
        await cut.InvokeAsync(() => cancelBtn!.Click());

        // Assert - ValueChanged should not have been invoked (cancel discards changes)
        Assert.Equal("unchanged", received);
        Assert.Empty(cut.FindAll(".sb-colorpicker__dropdown"));
    }
}
