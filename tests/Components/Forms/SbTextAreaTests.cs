using Microsoft.AspNetCore.Components;
using Bunit;
using SufiChain.SufiBlazor.Components.Forms;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Forms;

public class SbTextAreaTests : BunitContext
{
    private IRenderedComponent<SbTextArea> RenderTextArea(
        Action<ComponentParameterCollectionBuilder<SbTextArea>>? configure = null)
    {
        return Render<SbTextArea>(p => configure?.Invoke(p));
    }

    [Fact]
    public void RendersTextAreaStructure()
    {
        // Arrange & Act
        var cut = RenderTextArea();

        // Assert
        var wrapper = cut.Find(".sb-textarea");
        Assert.NotNull(wrapper);
        var textarea = cut.Find("textarea.sb-textarea__input");
        Assert.NotNull(textarea);
    }

    [Fact]
    public void RendersLabelWhenProvided()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p
            .Add(x => x.Label, "Description")
            .Add(x => x.Id, "desc"));

        // Assert
        var label = cut.Find(".sb-textarea__label");
        Assert.NotNull(label);
        Assert.Contains("Description", label.TextContent);
        Assert.Equal("desc", label.GetAttribute("for"));
    }

    [Fact]
    public void DoesNotRenderLabelWhenEmpty()
    {
        // Arrange & Act
        var cut = RenderTextArea();

        // Assert
        Assert.Empty(cut.FindAll(".sb-textarea__label"));
    }

    [Fact]
    public void RendersRequiredIndicatorWhenRequiredTrue()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p
            .Add(x => x.Label, "Comment")
            .Add(x => x.Required, true));

        // Assert
        var required = cut.Find(".sb-textarea__required");
        Assert.NotNull(required);
        Assert.Equal("*", required.TextContent);
        Assert.Equal("true", required.GetAttribute("aria-hidden"));
    }

    [Fact]
    public void AssociatesLabelWithTextAreaId()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p
            .Add(x => x.Label, "Notes")
            .Add(x => x.Id, "notes-field"));

        // Assert
        var label = cut.Find("label.sb-textarea__label");
        var textarea = cut.Find("textarea");
        Assert.Equal("notes-field", label.GetAttribute("for"));
        Assert.Equal("notes-field", textarea.GetAttribute("id"));
    }

    [Fact]
    public void RendersHelperTextWhenProvided()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p
            .Add(x => x.HelperText, "Enter a detailed description")
            .Add(x => x.Id, "desc"));

        // Assert
        var helper = cut.Find(".sb-textarea__helper");
        Assert.NotNull(helper);
        Assert.Contains("Enter a detailed description", helper.TextContent);
        Assert.Equal("desc-helper", helper.GetAttribute("id"));
    }

    [Fact]
    public void HidesHelperTextWhenErrorTrue()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p
            .Add(x => x.HelperText, "Helper text")
            .Add(x => x.Error, true)
            .Add(x => x.ErrorMessage, "Invalid input"));

        // Assert
        Assert.Empty(cut.FindAll(".sb-textarea__helper"));
    }

    [Fact]
    public void RendersErrorMessageWhenErrorTrue()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p
            .Add(x => x.Error, true)
            .Add(x => x.ErrorMessage, "This field is required")
            .Add(x => x.Id, "desc"));

        // Assert
        var error = cut.Find(".sb-textarea__error");
        Assert.NotNull(error);
        Assert.Contains("This field is required", error.TextContent);
        Assert.Equal("alert", error.GetAttribute("role"));
        Assert.Equal("desc-error", error.GetAttribute("id"));
    }

    [Fact]
    public void DoesNotRenderErrorMessageWhenErrorFalse()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.ErrorMessage, "Should not show"));

        // Assert
        Assert.Empty(cut.FindAll(".sb-textarea__error"));
    }

    [Fact]
    public void DisplaysValue()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.Value, "Hello world"));

        // Assert
        var textarea = cut.Find("textarea");
        Assert.Equal("Hello world", textarea.GetAttribute("value"));
    }

    [Fact]
    public void RendersPlaceholder()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.Placeholder, "Type here..."));

        // Assert
        var textarea = cut.Find("textarea");
        Assert.Equal("Type here...", textarea.GetAttribute("placeholder"));
    }

    [Fact]
    public void AppliesRowsAttribute()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.Rows, 5));

        // Assert
        var textarea = cut.Find("textarea");
        Assert.Equal("5", textarea.GetAttribute("rows"));
    }

    [Fact]
    public void AppliesMaxLengthAttribute()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.MaxLength, 500));

        // Assert
        var textarea = cut.Find("textarea");
        Assert.Equal("500", textarea.GetAttribute("maxlength"));
    }

    [Fact]
    public void RendersCharacterCountWhenShowCharacterCountAndMaxLengthSet()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p
            .Add(x => x.Value, "Hi")
            .Add(x => x.ShowCharacterCount, true)
            .Add(x => x.MaxLength, 100));

        // Assert
        var counter = cut.Find(".sb-textarea__counter");
        Assert.NotNull(counter);
        Assert.Contains("2 / 100", counter.TextContent);
    }

    [Fact]
    public void DoesNotRenderCharacterCountWhenShowCharacterCountFalse()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p
            .Add(x => x.MaxLength, 100)
            .Add(x => x.ShowCharacterCount, false));

        // Assert
        Assert.Empty(cut.FindAll(".sb-textarea__counter"));
    }

    [Fact]
    public void DoesNotRenderCharacterCountWhenMaxLengthNotSet()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.ShowCharacterCount, true));

        // Assert
        Assert.Empty(cut.FindAll(".sb-textarea__counter"));
    }

    [Fact]
    public void RendersResizeHandleWhenResizableTrue()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.Resizable, true));

        // Assert
        Assert.NotNull(cut.Find(".sb-textarea__resize-handle"));
    }

    [Fact]
    public void DoesNotRenderResizeHandleWhenResizableFalse()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.Resizable, false));

        // Assert
        Assert.Empty(cut.FindAll(".sb-textarea__resize-handle"));
    }

    [Fact]
    public void AppliesErrorClassWhenErrorTrue()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.Error, true));

        // Assert
        var wrapper = cut.Find(".sb-textarea");
        Assert.Contains("sb-textarea--error", wrapper.ClassList);
    }

    [Fact]
    public void AppliesDisabledClassWhenDisabledTrue()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.Disabled, true));

        // Assert
        var wrapper = cut.Find(".sb-textarea");
        Assert.Contains("sb-textarea--disabled", wrapper.ClassList);
        var textarea = cut.Find("textarea");
        Assert.NotNull(textarea.GetAttribute("disabled"));
    }

    [Fact]
    public void AppliesReadOnlyClassWhenReadOnlyTrue()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.ReadOnly, true));

        // Assert
        var wrapper = cut.Find(".sb-textarea");
        Assert.Contains("sb-textarea--readonly", wrapper.ClassList);
        var textarea = cut.Find("textarea");
        Assert.NotNull(textarea.GetAttribute("readonly"));
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.Class, "my-textarea"));

        // Assert
        var wrapper = cut.Find(".sb-textarea");
        Assert.Contains("my-textarea", wrapper.ClassList);
    }

    [Fact]
    public void AppliesStyleParameter()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.Style, "min-height: 100px;"));

        // Assert
        var wrapper = cut.Find(".sb-textarea");
        Assert.Contains("min-height: 100px", wrapper.GetAttribute("style"));
    }

    [Fact]
    public void SetsAriaInvalidWhenErrorTrue()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.Error, true));

        // Assert
        var textarea = cut.Find("textarea");
        Assert.Equal("true", textarea.GetAttribute("aria-invalid"));
    }

    [Fact]
    public void SetsAriaDescribedByToHelperWhenHelperTextProvided()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p
            .Add(x => x.Id, "desc")
            .Add(x => x.HelperText, "Hint text"));

        // Assert
        var textarea = cut.Find("textarea");
        Assert.Equal("desc-helper", textarea.GetAttribute("aria-describedby"));
    }

    [Fact]
    public void SetsAriaDescribedByToErrorWhenErrorTrue()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p
            .Add(x => x.Id, "desc")
            .Add(x => x.HelperText, "Hint")
            .Add(x => x.Error, true)
            .Add(x => x.ErrorMessage, "Error"));

        // Assert - when error, aria-describedby points to error, not helper
        var textarea = cut.Find("textarea");
        Assert.Equal("desc-error", textarea.GetAttribute("aria-describedby"));
    }

    [Fact]
    public async Task InvokesValueChangedOnInput()
    {
        // Arrange
        string? received = null;
        var cut = RenderTextArea(p => p
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => received = v)));

        var textarea = cut.Find("textarea");

        // Act
        await cut.InvokeAsync(() => textarea!.Input("New text"));

        // Assert
        Assert.Equal("New text", received);
    }

    [Fact]
    public async Task InvokesValueChangedOnChange()
    {
        // Arrange
        string? received = null;
        var cut = RenderTextArea(p => p
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => received = v)));

        var textarea = cut.Find("textarea");

        // Act
        await cut.InvokeAsync(() => textarea!.Change("Changed text"));

        // Assert
        Assert.Equal("Changed text", received);
    }

    [Fact]
    public void AppliesAdditionalAttributes()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.AdditionalAttributes, new Dictionary<string, object>
        {
            { "data-testid", "description-area" },
            { "aria-label", "Description field" }
        }));

        // Assert
        var textarea = cut.Find("textarea");
        Assert.Equal("description-area", textarea.GetAttribute("data-testid"));
        Assert.Equal("Description field", textarea.GetAttribute("aria-label"));
    }
}
