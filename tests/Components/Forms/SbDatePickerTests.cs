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
/// Stub localizer for SbDatePicker tests.
/// </summary>
file class StubStringLocalizer : IStringLocalizer<SufiBlazorResource>
{
    public LocalizedString this[string name] => new(name, name);
    public LocalizedString this[string name, params object[] arguments] => new(name, string.Format(CultureInfo.InvariantCulture, name, arguments));
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => Array.Empty<LocalizedString>();
}

public class SbDatePickerTests : BunitContext
{
    public SbDatePickerTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new StubStringLocalizer());
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<SbDatePicker> RenderDatePicker(
        Action<ComponentParameterCollectionBuilder<SbDatePicker>>? configure = null)
    {
        return Render<SbDatePicker>(p =>
        {
            p.Add(x => x.CalendarSystem, SbCalendarSystem.Gregorian); // Predictable calendar for tests
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersDatePickerStructure()
    {
        // Arrange & Act
        var cut = RenderDatePicker();

        // Assert
        var wrapper = cut.Find(".sb-datepicker");
        Assert.NotNull(wrapper);
        Assert.NotNull(cut.Find(".sb-datepicker__trigger"));
        Assert.NotNull(cut.Find(".sb-datepicker__placeholder"));
        Assert.NotNull(cut.Find(".sb-datepicker__icon"));
    }

    [Fact]
    public void RendersLabelWhenProvided()
    {
        // Arrange & Act
        var cut = RenderDatePicker(p => p.Add(x => x.Label, "Start date"));

        // Assert
        var label = cut.Find(".sb-datepicker__label");
        Assert.NotNull(label);
        Assert.Contains("Start date", label.TextContent);
    }

    [Fact]
    public void DoesNotRenderLabelWhenEmpty()
    {
        // Arrange & Act
        var cut = RenderDatePicker();

        // Assert
        Assert.Empty(cut.FindAll(".sb-datepicker__label"));
    }

    [Fact]
    public void DisplaysValueWhenValueSet()
    {
        // Arrange - use a fixed date for predictable output
        var date = new DateOnly(2025, 3, 15);

        // Act
        var cut = RenderDatePicker(p => p.Add(x => x.Value, date));

        // Assert - value span shows formatted date
        var valueSpan = cut.Find(".sb-datepicker__value");
        Assert.NotNull(valueSpan);
        Assert.Contains("15", valueSpan.TextContent);
        Assert.Contains("2025", valueSpan.TextContent);
    }

    [Fact]
    public void UsesLocalizedPlaceholderWhenPlaceholderNull()
    {
        // Arrange & Act
        var cut = RenderDatePicker();

        // Assert - StubStringLocalizer returns key as value
        var placeholderSpan = cut.Find(".sb-datepicker__placeholder");
        Assert.NotNull(placeholderSpan);
        Assert.Equal("SelectDate_Placeholder", placeholderSpan.TextContent);
    }

    [Fact]
    public void RendersPlaceholderWhenProvided()
    {
        // Arrange & Act
        var cut = RenderDatePicker(p => p.Add(x => x.Placeholder, "Pick a date..."));

        // Assert
        var placeholderSpan = cut.Find(".sb-datepicker__placeholder");
        Assert.NotNull(placeholderSpan);
        Assert.Equal("Pick a date...", placeholderSpan.TextContent);
    }

    [Fact]
    public void TriggerHasDisabledClassWhenDisabledTrue()
    {
        // Arrange & Act
        var cut = RenderDatePicker(p => p.Add(x => x.Disabled, true));

        // Assert
        var trigger = cut.Find(".sb-datepicker__trigger");
        Assert.Contains("sb-datepicker__trigger--disabled", trigger.ClassList);
    }

    [Fact]
    public void DoesNotRenderClearButtonWhenValueEmpty()
    {
        // Arrange & Act
        var cut = RenderDatePicker();

        // Assert
        Assert.Empty(cut.FindAll(".sb-datepicker__clear"));
    }

    [Fact]
    public void RendersClearButtonWhenValueSetAndClearable()
    {
        // Arrange & Act
        var cut = RenderDatePicker(p => p
            .Add(x => x.Value, new DateOnly(2025, 1, 10))
            .Add(x => x.Clearable, true));

        // Assert
        var clearBtn = cut.Find(".sb-datepicker__clear");
        Assert.NotNull(clearBtn);
        Assert.Equal("ClearDate", clearBtn.GetAttribute("aria-label"));
    }

    [Fact]
    public void DoesNotRenderClearButtonWhenClearableFalse()
    {
        // Arrange & Act
        var cut = RenderDatePicker(p => p
            .Add(x => x.Value, new DateOnly(2025, 1, 10))
            .Add(x => x.Clearable, false));

        // Assert
        Assert.Empty(cut.FindAll(".sb-datepicker__clear"));
    }

    [Fact]
    public async Task InvokesValueChangedWhenClearClicked()
    {
        // Arrange
        DateOnly? received = null;
        var cut = RenderDatePicker(p => p
            .Add(x => x.Value, new DateOnly(2025, 1, 10))
            .Add(x => x.Clearable, true)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<DateOnly?>(this, v => received = v)));

        // Act
        var clearBtn = cut.Find(".sb-datepicker__clear");
        await cut.InvokeAsync(() => clearBtn!.Click());

        // Assert
        Assert.Null(received);
    }

    [Fact]
    public async Task OpensDropdownWhenTriggerClicked()
    {
        // Arrange & Act
        var cut = RenderDatePicker();
        var trigger = cut.Find(".sb-datepicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        // Assert
        Assert.NotNull(cut.Find(".sb-datepicker__dropdown"));
        Assert.NotNull(cut.Find(".sb-datepicker__header"));
        Assert.NotNull(cut.Find(".sb-datepicker__day-names"));
        Assert.NotNull(cut.Find(".sb-datepicker__days"));
        Assert.NotNull(cut.Find(".sb-datepicker__footer"));
    }

    [Fact]
    public async Task DoesNotOpenWhenDisabled()
    {
        // Arrange & Act
        var cut = RenderDatePicker(p => p.Add(x => x.Disabled, true));
        var trigger = cut.Find(".sb-datepicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        // Assert
        Assert.Empty(cut.FindAll(".sb-datepicker__dropdown"));
    }

    [Fact]
    public async Task InvokesValueChangedWhenTodayClicked()
    {
        // Arrange
        DateOnly? received = null;
        var cut = RenderDatePicker(p => p
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<DateOnly?>(this, v => received = v)));

        // Act - open dropdown, click Today button
        var trigger = cut.Find(".sb-datepicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        var todayBtn = cut.Find(".sb-datepicker__today-btn");
        Assert.NotNull(todayBtn);
        await cut.InvokeAsync(() => todayBtn!.Click());

        // Assert
        Assert.NotNull(received);
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today), received!.Value);
    }

    [Fact]
    public async Task InvokesValueChangedWhenDaySelected()
    {
        // Arrange - use a fixed month so we can find a specific day
        DateOnly? received = null;
        var cut = RenderDatePicker(p => p
            .Add(x => x.Value, new DateOnly(2025, 3, 1)) // March 2025 - display will show this month
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<DateOnly?>(this, v => received = v)));

        // Act - open dropdown, click day 15 (guaranteed to exist in March)
        var trigger = cut.Find(".sb-datepicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        var day15 = cut.FindAll(".sb-datepicker__day")
            .FirstOrDefault(b => b.TextContent.Trim() == "15"
                && !b.ClassList.Contains("sb-datepicker__day--disabled")
                && !b.ClassList.Contains("sb-datepicker__day--other-month"));
        Assert.NotNull(day15);
        await cut.InvokeAsync(() => day15!.Click());

        // Assert
        Assert.NotNull(received);
        Assert.Equal(new DateOnly(2025, 3, 15), received!.Value);
    }

    [Fact]
    public async Task NavigatesToPreviousMonth()
    {
        // Arrange
        var cut = RenderDatePicker(p => p.Add(x => x.Value, new DateOnly(2025, 3, 15)));
        var trigger = cut.Find(".sb-datepicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        var title = cut.Find(".sb-datepicker__month-year");
        var initialTitle = title.TextContent;

        // Act - click previous month
        var prevBtn = cut.FindAll(".sb-datepicker__nav-btn").First();
        await cut.InvokeAsync(() => prevBtn!.Click());

        // Assert - title should have changed (e.g. Feb 2025)
        var newTitle = cut.Find(".sb-datepicker__month-year").TextContent;
        Assert.NotEqual(initialTitle, newTitle);
    }

    [Fact]
    public async Task SwitchesToMonthViewWhenMonthYearClicked()
    {
        // Arrange
        var cut = RenderDatePicker();
        var trigger = cut.Find(".sb-datepicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        // Act - click month/year header to switch to months view
        var monthYearBtn = cut.Find(".sb-datepicker__month-year");
        await cut.InvokeAsync(() => monthYearBtn!.Click());

        // Assert - months grid should be visible
        Assert.NotNull(cut.Find(".sb-datepicker__months"));
    }

    [Fact]
    public void AppliesCustomClass()
    {
        // Arrange & Act
        var cut = RenderDatePicker(p => p.Add(x => x.Class, "my-datepicker"));

        // Assert
        var wrapper = cut.Find(".sb-datepicker");
        Assert.Contains("my-datepicker", wrapper.ClassList);
    }

    [Fact]
    public void AppliesInlineStyle()
    {
        // Arrange & Act
        var cut = RenderDatePicker(p => p.Add(x => x.Style, "max-width: 220px;"));

        // Assert
        var wrapper = cut.Find(".sb-datepicker");
        Assert.Contains("max-width: 220px", wrapper.GetAttribute("style"));
    }

    [Fact]
    public void UsesCustomFormatWhenProvided()
    {
        // Arrange & Act
        var cut = RenderDatePicker(p => p
            .Add(x => x.Value, new DateOnly(2025, 12, 25))
            .Add(x => x.Format, "yyyy-MM-dd"));

        // Assert
        var valueSpan = cut.Find(".sb-datepicker__value");
        Assert.NotNull(valueSpan);
        Assert.Contains("2025-12-25", valueSpan.TextContent);
    }
}
