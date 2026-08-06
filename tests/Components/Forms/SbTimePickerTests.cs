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

file class StubStringLocalizer : IStringLocalizer<SufiBlazorResource>
{
    public LocalizedString this[string name] => new(name, name);
    public LocalizedString this[string name, params object[] arguments] => new(name, string.Format(CultureInfo.InvariantCulture, name, arguments));
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => Array.Empty<LocalizedString>();
}

public class SbTimePickerTests : BunitContext
{
    public SbTimePickerTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new StubStringLocalizer());
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<SbTimePicker> RenderTimePicker(
        Action<ComponentParameterCollectionBuilder<SbTimePicker>>? configure = null)
    {
        return Render<SbTimePicker>(p => configure?.Invoke(p));
    }

    [Fact]
    public void RendersTimePickerStructure()
    {
        // Arrange & Act
        var cut = RenderTimePicker();

        // Assert
        var wrapper = cut.Find(".sb-timepicker");
        Assert.NotNull(wrapper);
        Assert.NotNull(cut.Find(".sb-timepicker__trigger"));
        Assert.NotNull(cut.Find(".sb-timepicker__placeholder"));
        Assert.NotNull(cut.Find(".sb-timepicker__icon"));
    }

    [Fact]
    public void RendersLabelWhenProvided()
    {
        // Arrange & Act
        var cut = RenderTimePicker(p => p
            .Add(x => x.Label, "Start time")
            .Add(x => x.Id, "start"));

        // Assert
        var label = cut.Find(".sb-timepicker__label");
        Assert.NotNull(label);
        Assert.Contains("Start time", label.TextContent);
        Assert.Equal("start", label.GetAttribute("for"));
    }

    [Fact]
    public void DoesNotRenderLabelWhenEmpty()
    {
        // Arrange & Act
        var cut = RenderTimePicker();

        // Assert
        Assert.Empty(cut.FindAll(".sb-timepicker__label"));
    }

    [Fact]
    public void RendersRequiredIndicatorWhenRequiredTrue()
    {
        // Arrange & Act
        var cut = RenderTimePicker(p => p
            .Add(x => x.Label, "Time")
            .Add(x => x.Required, true));

        // Assert
        var required = cut.Find(".sb-timepicker__required");
        Assert.NotNull(required);
        Assert.Equal("*", required.TextContent);
    }

    [Fact]
    public void UsesLocalizedPlaceholderWhenPlaceholderNull()
    {
        // Arrange & Act
        var cut = RenderTimePicker();

        // Assert - StubStringLocalizer returns key as value
        var placeholder = cut.Find(".sb-timepicker__placeholder");
        Assert.NotNull(placeholder);
        Assert.Equal("SelectTime_Placeholder", placeholder.TextContent);
    }

    [Fact]
    public void RendersCustomPlaceholderWhenProvided()
    {
        // Arrange & Act
        var cut = RenderTimePicker(p => p.Add(x => x.Placeholder, "Choose time..."));

        // Assert
        var placeholder = cut.Find(".sb-timepicker__placeholder");
        Assert.NotNull(placeholder);
        Assert.Equal("Choose time...", placeholder.TextContent);
    }

    [Fact]
    public void DisplaysValueIn24HourFormat()
    {
        // Arrange & Act
        var time = new TimeOnly(14, 30, 0);
        var cut = RenderTimePicker(p => p.Add(x => x.Value, time));

        // Assert
        var valueSpan = cut.Find(".sb-timepicker__value");
        Assert.NotNull(valueSpan);
        Assert.Contains("14", valueSpan.TextContent);
        Assert.Contains("30", valueSpan.TextContent);
    }

    [Fact]
    public void DisplaysValueIn12HourFormatWhenUse24HourFalse()
    {
        // Arrange & Act
        var time = new TimeOnly(14, 30, 0);
        var cut = RenderTimePicker(p => p
            .Add(x => x.Value, time)
            .Add(x => x.Use24Hour, false));

        // Assert - 14:30 in 12h = 2:30 PM
        var valueSpan = cut.Find(".sb-timepicker__value");
        Assert.NotNull(valueSpan);
        Assert.Contains("PM", valueSpan.TextContent);
    }

    [Fact]
    public void DisplaysSecondsWhenShowSecondsTrue()
    {
        // Arrange & Act
        var time = new TimeOnly(9, 5, 45);
        var cut = RenderTimePicker(p => p
            .Add(x => x.Value, time)
            .Add(x => x.ShowSeconds, true));

        // Assert
        var valueSpan = cut.Find(".sb-timepicker__value");
        Assert.NotNull(valueSpan);
        Assert.Contains("45", valueSpan.TextContent);
    }

    [Fact]
    public void RendersClearButtonWhenClearableAndValueSet()
    {
        // Arrange & Act
        var cut = RenderTimePicker(p => p
            .Add(x => x.Value, new TimeOnly(10, 0, 0))
            .Add(x => x.Clearable, true));

        // Assert
        var clearBtn = cut.Find(".sb-timepicker__clear");
        Assert.NotNull(clearBtn);
        Assert.Equal("ClearTime", clearBtn.GetAttribute("aria-label"));
    }

    [Fact]
    public void DoesNotRenderClearButtonWhenValueEmpty()
    {
        // Arrange & Act
        var cut = RenderTimePicker(p => p.Add(x => x.Clearable, true));

        // Assert
        Assert.Empty(cut.FindAll(".sb-timepicker__clear"));
    }

    [Fact]
    public void TriggerHasDisabledClassWhenDisabledTrue()
    {
        // Arrange & Act
        var cut = RenderTimePicker(p => p.Add(x => x.Disabled, true));

        // Assert
        var trigger = cut.Find(".sb-timepicker__trigger");
        Assert.Contains("sb-timepicker__trigger--disabled", trigger.ClassList);
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderTimePicker(p => p.Add(x => x.Class, "my-timepicker"));

        // Assert
        var wrapper = cut.Find(".sb-timepicker");
        Assert.Contains("my-timepicker", wrapper.ClassList);
    }

    [Fact]
    public async Task OpensDropdownWhenTriggerClicked()
    {
        // Arrange & Act
        var cut = RenderTimePicker();
        var trigger = cut.Find(".sb-timepicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        // Assert
        Assert.NotNull(cut.Find(".sb-timepicker__dropdown"));
        Assert.NotNull(cut.Find(".sb-timepicker__selectors"));
        Assert.NotNull(cut.Find(".sb-timepicker__footer"));
    }

    [Fact]
    public async Task DoesNotOpenWhenDisabled()
    {
        // Arrange & Act
        var cut = RenderTimePicker(p => p.Add(x => x.Disabled, true));
        var trigger = cut.Find(".sb-timepicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        // Assert
        Assert.Empty(cut.FindAll(".sb-timepicker__dropdown"));
    }

    [Fact]
    public async Task InvokesValueChangedWhenApplyClicked()
    {
        // Arrange - preset Value so dropdown opens with 14:30 selected; Apply confirms it
        TimeOnly? received = null;
        var cut = RenderTimePicker(p => p
            .Add(x => x.Value, new TimeOnly(14, 30, 0))
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<TimeOnly?>(this, v => received = v)));

        var trigger = cut.Find(".sb-timepicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        var applyBtn = cut.Find(".sb-timepicker__apply-btn");
        await cut.InvokeAsync(() => applyBtn!.Click());

        // Assert
        Assert.NotNull(received);
        Assert.Equal(14, received!.Value.Hour);
        Assert.Equal(30, received.Value.Minute);
    }

    [Fact]
    public async Task InvokesValueChangedWhenNowClicked()
    {
        // Arrange
        TimeOnly? received = null;
        var cut = RenderTimePicker(p => p
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<TimeOnly?>(this, v => received = v)));

        var trigger = cut.Find(".sb-timepicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        var nowBtn = cut.Find(".sb-timepicker__now-btn");
        Assert.NotNull(nowBtn);
        await cut.InvokeAsync(() => nowBtn!.Click());

        // Assert
        Assert.NotNull(received);
        var expected = TimeOnly.FromDateTime(DateTime.Now);
        Assert.Equal(expected.Hour, received!.Value.Hour);
        Assert.Equal(expected.Minute, received.Value.Minute);
    }

    [Fact]
    public async Task InvokesValueChangedNullWhenClearClicked()
    {
        // Arrange
        TimeOnly? received = new TimeOnly(10, 30, 0);
        var cut = RenderTimePicker(p => p
            .Add(x => x.Value, new TimeOnly(10, 30, 0))
            .Add(x => x.Clearable, true)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<TimeOnly?>(this, v => received = v)));

        var clearBtn = cut.Find(".sb-timepicker__clear");
        await cut.InvokeAsync(() => clearBtn!.Click());

        // Assert
        Assert.Null(received);
    }

    [Fact]
    public async Task ShowsPeriodColumnWhenUse24HourFalse()
    {
        // Arrange & Act
        var cut = RenderTimePicker(p => p.Add(x => x.Use24Hour, false));
        var trigger = cut.Find(".sb-timepicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        // Assert - AM/PM options
        var periodOptions = cut.FindAll(".sb-timepicker__option")
            .Where(o => o.TextContent.Trim() is "AM" or "PM").ToList();
        Assert.Equal(2, periodOptions.Count);
    }

    [Fact]
    public async Task InvokesValueChangedWithCorrectTimeWhen12HourAndPMSelected()
    {
        // Arrange
        TimeOnly? received = null;
        var cut = RenderTimePicker(p => p
            .Add(x => x.Use24Hour, false)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<TimeOnly?>(this, v => received = v)));

        var trigger = cut.Find(".sb-timepicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        // Select hour 2, minute 30, PM
        var hour2 = cut.FindAll(".sb-timepicker__option").FirstOrDefault(b => b.TextContent.Trim() == "02");
        Assert.NotNull(hour2);
        await cut.InvokeAsync(() => hour2!.Click());

        var minute30 = cut.FindAll(".sb-timepicker__option").FirstOrDefault(b => b.TextContent.Trim() == "30");
        Assert.NotNull(minute30);
        await cut.InvokeAsync(() => minute30!.Click());

        var pmBtn = cut.FindAll(".sb-timepicker__option").FirstOrDefault(b => b.TextContent.Trim() == "PM");
        Assert.NotNull(pmBtn);
        await cut.InvokeAsync(() => pmBtn!.Click());

        var applyBtn = cut.Find(".sb-timepicker__apply-btn");
        await cut.InvokeAsync(() => applyBtn!.Click());

        // Assert - 2:30 PM = 14:30
        Assert.NotNull(received);
        Assert.Equal(14, received!.Value.Hour);
        Assert.Equal(30, received.Value.Minute);
    }

    [Fact]
    public async Task ShowsSecondsColumnWhenShowSecondsTrue()
    {
        // Arrange & Act
        var cut = RenderTimePicker(p => p.Add(x => x.ShowSeconds, true));
        var trigger = cut.Find(".sb-timepicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        // Assert - column headers include Second
        var headers = cut.FindAll(".sb-timepicker__column-header");
        Assert.Contains(headers, h => h.TextContent.Contains("Second"));
    }

    [Fact]
    public async Task RespectsMinuteStep()
    {
        // Arrange - with MinuteStep 15: 4 minute options (00, 15, 30, 45). 24 hours = 28 total options.
        // With default step 1: 60 minute options. 24 + 60 = 84 total.
        var cut = RenderTimePicker(p => p.Add(x => x.MinuteStep, 15));
        var trigger = cut.Find(".sb-timepicker__trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        // Assert
        var allOptions = cut.FindAll(".sb-timepicker__option");
        Assert.Equal(28, allOptions.Count); // 24 hours + 4 minutes
    }
}
