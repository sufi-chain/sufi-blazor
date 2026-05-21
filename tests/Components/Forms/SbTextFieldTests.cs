using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Bunit;
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

public class SbTextFieldTests : BunitContext
{
    private string _textFieldValue = "";

    public SbTextFieldTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new StubStringLocalizer());
    }

    private IRenderedComponent<SbTextField<string>> RenderTextField(
        Action<ComponentParameterCollectionBuilder<SbTextField<string>>>? configure = null)
    {
        return Render<SbTextField<string>>(p =>
        {
            p.Add(x => x.ValueExpression, () => _textFieldValue);
            p.Add(x => x.Value, _textFieldValue);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, v => _textFieldValue = v));
            configure?.Invoke(p);
        });
    }

    /// <summary>Use when test needs to provide custom ValueChanged (avoids duplicate parameter).</summary>
    private IRenderedComponent<SbTextField<string>> RenderTextFieldWithCustomCallback(
        Action<ComponentParameterCollectionBuilder<SbTextField<string>>> configure)
    {
        return Render<SbTextField<string>>(p =>
        {
            p.Add(x => x.ValueExpression, () => _textFieldValue);
            p.Add(x => x.Value, _textFieldValue);
            configure.Invoke(p);
        });
    }

    [Fact]
    public void RendersTextFieldStructure()
    {
        // Arrange & Act
        var cut = RenderTextField();

        // Assert
        var wrapper = cut.Find(".sb-text-field-wrapper");
        Assert.NotNull(wrapper);
        var input = cut.Find("input.sb-text-field__input");
        Assert.NotNull(input);
    }

    [Fact]
    public void RendersLabelWhenProvided()
    {
        // Arrange & Act
        var cut = RenderTextField(p => p
            .Add(x => x.Label, "Username")
            .Add(x => x.Id, "username"));

        // Assert
        var label = cut.Find(".sb-text-field__label");
        Assert.NotNull(label);
        Assert.Contains("Username", label.TextContent);
        Assert.Equal("username", label.GetAttribute("for"));
    }

    [Fact]
    public void DoesNotRenderLabelWhenEmpty()
    {
        // Arrange & Act
        var cut = RenderTextField();

        // Assert
        Assert.Empty(cut.FindAll(".sb-text-field__label"));
    }

    [Fact]
    public void RendersRequiredIndicatorWhenRequiredTrue()
    {
        // Arrange & Act
        var cut = RenderTextField(p => p
            .Add(x => x.Label, "Email")
            .Add(x => x.Required, true));

        // Assert
        var required = cut.Find(".sb-text-field__required");
        Assert.NotNull(required);
        Assert.Equal("*", required.TextContent);
        Assert.Equal("true", required.GetAttribute("aria-hidden"));
    }

    [Fact]
    public void AssociatesLabelWithInputId()
    {
        // Arrange & Act
        var cut = RenderTextField(p => p
            .Add(x => x.Label, "Name")
            .Add(x => x.Id, "name-field"));

        // Assert
        var label = cut.Find("label.sb-text-field__label");
        var input = cut.Find("input");
        Assert.Equal("name-field", label.GetAttribute("for"));
        Assert.Equal("name-field", input.GetAttribute("id"));
    }

    [Fact]
    public void DisplaysValue()
    {
        // Arrange & Act
        _textFieldValue = "hello";
        var cut = RenderTextField();

        // Assert
        var input = cut.Find("input");
        Assert.Equal("hello", input.GetAttribute("value"));
    }

    [Fact]
    public void RendersPlaceholder()
    {
        // Arrange & Act
        var cut = RenderTextField(p => p.Add(x => x.Placeholder, "Enter text..."));

        // Assert
        var input = cut.Find("input");
        Assert.Equal("Enter text...", input.GetAttribute("placeholder"));
    }

    [Fact]
    public void UsesTextTypeByDefault()
    {
        // Arrange & Act
        var cut = RenderTextField();

        // Assert
        var input = cut.Find("input");
        Assert.Equal("text", input.GetAttribute("type"));
    }

    [Fact]
    public void UsesPasswordTypeWhenTypePassword()
    {
        // Arrange & Act
        var cut = RenderTextField(p => p.Add(x => x.Type, "password"));

        // Assert
        var input = cut.Find("input");
        Assert.Equal("password", input.GetAttribute("type"));
    }

    [Fact]
    public void AppliesInputModeWhenProvided()
    {
        // Arrange & Act
        var cut = RenderTextField(p => p.Add(x => x.InputMode, "email"));

        // Assert
        var input = cut.Find("input");
        Assert.Equal("email", input.GetAttribute("inputmode"));
    }

    [Fact]
    public void AppliesAutoCompleteWhenProvided()
    {
        // Arrange & Act
        var cut = RenderTextField(p => p.Add(x => x.AutoComplete, "email"));

        // Assert
        var input = cut.Find("input");
        Assert.Equal("email", input.GetAttribute("autocomplete"));
    }

    [Fact]
    public void AppliesDisabledClassWhenDisabledTrue()
    {
        // Arrange & Act
        var cut = RenderTextField(p => p.Add(x => x.Disabled, true));

        // Assert
        var innerWrapper = cut.Find(".sb-text-field");
        Assert.Contains("sb-text-field--disabled", innerWrapper.ClassList);
        var input = cut.Find("input");
        Assert.NotNull(input.GetAttribute("disabled"));
    }

    [Fact]
    public void AppliesReadOnlyClassWhenReadOnlyTrue()
    {
        // Arrange & Act
        var cut = RenderTextField(p => p.Add(x => x.ReadOnly, true));

        // Assert
        var innerWrapper = cut.Find(".sb-text-field");
        Assert.Contains("sb-text-field--readonly", innerWrapper.ClassList);
        var input = cut.Find("input");
        Assert.NotNull(input.GetAttribute("readonly"));
    }

    [Fact]
    public void RendersClearButtonWhenClearableAndValueSet()
    {
        // Arrange & Act
        _textFieldValue = "some text";
        var cut = RenderTextField(p => p.Add(x => x.Clearable, true));

        // Assert
        var clearBtn = cut.Find(".sb-text-field__clear");
        Assert.NotNull(clearBtn);
        Assert.Equal("Clear", clearBtn.GetAttribute("aria-label"));
    }

    [Fact]
    public void DoesNotRenderClearButtonWhenClearableFalse()
    {
        // Arrange & Act
        _textFieldValue = "text";
        var cut = RenderTextField();

        // Assert
        Assert.Empty(cut.FindAll(".sb-text-field__clear"));
    }

    [Fact]
    public void DoesNotRenderClearButtonWhenValueEmpty()
    {
        // Arrange & Act
        var cut = RenderTextField(p => p.Add(x => x.Clearable, true));

        // Assert
        Assert.Empty(cut.FindAll(".sb-text-field__clear"));
    }

    [Fact]
    public void RendersClearButtonWithCustomAriaLabel()
    {
        // Arrange & Act
        _textFieldValue = "x";
        var cut = RenderTextField(p => p
            .Add(x => x.Clearable, true)
            .Add(x => x.ClearAriaLabel, "Clear field"));

        // Assert
        var clearBtn = cut.Find(".sb-text-field__clear");
        Assert.Equal("Clear field", clearBtn.GetAttribute("aria-label"));
    }

    [Fact]
    public void RendersPasswordToggleWhenTypePassword()
    {
        // Arrange & Act
        var cut = RenderTextField(p => p.Add(x => x.Type, "password"));

        // Assert
        var toggle = cut.Find(".sb-text-field__toggle-password");
        Assert.NotNull(toggle);
        Assert.Equal("ShowPassword", toggle.GetAttribute("aria-label"));
    }

    [Fact]
    public void DoesNotRenderPasswordToggleWhenTypeText()
    {
        // Arrange & Act
        var cut = RenderTextField();

        // Assert
        Assert.Empty(cut.FindAll(".sb-text-field__toggle-password"));
    }

    [Fact]
    public void RendersStartAdornmentWhenProvided()
    {
        // Arrange & Act
        var cut = RenderTextField(p => p.Add(x => x.StartAdornment, b => b.AddMarkupContent(0, "<span>@</span>")));

        // Assert
        var adornment = cut.Find(".sb-text-field__adornment--start");
        Assert.NotNull(adornment);
        Assert.Contains("sb-text-field--has-start", cut.Find(".sb-text-field").ClassList);
    }

    [Fact]
    public void RendersEndAdornmentWhenProvided()
    {
        // Arrange & Act
        var cut = RenderTextField(p => p.Add(x => x.EndAdornment, b => b.AddMarkupContent(0, "<span>.com</span>")));

        // Assert
        var adornment = cut.Find(".sb-text-field__adornment--end");
        Assert.NotNull(adornment);
        Assert.Contains("sb-text-field--has-end", cut.Find(".sb-text-field").ClassList);
    }

    [Fact]
    public void AppliesStyleParameter()
    {
        // Arrange & Act
        var cut = RenderTextField(p => p.Add(x => x.Style, "max-width: 300px;"));

        // Assert
        var wrapper = cut.Find(".sb-text-field-wrapper");
        Assert.Contains("max-width: 300px", wrapper.GetAttribute("style"));
    }

    [Fact]
    public void AppliesAriaRequiredWhenRequiredTrue()
    {
        // Arrange & Act
        var cut = RenderTextField(p => p.Add(x => x.Required, true));

        // Assert
        var input = cut.Find("input");
        Assert.Equal("true", input.GetAttribute("aria-required"));
    }

    [Fact]
    public void AppliesAriaDescribedByWhenProvided()
    {
        // Arrange & Act
        var cut = RenderTextField(p => p.Add(x => x.AriaDescribedBy, "helper-id"));

        // Assert
        var input = cut.Find("input");
        Assert.Equal("helper-id", input.GetAttribute("aria-describedby"));
    }

    [Fact]
    public async Task InvokesValueChangedOnInput()
    {
        // Arrange
        string? received = null;
        var cut = RenderTextFieldWithCustomCallback(p => p
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, v => received = v)));

        var input = cut.Find("input");

        // Act
        await cut.InvokeAsync(() => input!.Input("typed text"));

        // Assert
        Assert.Equal("typed text", received);
    }

    [Fact]
    public async Task InvokesValueChangedWithEmptyWhenClearClicked()
    {
        // Arrange
        _textFieldValue = "remove me";
        string? received = "original";
        var cut = RenderTextFieldWithCustomCallback(p => p
            .Add(x => x.Clearable, true)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, v => received = v)));

        var clearBtn = cut.Find(".sb-text-field__clear");

        // Act
        await cut.InvokeAsync(() => clearBtn!.Click());

        // Assert - ValueChanged is invoked with empty string
        Assert.Equal("", received);
    }

    [Fact]
    public async Task InvokesOnClearWhenClearClicked()
    {
        // Arrange
        _textFieldValue = "x";
        var clearInvoked = false;
        var cut = RenderTextField(p => p
            .Add(x => x.Clearable, true)
            .Add(x => x.OnClear, EventCallback.Factory.Create(this, () => clearInvoked = true)));

        var clearBtn = cut.Find(".sb-text-field__clear");

        // Act
        await cut.InvokeAsync(() => clearBtn!.Click());

        // Assert
        Assert.True(clearInvoked);
    }

    [Fact]
    public async Task TogglesPasswordVisibilityWhenToggleClicked()
    {
        // Arrange
        _textFieldValue = "secret";
        var cut = RenderTextField(p => p.Add(x => x.Type, "password"));

        var toggle = cut.Find(".sb-text-field__toggle-password");
        var input = cut.Find("input");

        // Assert initial - type is password
        Assert.Equal("password", input.GetAttribute("type"));

        // Act - click to show password
        await cut.InvokeAsync(() => toggle!.Click());

        // Assert - type switches to text
        Assert.Equal("text", input.GetAttribute("type"));
        Assert.Equal("HidePassword", toggle.GetAttribute("aria-label"));

        // Act - click again to hide
        await cut.InvokeAsync(() => toggle.Click());

        // Assert - back to password
        Assert.Equal("password", input.GetAttribute("type"));
    }

    [Fact]
    public void AppliesAdditionalAttributes()
    {
        // Arrange & Act
        var cut = RenderTextField(p => p.Add(x => x.AdditionalAttributes, new Dictionary<string, object>
        {
            { "data-testid", "username-input" },
            { "aria-label", "Username" }
        }));

        // Assert
        var input = cut.Find("input");
        Assert.Equal("username-input", input.GetAttribute("data-testid"));
        Assert.Equal("Username", input.GetAttribute("aria-label"));
    }
}
