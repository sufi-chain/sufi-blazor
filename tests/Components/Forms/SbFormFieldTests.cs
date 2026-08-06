using Bunit;
using SufiChain.SufiBlazor.Components.Forms;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Forms;

public class SbFormFieldTests : BunitContext
{
    private IRenderedComponent<SbFormField> RenderFormField(
        Action<ComponentParameterCollectionBuilder<SbFormField>>? configure = null)
    {
        return Render<SbFormField>(p =>
        {
            p.AddChildContent("<input type=\"text\" />");
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersFormFieldStructure()
    {
        // Arrange & Act
        var cut = RenderFormField();

        // Assert
        var wrapper = cut.Find(".sb-form-field");
        Assert.NotNull(wrapper);
        Assert.NotNull(cut.Find(".sb-form-field__input"));
    }

    [Fact]
    public void RendersChildContent()
    {
        // Arrange & Act
        var cut = RenderFormField();

        // Assert
        var inputWrapper = cut.Find(".sb-form-field__input");
        Assert.NotNull(inputWrapper);
        Assert.NotNull(cut.Find("input[type='text']"));
    }

    [Fact]
    public void RendersLabelWhenProvided()
    {
        // Arrange & Act
        var cut = RenderFormField(p => p
            .Add(x => x.Label, "Email")
            .Add(x => x.InputId, "email"));

        // Assert
        var label = cut.Find(".sb-form-field__label");
        Assert.NotNull(label);
        Assert.Contains("Email", label.TextContent);
    }

    [Fact]
    public void DoesNotRenderLabelWhenEmpty()
    {
        // Arrange & Act
        var cut = RenderFormField();

        // Assert
        Assert.Empty(cut.FindAll(".sb-form-field__label"));
    }

    [Fact]
    public void RendersRequiredIndicatorWhenRequiredTrue()
    {
        // Arrange & Act
        var cut = RenderFormField(p => p
            .Add(x => x.Label, "Name")
            .Add(x => x.Required, true));

        // Assert
        var requiredSpan = cut.Find(".sb-form-field__required");
        Assert.NotNull(requiredSpan);
        Assert.Equal("*", requiredSpan.TextContent);
        Assert.Equal("true", requiredSpan.GetAttribute("aria-hidden"));
    }

    [Fact]
    public void DoesNotRenderRequiredIndicatorWhenRequiredFalse()
    {
        // Arrange & Act
        var cut = RenderFormField(p => p
            .Add(x => x.Label, "Name")
            .Add(x => x.Required, false));

        // Assert
        Assert.Empty(cut.FindAll(".sb-form-field__required"));
    }

    [Fact]
    public void AssociatesLabelWithInputId()
    {
        // Arrange & Act
        var cut = RenderFormField(p => p
            .Add(x => x.Label, "Username")
            .Add(x => x.InputId, "username-field"));

        // Assert
        var label = cut.Find("label.sb-form-field__label");
        Assert.Equal("username-field", label.GetAttribute("for"));
    }

    [Fact]
    public void RendersHelperTextWhenProvided()
    {
        // Arrange & Act
        var cut = RenderFormField(p => p
            .Add(x => x.HelperText, "We'll never share your email")
            .Add(x => x.InputId, "email"));

        // Assert
        var helper = cut.Find(".sb-form-field__helper");
        Assert.NotNull(helper);
        Assert.Contains("We'll never share your email", helper.TextContent);
    }

    [Fact]
    public void DoesNotRenderHelperTextWhenErrorTextPresent()
    {
        // Arrange & Act - ErrorText takes precedence, helper is hidden
        var cut = RenderFormField(p => p
            .Add(x => x.HelperText, "Helper text")
            .Add(x => x.ErrorText, "This field is invalid")
            .Add(x => x.InputId, "field"));

        // Assert
        Assert.Empty(cut.FindAll(".sb-form-field__helper"));
    }

    [Fact]
    public void RendersErrorTextWhenProvided()
    {
        // Arrange & Act
        var cut = RenderFormField(p => p
            .Add(x => x.ErrorText, "Email is required")
            .Add(x => x.InputId, "email"));

        // Assert
        var error = cut.Find(".sb-form-field__error");
        Assert.NotNull(error);
        Assert.Contains("Email is required", error.TextContent);
        Assert.Equal("alert", error.GetAttribute("role"));
    }

    [Fact]
    public void AppliesErrorClassWhenErrorTextPresent()
    {
        // Arrange & Act
        var cut = RenderFormField(p => p.Add(x => x.ErrorText, "Invalid"));

        // Assert
        var wrapper = cut.Find(".sb-form-field");
        Assert.Contains("sb-form-field--error", wrapper.ClassList);
    }

    [Fact]
    public void AppliesDisabledClassWhenDisabledTrue()
    {
        // Arrange & Act
        var cut = RenderFormField(p => p.Add(x => x.Disabled, true));

        // Assert
        var wrapper = cut.Find(".sb-form-field");
        Assert.Contains("sb-form-field--disabled", wrapper.ClassList);
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderFormField(p => p.Add(x => x.Class, "custom-field"));

        // Assert
        var wrapper = cut.Find(".sb-form-field");
        Assert.Contains("custom-field", wrapper.ClassList);
    }

    [Fact]
    public void AppliesStyleParameter()
    {
        // Arrange & Act
        var cut = RenderFormField(p => p.Add(x => x.Style, "margin: 8px;"));

        // Assert
        var wrapper = cut.Find(".sb-form-field");
        Assert.Contains("margin: 8px", wrapper.GetAttribute("style"));
    }

    [Fact]
    public void AppliesAdditionalAttributes()
    {
        // Arrange & Act
        var cut = RenderFormField(p => p.Add(x => x.AdditionalAttributes, new Dictionary<string, object>
        {
            { "data-testid", "form-field" },
            { "aria-describedby", "field-desc" }
        }));

        // Assert
        var wrapper = cut.Find(".sb-form-field");
        Assert.Equal("form-field", wrapper.GetAttribute("data-testid"));
        Assert.Equal("field-desc", wrapper.GetAttribute("aria-describedby"));
    }

    [Fact]
    public void HelperAndErrorIdsUseInputId()
    {
        // Arrange & Act
        var cut = RenderFormField(p => p
            .Add(x => x.InputId, "my-input")
            .Add(x => x.HelperText, "Helper")
            .Add(x => x.ErrorText, "Error"));

        // Assert - ErrorText overrides HelperText display, but both ids should be set
        var error = cut.Find(".sb-form-field__error");
        Assert.NotNull(error);
        Assert.Equal("my-input-error", error.GetAttribute("id"));

        // When only HelperText (no ErrorText), helper id should be my-input-helper
        var cutHelperOnly = RenderFormField(p => p
            .Add(x => x.InputId, "my-input")
            .Add(x => x.HelperText, "Helper"));
        var helper = cutHelperOnly.Find(".sb-form-field__helper");
        Assert.NotNull(helper);
        Assert.Equal("my-input-helper", helper.GetAttribute("id"));
    }
}
