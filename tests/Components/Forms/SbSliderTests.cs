using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Bunit;
using Bunit.JSInterop;
using SufiChain.SufiBlazor.Components.Forms;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Forms;

public class SbSliderTests : BunitContext
{
    public SbSliderTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<SbSlider<int>> RenderSlider(
        Action<ComponentParameterCollectionBuilder<SbSlider<int>>>? configure = null)
    {
        return Render<SbSlider<int>>(p =>
        {
            p.Add(x => x.Min, 0);
            p.Add(x => x.Max, 100);
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersSliderStructure()
    {
        // Arrange & Act
        var cut = RenderSlider();

        // Assert
        var wrapper = cut.Find(".sb-slider");
        Assert.NotNull(wrapper);
        Assert.NotNull(cut.Find(".sb-slider__track-container"));
        Assert.NotNull(cut.Find(".sb-slider__track"));
        Assert.NotNull(cut.Find(".sb-slider__fill"));
        Assert.NotNull(cut.Find(".sb-slider__thumb"));
    }

    [Fact]
    public void RendersValueWhenShowValueTrue()
    {
        // Arrange & Act
        var cut = RenderSlider(p => p.Add(x => x.Value, 50));

        // Assert
        var valueSpan = cut.Find(".sb-slider__value");
        Assert.NotNull(valueSpan);
        Assert.Equal("50", valueSpan.TextContent);
    }

    [Fact]
    public void DoesNotRenderValueWhenShowValueFalse()
    {
        // Arrange & Act
        var cut = RenderSlider(p => p
            .Add(x => x.Value, 50)
            .Add(x => x.ShowValue, false));

        // Assert
        Assert.Empty(cut.FindAll(".sb-slider__value"));
    }

    [Fact]
    public void ThumbHasSliderRoleAndAriaAttributes()
    {
        // Arrange & Act
        var cut = RenderSlider(p => p.Add(x => x.Value, 25));

        // Assert
        var thumb = cut.Find(".sb-slider__thumb");
        Assert.Equal("slider", thumb.GetAttribute("role"));
        Assert.Equal("25", thumb.GetAttribute("aria-valuenow"));
        Assert.Equal("0", thumb.GetAttribute("aria-valuemin"));
        Assert.Equal("100", thumb.GetAttribute("aria-valuemax"));
        Assert.Equal("0", thumb.GetAttribute("tabindex"));
    }

    [Fact]
    public void FillAndThumbReflectValuePosition()
    {
        // Arrange & Act - value 50 on 0-100 = 50%
        var cut = RenderSlider(p => p.Add(x => x.Value, 50));

        // Assert
        var fill = cut.Find(".sb-slider__fill");
        Assert.Contains("50", fill.GetAttribute("style")!);
        var thumb = cut.Find(".sb-slider__thumb");
        Assert.Contains("50", thumb.GetAttribute("style")!);
    }

    [Fact]
    public void RendersDisabledClassWhenDisabledTrue()
    {
        // Arrange & Act
        var cut = RenderSlider(p => p.Add(x => x.Disabled, true));

        // Assert
        var wrapper = cut.Find(".sb-slider");
        Assert.Contains("sb-slider--disabled", wrapper.ClassList);
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderSlider(p => p.Add(x => x.Class, "my-slider"));

        // Assert
        var wrapper = cut.Find(".sb-slider");
        Assert.Contains("my-slider", wrapper.ClassList);
    }

    [Fact]
    public void AppliesStyleParameter()
    {
        // Arrange & Act
        var cut = RenderSlider(p => p.Add(x => x.Style, "width: 200px;"));

        // Assert
        var wrapper = cut.Find(".sb-slider");
        Assert.Contains("width: 200px", wrapper.GetAttribute("style"));
    }

    [Fact]
    public void AppliesIdToThumb()
    {
        // Arrange & Act
        var cut = RenderSlider(p => p.Add(x => x.Id, "volume-slider"));

        // Assert
        var thumb = cut.Find(".sb-slider__thumb");
        Assert.Equal("volume-slider", thumb.GetAttribute("id"));
    }

    [Fact]
    public async Task InvokesValueChangedWhenArrowRightPressed()
    {
        // Arrange
        int? received = null;
        var cut = RenderSlider(p => p
            .Add(x => x.Value, 10)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<int?>(this, v => received = v)));

        var thumb = cut.Find(".sb-slider__thumb");

        // Act
        await cut.InvokeAsync(() =>
            thumb.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "ArrowRight" }));

        // Assert
        Assert.Equal(11, received);
    }

