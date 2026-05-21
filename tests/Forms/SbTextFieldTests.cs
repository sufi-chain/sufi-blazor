using Bunit;
using SufiChain.SufiBlazor.Components.Forms;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Forms;

public class SbTextFieldTests : BunitContext
{
    [Fact]
    public void PasswordField_DefaultsToNewPassword_Autocomplete()
    {
        // Arrange & Act - password field without explicit AutoComplete
        var cut = Render<SbTextField<string>>(parameters => parameters
            .Add(p => p.Type, "password")
            .Add(p => p.Value, "")
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<string>(this, _ => { })));

        // Assert - prevents browser autofill with cached credentials
        var input = cut.Find("input");
        Assert.Equal("new-password", input.GetAttribute("autocomplete"));
    }

    [Fact]
    public void PasswordField_WithCurrentPassword_UsesExplicitAutocomplete()
    {
        // Arrange & Act - login form explicitly requests current-password autofill
        var cut = Render<SbTextField<string>>(parameters => parameters
            .Add(p => p.Type, "password")
            .Add(p => p.AutoComplete, "current-password")
            .Add(p => p.Value, "")
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<string>(this, _ => { })));

        // Assert
        var input = cut.Find("input");
        Assert.Equal("current-password", input.GetAttribute("autocomplete"));
    }

    [Fact]
    public void TextField_WithoutAutocomplete_DoesNotSetAutocompleteAttribute()
    {
        // Arrange & Act - non-password field, no AutoComplete
        var cut = Render<SbTextField<string>>(parameters => parameters
            .Add(p => p.Type, "text")
            .Add(p => p.Value, "")
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<string>(this, _ => { })));

        // Assert - no autocomplete attribute for text fields when not specified
        var input = cut.Find("input");
        Assert.Null(input.GetAttribute("autocomplete"));
    }

    [Fact]
    public void RendersPasswordToggle_WhenTypeIsPassword()
    {
        // Arrange & Act
        var cut = Render<SbTextField<string>>(parameters => parameters
            .Add(p => p.Type, "password")
            .Add(p => p.Value, "")
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<string>(this, _ => { })));

        // Assert
        var toggle = cut.Find(".sb-text-field__toggle-password");
        Assert.NotNull(toggle);
    }
}
