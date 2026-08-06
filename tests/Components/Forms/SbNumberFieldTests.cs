using Microsoft.AspNetCore.Components;
using Bunit;
using SufiChain.SufiBlazor.Components.Forms;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Forms;

public class SbNumberFieldTests : BunitContext
{
    private IRenderedComponent<SbNumberField<int>> RenderNumberField(
        Action<ComponentParameterCollectionBuilder<SbNumberField<int>>>? configure = null)
    {
        return Render<SbNumberField<int>>(p =>
        {
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersNumberFieldStructure()
    {
        // Arrange & Act
        var cut = RenderNumberField();

        // Assert
        var wrapper = cut.Find(".sb-number-field");
        Assert.NotNull(wrapper);
        Assert.NotNull(cut.Find(".sb-number-field__input-group"));
        var input = cut.Find("input.sb-number-field__input");
        Assert.NotNull(input);
        Assert.Equal("number", input.GetAttribute("type"));
    }

    [Fact]
    public void RendersLabelWhenProvided()
    {
        // Arrange & Act
        var cut = RenderNumberField(p => p
            .Add(x => x.Label, "Quantity")
            .Add(x => x.Id, "qty"));

        // Assert
        var label = cut.Find(".sb-number-field__label");
        Assert.NotNull(label);
        Assert.Contains("Quantity", label.TextContent);
    }

    [Fact]
    public void DoesNotRenderLabelWhenEmpty()
    {
        // Arrange & Act
        var cut = RenderNumberField();

        // Assert
        Assert.Empty(cut.FindAll(".sb-number-field__label"));
    }

    [Fact]
    public void RendersRequiredAsteriskWhenRequired()
    {
        // Arrange & Act
        var cut = RenderNumberField(p => p
            .Add(x => x.Label, "Amount")
            .Add(x => x.Required, true));

        // Assert
        var required = cut.Find(".sb-number-field__required");
        Assert.NotNull(required);
        Assert.Contains("*", required.TextContent);
    }

    [Fact]
    public void AssociatesLabelWithInputId()
    {
        // Arrange & Act
        var cut = RenderNumberField(p => p
            .Add(x => x.Label, "Value")
            .Add(x => x.Id, "value-input"));

        // Assert
        var label = cut.Find("label.sb-number-field__label");
        Assert.Equal("value-input", label.GetAttribute("for"));
    }

    [Fact]
    public void RendersHelperTextWhenProvided()
    {
        // Arrange & Act
        var cut = RenderNumberField(p => p.Add(x => x.HelperText, "Enter a number between 1 and 100"));

        // Assert
        var helper = cut.Find(".sb-number-field__helper");
        Assert.NotNull(helper);
        Assert.Contains("Enter a number between 1 and 100", helper.TextContent);
    }

    [Fact]
    public void DisplaysValueInInput()
    {
        // Arrange & Act
        var cut = RenderNumberField(p => p.Add(x => x.Value, 42));

        // Assert
        var input = cut.Find("input");
        Assert.Equal("42", input.GetAttribute("value"));
    }

    [Fact]
    public void AppliesMinMaxStepAttributes()
    {
        // Arrange & Act
        var cut = RenderNumberField(p => p
            .Add(x => x.Value, 5)
            .Add(x => x.Min, 0)
            .Add(x => x.Max, 10)
            .Add(x => x.Step, 2));

        // Assert
        var input = cut.Find("input");
        Assert.Equal("0", input.GetAttribute("min"));
        Assert.Equal("10", input.GetAttribute("max"));
        Assert.Equal("2", input.GetAttribute("step"));
    }

    [Fact]
    public void AppliesPlaceholder()
    {
        // Arrange & Act
        var cut = RenderNumberField(p => p.Add(x => x.Placeholder, "0.00"));

        // Assert
        var input = cut.Find("input");
        Assert.Equal("0.00", input.GetAttribute("placeholder"));
    }

    [Fact]
    public void InputIsDisabledWhenDisabledTrue()
    {
        // Arrange & Act
        var cut = RenderNumberField(p => p.Add(x => x.Disabled, true));

        // Assert
        var input = cut.Find("input");
        Assert.NotNull(input.GetAttribute("disabled"));
    }

    [Fact]
    public void InputIsReadOnlyWhenReadOnlyTrue()
    {
        // Arrange & Act
        var cut = RenderNumberField(p => p.Add(x => x.ReadOnly, true));

        // Assert
        var input = cut.Find("input");
        Assert.NotNull(input.GetAttribute("readonly"));
    }

    [Fact]
    public void AppliesDisabledClassWhenDisabled()
    {
        // Arrange & Act
        var cut = RenderNumberField(p => p.Add(x => x.Disabled, true));

        // Assert
        var wrapper = cut.Find(".sb-number-field");
        Assert.Contains("sb-number-field--disabled", wrapper.ClassList);
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderNumberField(p => p.Add(x => x.Class, "custom-field"));

        // Assert
        var wrapper = cut.Find(".sb-number-field");
        Assert.Contains("custom-field", wrapper.ClassList);
    }

    [Fact]
    public void AppliesStyleParameter()
    {
        // Arrange & Act
        var cut = RenderNumberField(p => p.Add(x => x.Style, "max-width: 200px;"));

        // Assert
        var wrapper = cut.Find(".sb-number-field");
        Assert.Contains("max-width: 200px", wrapper.GetAttribute("style"));
    }

    [Fact]
    public void RendersSpinButtonsByDefault()
    {
        // Arrange & Act
        var cut = RenderNumberField();

        // Assert
        var spinners = cut.FindAll(".sb-number-field__spinner");
        Assert.Equal(2, spinners.Count);
    }

    [Fact]
    public void DoesNotRenderSpinButtonsWhenShowSpinButtonsFalse()
    {
        // Arrange & Act
        var cut = RenderNumberField(p => p.Add(x => x.ShowSpinButtons, false));

        // Assert
        Assert.Empty(cut.FindAll(".sb-number-field__spinners"));
    }

    [Fact]
    public async Task InvokesValueChangedWhenInputChanged()
    {
        // Arrange
        int? received = null;
        var cut = RenderNumberField(p => p
            .Add(x => x.Value, 10)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<int>(this, v => received = v)));

        // Act
        var input = cut.Find("input");
        await cut.InvokeAsync(() => input!.Change("25"));

        // Assert
        Assert.NotNull(received);
        Assert.Equal(25, received);
    }

    [Fact]
    public async Task InvokesValueChangedWhenIncrementClicked()
    {
        // Arrange
        int? received = null;
        var cut = RenderNumberField(p => p
            .Add(x => x.Value, 5)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<int>(this, v => received = v)));

        // Act
        var incrementBtn = cut.FindAll(".sb-number-field__spinner")[0];
        await cut.InvokeAsync(() => incrementBtn!.Click());

        // Assert
        Assert.NotNull(received);
        Assert.Equal(6, received);
    }

    [Fact]
    public async Task InvokesValueChangedWhenDecrementClicked()
    {
        // Arrange
        int? received = null;
        var cut = RenderNumberField(p => p
            .Add(x => x.Value, 5)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<int>(this, v => received = v)));

        // Act
        var decrementBtn = cut.FindAll(".sb-number-field__spinner")[1];
        await cut.InvokeAsync(() => decrementBtn!.Click());

        // Assert
        Assert.NotNull(received);
        Assert.Equal(4, received);
    }