    [Fact]
    public async Task InvokesValueChangedWhenArrowLeftPressed()
    {
        // Arrange
        int? received = null;
        var cut = RenderSlider(p => p
            .Add(x => x.Value, 20)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<int?>(this, v => received = v)));

        var thumb = cut.Find(".sb-slider__thumb");

        // Act
        await cut.InvokeAsync(() =>
            thumb.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "ArrowLeft" }));

        // Assert
        Assert.Equal(19, received);
    }

    [Fact]
    public async Task InvokesValueChangedWhenHomePressed()
    {
        // Arrange
        int? received = null;
        var cut = RenderSlider(p => p
            .Add(x => x.Value, 75)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<int?>(this, v => received = v)));

        var thumb = cut.Find(".sb-slider__thumb");

        // Act
        await cut.InvokeAsync(() =>
            thumb.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "Home" }));

        // Assert
        Assert.Equal(0, received);
    }

    [Fact]
    public async Task InvokesValueChangedWhenEndPressed()
    {
        // Arrange
        int? received = null;
        var cut = RenderSlider(p => p
            .Add(x => x.Value, 25)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<int?>(this, v => received = v)));

        var thumb = cut.Find(".sb-slider__thumb");

        // Act
        await cut.InvokeAsync(() =>
            thumb.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "End" }));

        // Assert
        Assert.Equal(100, received);
    }

    [Fact]
    public async Task RespectsStepWhenArrowKeyPressed()
    {
        // Arrange
        int? received = null;
        var cut = RenderSlider(p => p
            .Add(x => x.Value, 10)
            .Add(x => x.Step, 5)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<int?>(this, v => received = v)));

        var thumb = cut.Find(".sb-slider__thumb");

        // Act
        await cut.InvokeAsync(() =>
            thumb.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "ArrowRight" }));

        // Assert
        Assert.Equal(15, received);
    }

    [Fact]
    public async Task ClampsValueAtMaxWhenArrowRightAtMax()
    {
        // Arrange
        int? received = null;
        var cut = RenderSlider(p => p
            .Add(x => x.Value, 100)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<int?>(this, v => received = v)));

        var thumb = cut.Find(".sb-slider__thumb");

        // Act
        await cut.InvokeAsync(() =>
            thumb.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "ArrowRight" }));

        // Assert
        Assert.Equal(100, received);
    }

    [Fact]
    public async Task ClampsValueAtMinWhenArrowLeftAtMin()
    {
        // Arrange
        int? received = null;
        var cut = RenderSlider(p => p
            .Add(x => x.Value, 0)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<int?>(this, v => received = v)));

        var thumb = cut.Find(".sb-slider__thumb");

        // Act
        await cut.InvokeAsync(() =>
            thumb.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "ArrowLeft" }));

        // Assert
        Assert.Equal(0, received);
    }

    [Fact]
    public async Task DoesNotInvokeValueChangedWhenDisabledAndKeyPressed()
    {
        // Arrange
        int? received = null;
        var cut = RenderSlider(p => p
            .Add(x => x.Value, 50)
            .Add(x => x.Disabled, true)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<int?>(this, v => received = v)));

        var thumb = cut.Find(".sb-slider__thumb");

        // Act
        await cut.InvokeAsync(() =>
            thumb.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "ArrowRight" }));

        // Assert
        Assert.Null(received);
    }

    [Fact]
    public void UsesMinWhenValueNull()
    {
        // Arrange & Act - Value null uses Min for CurrentValueDouble
        var cut = RenderSlider();

        // Assert
        var thumb = cut.Find(".sb-slider__thumb");
        Assert.Equal("0", thumb.GetAttribute("aria-valuenow"));
    }

    [Fact]
    public void FormatsDecimalValueWithLimitedDecimals()
    {
        // Arrange & Act
        var cut = Render<SbSlider<double>>(p => p
            .Add(x => x.Min, 0)
            .Add(x => x.Max, 10)
            .Add(x => x.Value, 3.5));

        // Assert
        var valueSpan = cut.Find(".sb-slider__value");
        Assert.NotNull(valueSpan);
        Assert.Contains("3.5", valueSpan.TextContent);
    }
}
