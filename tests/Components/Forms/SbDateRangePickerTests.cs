using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Bunit;
using Bunit.JSInterop;
using SufiChain.SufiBlazor.Components.Forms;
using SufiChain.SufiBlazor.Localization;
using SufiChain.SufiBlazor.Utilities.DateUtils;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Forms;

/// <summary>
/// Stub localizer for SbDateRangePicker tests.
/// </summary>
file class StubStringLocalizer : IStringLocalizer<SufiBlazorResource>
{
    public LocalizedString this[string name] => new(name, name);
    public LocalizedString this[string name, params object[] arguments] => new(name, string.Format(CultureInfo.InvariantCulture, name, arguments));
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => Array.Empty<LocalizedString>();
}

public class SbDateRangePickerTests : BunitContext
{
    public SbDateRangePickerTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new StubStringLocalizer());
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<SbDateRangePicker> RenderDateRangePicker(
        Action<ComponentParameterCollectionBuilder<SbDateRangePicker>>? configure = null)
    {
        return Render<SbDateRangePicker>(p =>
        {
            p.Add(x => x.CalendarSystem, SbCalendarSystem.Gregorian); // Predictable calendar
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersDateRangePickerStructure()
    {
        // Arrange & Act
        var cut = RenderDateRangePicker();

        // Assert
        var wrapper = cut.Find(".sb-daterangepicker");
        Assert.NotNull(wrapper);
        Assert.NotNull(cut.Find(".sb-daterangepicker__trigger"));
        Assert.NotNull(cut.Find(".sb-daterangepicker__placeholder"));
        Assert.NotNull(cut.Find(".sb-daterangepicker__icon"));
    }

    [Fact]
    public void RendersLabelWhenProvided()
    {
        // Arrange & Act
        var cut = RenderDateRangePicker(p => p.Add(x => x.Label, "Date range"));

        // Assert
        var label = cut.Find(".sb-daterangepicker__label");
        Assert.NotNull(label);
        Assert.Contains("Date range", label.TextContent);
    }

    [Fact]
    public void DoesNotRenderLabelWhenEmpty()
    {
        // Arrange & Act
        var cut = RenderDateRangePicker();

        // Assert
        Assert.Empty(cut.FindAll(".sb-daterangepicker__label"));
    }

    [Fact]
    public void RendersRequiredAsteriskWhenRequired()
    {
        // Arrange & Act
        var cut = RenderDateRangePicker(p => p
            .Add(x => x.Label, "Range")
            .Add(x => x.Required, true));

        // Assert
        var required = cut.Find(".sb-daterangepicker__required");
        Assert.NotNull(required);
        Assert.Contains("*", required.TextContent);
    }

    [Fact]
    public void DisplaysValueWhenValueSet()
    {
        // Arrange
        var range = new SbDateRange(
            new DateOnly(2025, 3, 10),
            new DateOnly(2025, 3, 25));

        // Act
        var cut = RenderDateRangePicker(p => p.Add(x => x.Value, range));

        // Assert
        var valueSpan = cut.Find(".sb-daterangepicker__value");
        Assert.NotNull(valueSpan);
        Assert.Contains("10", valueSpan.TextContent);
        Assert.Contains("25", valueSpan.TextContent);
        Assert.Contains("2025", valueSpan.TextContent);
    }

    [Fact]
    public void UsesLocalizedPlaceholderWhenPlaceholderNull()
    {
        // Arrange & Act
        var cut = RenderDateRangePicker();

        // Assert - StubStringLocalizer returns key as value
        var placeholderSpan = cut.Find(".sb-daterangepicker__placeholder");
        Assert.NotNull(placeholderSpan);
        Assert.Equal("SelectDateRange_Placeholder", placeholderSpan.TextContent);
    }

    [Fact]
    public void RendersPlaceholderWhenProvided()
    {
        // Arrange & Act
        var cut = RenderDateRangePicker(p => p.Add(x => x.Placeholder, "Pick a range..."));

        // Assert
        var placeholderSpan = cut.Find(".sb-daterangepicker__placeholder");
        Assert.NotNull(placeholderSpan);
        Assert.Equal("Pick a range...", placeholderSpan.TextContent);
    }

    [Fact]
    public void TriggerHasDisabledClassWhenDisabledTrue()
    {
        // Arrange & Act
        var cut = RenderDateRangePicker(p => p.Add(x => x.Disabled, true));

        // Assert
        var trigger = cut.Find(".sb-daterangepicker__trigger");
        Assert.Contains("sb-daterangepicker__trigger--disabled", trigger.ClassList);
    }

    [Fact]
    public void DoesNotRenderClearButtonWhenValueEmpty()
    {
        // Arrange & Act
        var cut = RenderDateRangePicker();

        // Assert
        Assert.Empty(cut.FindAll(".sb-daterangepicker__clear"));
    }

    [Fact]
    public void RendersClearButtonWhenValueSetAndClearable()
    {
        // Arrange & Act
        var cut = RenderDateRangePicker(p => p
            .Add(x => x.Value, new SbDateRange(new DateOnly(2025, 1, 10), new DateOnly(2025, 1, 20)))
            .Add(x => x.Clearable, true));

        // Assert
        var clearBtn = cut.Find(".sb-daterangepicker__clear");
        Assert.NotNull(clearBtn);
        Assert.Equal("ClearDates", clearBtn.GetAttribute("aria-label"));
    }

    [Fact]
    public void DoesNotRenderClearButtonWhenClearableFalse()
    {
        // Arrange & Act
        var cut = RenderDateRangePicker(p => p
            .Add(x => x.Value, new SbDateRange(new DateOnly(2025, 1, 10), new DateOnly(2025, 1, 20)))
            .Add(x => x.Clearable, false));

        // Assert
        Assert.Empty(cut.FindAll(".sb-daterangepicker__clear"));
    }

    [Fact]
    public async Task InvokesValueChangedWhenClearClicked()
    {
        // Arrange
        SbDateRange? received = null;
        var cut = RenderDateRangePicker(p => p
            .Add(x => x.Value, new SbDateRange(new DateOnly(2025, 1, 10), new DateOnly(2025, 1, 20)))
            .Add(x => x.Clearable, true)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<SbDateRange?>(this, v => received = v)));

        // Act
        var clearBtn = cut.Find(".sb-daterangepicker__clear");
        await cut.InvokeAsync(() => clearBtn!.Click());

        // Assert
        Assert.Null(received);
    }

    [Fact]
    public async Task OpensDropdownWhenTriggerClicked()
    {
        // Arrange & Act
        var cut = RenderDateRangePicker();
        var trigger = cut.Find(".sb-daterangepicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        // Assert
        Assert.NotNull(cut.Find(".sb-daterangepicker__dropdown"));
        Assert.NotNull(cut.Find(".sb-daterangepicker__calendars"));
        Assert.True(cut.FindAll(".sb-daterangepicker__calendar").Count >= 2);
        Assert.NotNull(cut.Find(".sb-daterangepicker__footer"));
    }

    [Fact]
    public async Task DoesNotOpenWhenDisabled()
    {
        // Arrange & Act
        var cut = RenderDateRangePicker(p => p.Add(x => x.Disabled, true));
        var trigger = cut.Find(".sb-daterangepicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        // Assert
        Assert.Empty(cut.FindAll(".sb-daterangepicker__dropdown"));
    }

    [Fact]
    public async Task ApplyButtonDisabledUntilBothDatesSelected()
    {
        // Arrange - open with no value, click one day (sets start only)
        var cut = RenderDateRangePicker();
        var trigger = cut.Find(".sb-daterangepicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        // Act - select only start date (day 15 exists in any month)
        var day15 = cut.FindAll(".sb-daterangepicker__day")
            .FirstOrDefault(b => b.TextContent.Trim() == "15"
                && !b.ClassList.Contains("sb-daterangepicker__day--disabled")
                && !b.ClassList.Contains("sb-daterangepicker__day--other-month"));
        Assert.NotNull(day15);
        await cut.InvokeAsync(() => day15!.Click());

        // Assert - Apply should still be disabled (need both start and end)
        var applyBtn = cut.Find(".sb-daterangepicker__apply");
        Assert.NotNull(applyBtn);
        Assert.True(applyBtn.GetAttribute("disabled") != null || applyBtn.OuterHtml.Contains("disabled"));
    }

    [Fact]
    public async Task InvokesValueChangedWhenApplyClickedAfterRangeSelected()
    {
        // Arrange - Value with start only (March 1) so display shows March; we'll select end (March 10)
        SbDateRange? received = null;
        var cut = RenderDateRangePicker(p => p
            .Add(x => x.Value, new SbDateRange(new DateOnly(2025, 3, 1), null)) // Start only, shows March
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<SbDateRange?>(this, v => received = v)));

        // Act - open, select end date (day 10 completes the range Mar 1 - Mar 10), apply
        var trigger = cut.Find(".sb-daterangepicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        var day10 = cut.FindAll(".sb-daterangepicker__day")
            .FirstOrDefault(b => b.TextContent.Trim() == "10"
                && !b.ClassList.Contains("sb-daterangepicker__day--disabled")
                && !b.ClassList.Contains("sb-daterangepicker__day--other-month"));
        Assert.NotNull(day10);
        await cut.InvokeAsync(() => day10!.Click());

        var applyBtn = cut.Find(".sb-daterangepicker__apply");
        await cut.InvokeAsync(() => applyBtn!.Click());

        // Assert
        Assert.NotNull(received);
        Assert.Equal(new DateOnly(2025, 3, 1), received!.Start);
        Assert.Equal(new DateOnly(2025, 3, 10), received!.End);
    }

    [Fact]
    public async Task CancelClosesDropdownWithoutInvokingValueChanged()
    {
        // Arrange
        SbDateRange? received = null;
        var initialValue = new SbDateRange(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 15));
        var cut = RenderDateRangePicker(p => p
            .Add(x => x.Value, initialValue)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<SbDateRange?>(this, v => received = v)));

        // Act - open, change selection (click a day), then cancel
        var trigger = cut.Find(".sb-daterangepicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        var cancelBtn = cut.Find(".sb-daterangepicker__cancel");
        Assert.NotNull(cancelBtn);
        await cut.InvokeAsync(() => cancelBtn!.Click());

        // Assert - ValueChanged should not have been invoked
        Assert.Null(received);
        Assert.Empty(cut.FindAll(".sb-daterangepicker__dropdown"));
    }

    [Fact]
    public async Task PresetSelectFillsRangeAndApplyInvokesValueChanged()
    {
        // Arrange
        SbDateRange? received = null;
        var cut = RenderDateRangePicker(p => p
            .Add(x => x.ShowPresets, true)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<SbDateRange?>(this, v => received = v)));

        // Act - open, select "Last 7 days" preset, apply
        var trigger = cut.Find(".sb-daterangepicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        var presetSelect = cut.Find(".sb-daterangepicker__preset-select");
        Assert.NotNull(presetSelect);
        await cut.InvokeAsync(() => presetSelect!.Change("7"));

        var applyBtn = cut.Find(".sb-daterangepicker__apply");
        await cut.InvokeAsync(() => applyBtn!.Click());

        // Assert - Last 7 days preset: start = today - 7, end = today (8 days inclusive)
        Assert.NotNull(received);
        Assert.NotNull(received!.Start);
        Assert.NotNull(received!.End);
        Assert.True(received.End >= received.Start);
        var dayCount = received.End!.Value.DayNumber - received.Start!.Value.DayNumber + 1;
        Assert.True(dayCount is 7 or 8, $"Expected 7 or 8 days for Last 7 days preset, got {dayCount}");
    }

    [Fact]
    public async Task HidesPresetsWhenShowPresetsFalse()
    {
        // Arrange & Act
        var cut = RenderDateRangePicker(p => p.Add(x => x.ShowPresets, false));
        var trigger = cut.Find(".sb-daterangepicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        // Assert
        Assert.Empty(cut.FindAll(".sb-daterangepicker__presets"));
    }

    [Fact]
    public void AppliesCustomClass()
    {
        // Arrange & Act
        var cut = RenderDateRangePicker(p => p.Add(x => x.Class, "my-daterangepicker"));

        // Assert
        var wrapper = cut.Find(".sb-daterangepicker");
        Assert.Contains("my-daterangepicker", wrapper.ClassList);
    }

    [Fact]
    public void AppliesInlineStyle()
    {
        // Arrange & Act
        var cut = RenderDateRangePicker(p => p.Add(x => x.Style, "max-width: 500px;"));

        // Assert
        var wrapper = cut.Find(".sb-daterangepicker");
        Assert.Contains("max-width: 500px", wrapper.GetAttribute("style"));
    }

    [Fact]
    public void UsesCustomFormatWhenProvided()
    {
        // Arrange & Act
        var cut = RenderDateRangePicker(p => p
            .Add(x => x.Value, new SbDateRange(new DateOnly(2025, 12, 10), new DateOnly(2025, 12, 25)))
            .Add(x => x.Format, "yyyy-MM-dd"));

        // Assert
        var valueSpan = cut.Find(".sb-daterangepicker__value");
        Assert.NotNull(valueSpan);
        Assert.Contains("2025-12-10", valueSpan.TextContent);
        Assert.Contains("2025-12-25", valueSpan.TextContent);
    }
}
