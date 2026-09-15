using System.Globalization;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using SufiChain.SufiBlazor.Components.Forms;
using SufiChain.SufiBlazor.Localization;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Forms;

file class StubStringLocalizer : IStringLocalizer<SufiBlazorResource>
{
    public LocalizedString this[string name] => new(name, name);
    public LocalizedString this[string name, params object[] arguments] =>
        new(name, string.Format(CultureInfo.InvariantCulture, name, arguments));
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => Array.Empty<LocalizedString>();
}

public class SbTextFieldTests : BunitContext
{
    private string _value = "";

    public SbTextFieldTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new StubStringLocalizer());
    }

    private IRenderedComponent<SbTextField<string>> RenderTextField(
        Action<ComponentParameterCollectionBuilder<SbTextField<string>>> configure)
    {
        return Render<SbTextField<string>>(parameters =>
        {
            parameters
                .Add(p => p.ValueExpression, () => _value)
                .Add(p => p.Value, _value)
                .Add(p => p.ValueChanged, EventCallback.Factory.Create<string>(this, v => _value = v));
            configure(parameters);
        });
    }

    [Fact]
    public void PasswordField_DefaultsToNewPassword_Autocomplete()
    {
        // Arrange & Act - password field without explicit AutoComplete
        var cut = RenderTextField(parameters => parameters.Add(p => p.Type, "password"));

        // Assert - prevents browser autofill with cached credentials
        var input = cut.Find("input");
        Assert.Equal("new-password", input.GetAttribute("autocomplete"));
    }

    [Fact]
    public void PasswordField_WithCurrentPassword_UsesExplicitAutocomplete()
    {
        // Arrange & Act - login form explicitly requests current-password autofill
        var cut = RenderTextField(parameters => parameters
            .Add(p => p.Type, "password")
            .Add(p => p.AutoComplete, "current-password"));

        // Assert
        var input = cut.Find("input");
        Assert.Equal("current-password", input.GetAttribute("autocomplete"));
    }

    [Fact]
    public void TextField_WithoutAutocomplete_DoesNotSetAutocompleteAttribute()
    {
        // Arrange & Act - non-password field, no AutoComplete
        var cut = RenderTextField(parameters => parameters.Add(p => p.Type, "text"));

        // Assert - no autocomplete attribute for text fields when not specified
        var input = cut.Find("input");
        Assert.Null(input.GetAttribute("autocomplete"));
    }

    [Fact]
    public void RendersPasswordToggle_WhenTypeIsPassword()
    {
        // Arrange & Act
        var cut = RenderTextField(parameters => parameters.Add(p => p.Type, "password"));

        // Assert
        var toggle = cut.Find(".sb-text-field__toggle-password");
        Assert.NotNull(toggle);
    }
}
