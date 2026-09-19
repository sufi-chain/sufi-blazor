using System.Globalization;
using Bunit;
using Bunit.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using SufiChain.SufiBlazor.Components.Forms;
using SufiChain.SufiBlazor.Components.Overlays;
using SufiChain.SufiBlazor.Localization;

namespace SufiChain.SufiBlazor.Tests.Components.Forms;

public class ConfigurationAccessibilityTests : BunitContext
{
    private string _value = "existing value";

    public ConfigurationAccessibilityTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new ConfigurationLocalizer());
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void NumericFieldGeneratesStableUniqueLabelAndHelpIds()
    {
        var first = Render<SbNumberField<int>>(p => p
            .Add(x => x.Label, "Retention days")
            .Add(x => x.HelperText, "Applies to archived files"));
        var second = Render<SbNumberField<int>>(p => p.Add(x => x.Label, "Maximum retries"));
        var id = first.Find("input").Id;

        Assert.False(string.IsNullOrWhiteSpace(id));
        Assert.NotEqual(id, second.Find("input").Id);
        Assert.Equal(id, first.Find("label").GetAttribute("for"));
        Assert.Equal(first.Find(".sb-number-field__helper").Id, first.Find("input").GetAttribute("aria-describedby"));
        first.Render(p => p.Add(x => x.Value, 10));
        Assert.Equal(id, first.Find("input").Id);
    }

    [Fact]
    public void FormFieldConnectsLabelRequiredStateAndChangingErrorsToNestedInput()
    {
        RenderFragment input = builder =>
        {
            builder.OpenComponent<SbNumberField<int>>(0);
            builder.CloseComponent();
        };
        var cut = Render<SbFormField>(p => p
            .Add(x => x.Label, "Retention days")
            .Add(x => x.HelperText, "Choose a positive number")
            .Add(x => x.Required, true)
            .Add(x => x.ChildContent, input));

        var inputId = cut.Find("input").Id;
        Assert.Equal(inputId, cut.Find(".sb-form-field__label").GetAttribute("for"));
        Assert.Equal("true", cut.Find("input").GetAttribute("aria-required"));
        Assert.Equal(cut.Find(".sb-form-field__helper").Id, cut.Find("input").GetAttribute("aria-describedby"));

        cut.Render(p => p.Add(x => x.ErrorText, "Retention must be greater than zero"));

        Assert.Equal(inputId, cut.Find("input").Id);
        Assert.Equal("true", cut.Find("input").GetAttribute("aria-invalid"));
        Assert.Empty(cut.FindAll(".sb-form-field__helper"));
        Assert.Equal(cut.Find(".sb-form-field__error").Id, cut.Find("input").GetAttribute("aria-describedby"));
    }

    [Fact]
    public void ReadOnlyNumberFieldDisablesItsSpinnerButtons()
    {
        var cut = Render<SbNumberField<int>>(p => p.Add(x => x.ReadOnly, true));
        Assert.All(cut.FindAll(".sb-number-field__spinner"), button => Assert.True(button.HasAttribute("disabled")));
    }

    [Theory]
    [InlineData("Provider")]
    [InlineData(null)]
    public void WrappedSelectOnlyReferencesLabelsThatAreRendered(string? label)
    {
        RenderFragment input = builder =>
        {
            builder.OpenComponent<SbSimpleSelect<string>>(0);
            builder.CloseComponent();
        };
        var cut = Render<SbFormField>(p => p
            .Add(x => x.Label, label)
            .Add(x => x.HelperText, "Select an available provider")
            .Add(x => x.ChildContent, input));
        var trigger = cut.Find(".sb-select-trigger__main");
        foreach (var id in trigger.GetAttribute("aria-labelledby")!.Split(' '))
            Assert.NotNull(cut.Find($"#{id}"));
        Assert.Equal(cut.Find(".sb-form-field__helper").Id, trigger.GetAttribute("aria-describedby"));
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void NonEditableTextFieldsDoNotOfferClear(bool disabled, bool readOnly)
    {
        var cut = Render<SbTextField<string>>(p => p
            .Add(x => x.ValueExpression, () => _value)
            .Add(x => x.Value, _value)
            .Add(x => x.Clearable, true)
            .Add(x => x.Disabled, disabled)
            .Add(x => x.ReadOnly, readOnly));
        Assert.Empty(cut.FindAll(".sb-text-field__clear"));
    }

    [Fact]
    public void PasswordVisibilityIsKeyboardReachableAndControlsTheAssociatedInput()
    {
        var cut = Render<SbTextField<string>>(p => p
            .Add(x => x.ValueExpression, () => _value)
            .Add(x => x.Type, "password"));
        var toggle = cut.Find(".sb-text-field__toggle-password");
        Assert.NotEqual("-1", toggle.GetAttribute("tabindex"));
        Assert.Equal(cut.Find("input").Id, toggle.GetAttribute("aria-controls"));
        toggle.Click();
        Assert.Equal("text", cut.Find("input").GetAttribute("type"));
    }

    [Fact]
    public void CustomDialogHeaderAndDescriptionHaveValidAccessibleReferences()
    {
        var header = Render<SbDialog>(p => p.Add(x => x.Header,
            (RenderFragment)(builder => builder.AddContent(0, "Configure routing"))));
        var labelId = header.Find("dialog").GetAttribute("aria-labelledby");
        Assert.Equal("Configure routing", header.Find($"#{labelId}").TextContent);

        var described = Render<SbDialog>(p => p
            .Add(x => x.Title, "Configure provider")
            .Add(x => x.Description, "Applies to this workspace"));
        var descriptionId = described.Find("dialog").GetAttribute("aria-describedby");
        Assert.Equal("Applies to this workspace", described.Find($"#{descriptionId}").TextContent);
    }

    [Fact]
    public void SelectLabelValueAndOptionsReferencesResolveAndEnterDoesNotDoubleToggle()
    {
        RenderFragment options = builder =>
        {
            builder.OpenComponent<SbSelectOption<string>>(0);
            builder.AddAttribute(1, "Value", "primary");
            builder.AddAttribute(2, "Text", "Primary provider");
            builder.CloseComponent();
        };
        var cut = Render<SbSimpleSelect<string>>(p => p
            .Add(x => x.Label, "Provider")
            .Add(x => x.Searchable, true)
            .Add(x => x.ChildContent, options));
        var trigger = cut.Find(".sb-select-trigger__main");
        Assert.Equal(trigger.Id, cut.Find("label").GetAttribute("for"));
        foreach (var id in trigger.GetAttribute("aria-labelledby")!.Split(' '))
            Assert.NotNull(cut.Find($"#{id}"));

        // Native buttons dispatch click for Enter. The key handler must not toggle twice.
        trigger.KeyDown(new KeyboardEventArgs { Key = "Enter" });
        trigger.Click();
        Assert.Single(cut.FindAll(".sb-select-options"));
        Assert.Equal(cut.Find(".sb-select-options").Id, trigger.GetAttribute("aria-controls"));
        Assert.False(string.IsNullOrWhiteSpace(cut.Find(".sb-select-search__input").GetAttribute("aria-label")));
        cut.Find(".sb-select-option").Click();
        Assert.Empty(cut.FindAll(".sb-select-options"));
        Assert.Contains(JSInterop.Invocations, invocation => invocation.Identifier == "SufiBlazor.focus.set");
    }

    private sealed class ConfigurationLocalizer : IStringLocalizer<SufiBlazorResource>
    {
        public LocalizedString this[string name] => new(name, name);
        public LocalizedString this[string name, params object[] arguments] => new(name, string.Format(CultureInfo.InvariantCulture, name, arguments));
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => Array.Empty<LocalizedString>();
    }
}