    [Fact]
    public void IncrementButtonDisabledWhenAtMax()
    {
        // Arrange & Act
        var cut = RenderNumberField(p => p
            .Add(x => x.Value, 10)
            .Add(x => x.Max, 10));

        // Assert
        var spinnerButtons = cut.FindAll(".sb-number-field__spinner");
        var incrementBtn = spinnerButtons[0];
        Assert.NotNull(incrementBtn.GetAttribute("disabled"));
    }

    [Fact]
    public void DecrementButtonDisabledWhenAtMin()
    {
        // Arrange & Act
        var cut = RenderNumberField(p => p
            .Add(x => x.Value, 0)
            .Add(x => x.Min, 0));

        // Assert
        var spinnerButtons = cut.FindAll(".sb-number-field__spinner");
        var decrementBtn = spinnerButtons[1];
        Assert.NotNull(decrementBtn.GetAttribute("disabled"));
    }

    [Fact]
    public void UsesCustomIncreaseDecreaseAriaLabels()
    {
        // Arrange & Act
        var cut = RenderNumberField(p => p
            .Add(x => x.IncreaseAriaLabel, "Add one")
            .Add(x => x.DecreaseAriaLabel, "Subtract one"));

        // Assert
        var spinners = cut.FindAll(".sb-number-field__spinner");
        Assert.Equal("Add one", spinners[0].GetAttribute("aria-label"));
        Assert.Equal("Subtract one", spinners[1].GetAttribute("aria-label"));
    }

    [Fact]
    public void WorksWithDecimalType()
    {
        // Arrange & Act
        var cut = Render<SbNumberField<decimal>>(p => p
            .Add(x => x.Value, 3.5m)
            .Add(x => x.Step, 0.5m));

        // Assert
        var input = cut.Find("input");
        Assert.Equal("3.5", input.GetAttribute("value"));
        Assert.Equal("0.5", input.GetAttribute("step"));
    }

    [Fact]
    public void AppliesAdditionalAttributes()
    {
        // Arrange & Act
        var cut = RenderNumberField(p => p.Add(x => x.AdditionalAttributes, new Dictionary<string, object>
        {
            { "data-testid", "number-input" },
            { "aria-label", "Quantity" }
        }));

        // Assert
        var input = cut.Find("input");
        Assert.Equal("number-input", input.GetAttribute("data-testid"));
        Assert.Equal("Quantity", input.GetAttribute("aria-label"));
    }
}
