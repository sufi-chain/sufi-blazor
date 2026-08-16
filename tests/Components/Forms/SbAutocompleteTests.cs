using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Bunit;
using Bunit.JSInterop;
using SufiChain.SufiBlazor.Components.Forms;
using SufiChain.SufiBlazor.Localization;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Forms;

/// <summary>
/// Stub localizer that returns the key as the value (for testing).
/// </summary>
file class StubStringLocalizer : IStringLocalizer<SufiBlazorResource>
{
    public LocalizedString this[string name] => new(name, name);
    public LocalizedString this[string name, params object[] arguments] => new(name, string.Format(CultureInfo.InvariantCulture, name, arguments));
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => Array.Empty<LocalizedString>();
}

public class SbAutocompleteTests : BunitContext
{
    public SbAutocompleteTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new StubStringLocalizer());
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private static List<AutocompleteTestItem> CreateItems(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => new AutocompleteTestItem { Id = i, Name = $"Item {i}" })
            .ToList();
    }

    private IRenderedComponent<SbAutocomplete<AutocompleteTestItem>> RenderAutocomplete(
        Action<ComponentParameterCollectionBuilder<SbAutocomplete<AutocompleteTestItem>>>? configure = null)
    {
        var items = CreateItems(5);
        return Render<SbAutocomplete<AutocompleteTestItem>>(p =>
        {
            p.Add(x => x.Items, items)
                .Add(x => x.TextField, (Func<AutocompleteTestItem, string>)(item => item.Name))
                .Add(x => x.ValueField, (Func<AutocompleteTestItem, object>)(item => item.Id))
                .Add(x => x.DebounceMs, 0); // No debounce for faster tests
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersAutocompleteStructure()
    {
        // Arrange & Act
        var cut = RenderAutocomplete();

        // Assert
        var wrapper = cut.Find(".sb-autocomplete");
        Assert.NotNull(wrapper);
        Assert.NotNull(cut.Find(".sb-autocomplete__input-wrapper"));
        Assert.NotNull(cut.Find(".sb-autocomplete__input"));
        Assert.NotNull(cut.Find(".sb-autocomplete__icon"));
    }

    [Fact]
    public void RendersLabelWhenProvided()
    {
        // Arrange & Act
        var cut = RenderAutocomplete(p => p.Add(x => x.Label, "Search"));

        // Assert
        var label = cut.Find(".sb-autocomplete__label");
        Assert.NotNull(label);
        Assert.Contains("Search", label.TextContent);
    }

    [Fact]
    public void DoesNotRenderLabelWhenEmpty()
    {
        // Arrange & Act
        var cut = RenderAutocomplete();

        // Assert
        Assert.Empty(cut.FindAll(".sb-autocomplete__label"));
    }

    [Fact]
    public void RendersRequiredAsteriskWhenRequired()
    {
        // Arrange & Act
        var cut = RenderAutocomplete(p => p
            .Add(x => x.Label, "Name")
            .Add(x => x.Required, true));

        // Assert
        var required = cut.Find(".sb-autocomplete__required");
        Assert.NotNull(required);
        Assert.Contains("*", required.TextContent);
    }

    [Fact]
    public void RendersPlaceholderWhenProvided()
    {
        // Arrange & Act
        var cut = RenderAutocomplete(p => p.Add(x => x.Placeholder, "Type to search..."));

        // Assert
        var input = cut.Find(".sb-autocomplete__input");
        Assert.Equal("Type to search...", input.GetAttribute("placeholder"));
    }

    [Fact]
    public void UsesLocalizedPlaceholderWhenPlaceholderNull()
    {
        // Arrange & Act
        var cut = RenderAutocomplete();

        // Assert - StubStringLocalizer returns key as value, so "Search_Placeholder"
        var input = cut.Find(".sb-autocomplete__input");
        Assert.Equal("Search_Placeholder", input.GetAttribute("placeholder"));
    }

    [Fact]
    public void InputIsDisabledWhenDisabledTrue()
    {
        // Arrange & Act
        var cut = RenderAutocomplete(p => p.Add(x => x.Disabled, true));

        // Assert
        var input = cut.Find(".sb-autocomplete__input");
        Assert.True(input.GetAttribute("disabled") != null || input.OuterHtml.Contains("disabled"));
    }

    [Fact]
    public void DisplaysValueWhenValueSet()
    {
        // Arrange
        var items = CreateItems(3);
        var selected = items[1];

        // Act
        var cut = Render<SbAutocomplete<AutocompleteTestItem>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.Value, selected)
            .Add(x => x.TextField, (Func<AutocompleteTestItem, string>)(item => item.Name))
            .Add(x => x.ValueField, (Func<AutocompleteTestItem, object>)(item => item.Id))
            .Add(x => x.DebounceMs, 0));

        // Assert
        var input = cut.Find(".sb-autocomplete__input");
        Assert.Equal("Item 2", input.GetAttribute("value"));
    }

    [Fact]
    public void RapidInputPreservesLocalSearchDuringRerenders()
    {
        var items = CreateItems(3);
        var cut = Render<SbAutocomplete<AutocompleteTestItem>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.Value, items[0])
            .Add(x => x.TextField, (Func<AutocompleteTestItem, string>)(item => item.Name))
            .Add(x => x.ValueField, (Func<AutocompleteTestItem, object>)(item => item.Id))
            .Add(x => x.DebounceMs, 0));

        var input = cut.Find(".sb-autocomplete__input");
        input.Input("I");
        cut.Render(parameters => parameters.Add(x => x.Value, items[0]));
        input.Input("It");
        cut.Render(parameters => parameters.Add(x => x.Value, items[0]));
        input.Input("Item");
        cut.Render(parameters => parameters.Add(x => x.Value, items[0]));
        input.Input("Item 2");
        cut.Render(parameters => parameters.Add(x => x.Value, items[0]));

        Assert.Equal("Item 2", cut.Find(".sb-autocomplete__input").GetAttribute("value"));
    }

    [Fact]
    public void AppliesCustomClass()
    {
        // Arrange & Act
        var cut = RenderAutocomplete(p => p.Add(x => x.Class, "my-autocomplete"));

        // Assert
        var wrapper = cut.Find(".sb-autocomplete");
        Assert.Contains("my-autocomplete", wrapper.ClassList);
    }

    [Fact]
    public void AppliesInlineStyle()
    {
        // Arrange & Act
        var cut = RenderAutocomplete(p => p.Add(x => x.Style, "max-width: 300px;"));

        // Assert
        var wrapper = cut.Find(".sb-autocomplete");
        Assert.Contains("max-width: 300px", wrapper.GetAttribute("style"));
    }

    [Fact]
    public async Task InvokesValueChangedWhenItemSelected()
    {
        // Arrange
        AutocompleteTestItem? selected = null;
        var items = CreateItems(3);

        var cut = Render<SbAutocomplete<AutocompleteTestItem>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.TextField, (Func<AutocompleteTestItem, string>)(item => item.Name))
            .Add(x => x.ValueField, (Func<AutocompleteTestItem, object>)(item => item.Id))
            .Add(x => x.DebounceMs, 0)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<AutocompleteTestItem?>(this, item => selected = item)));

        // Act - type to open dropdown (SbAutocomplete uses @oninput, not @onchange), then select
        var input = cut.Find(".sb-autocomplete__input");
        await cut.InvokeAsync(() => input!.Input("Item"));
        cut.WaitForState(() => cut.FindAll(".sb-autocomplete__option").Count > 0);

        var option = cut.FindAll(".sb-autocomplete__option").FirstOrDefault(o => o.TextContent.Contains("Item 1"));
        Assert.NotNull(option);
        await cut.InvokeAsync(() => option!.Click());

        cut.WaitForState(() => selected != null);

        // Assert
        Assert.NotNull(selected);
        Assert.Equal(1, selected!.Id);
        Assert.Equal("Item 1", selected.Name);
    }

    [Fact]
    public async Task ClearsValueWhenClearButtonClicked()
    {
        // Arrange
        var items = CreateItems(2);
        var selected = items[0];

        var cut = Render<SbAutocomplete<AutocompleteTestItem>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.Value, selected)
            .Add(x => x.TextField, (Func<AutocompleteTestItem, string>)(item => item.Name))
            .Add(x => x.ValueField, (Func<AutocompleteTestItem, object>)(item => item.Id))
            .Add(x => x.Clearable, true)
            .Add(x => x.DebounceMs, 0));

        cut.WaitForState(() => cut.Find(".sb-autocomplete__input")?.GetAttribute("value") == "Item 1");

        // Act
        var clearBtn = cut.Find(".sb-autocomplete__clear");
        Assert.NotNull(clearBtn);
        await cut.InvokeAsync(() => clearBtn!.Click());

        cut.WaitForState(() => cut.Find(".sb-autocomplete__input")?.GetAttribute("value") == "");

        // Assert
        var input = cut.Find(".sb-autocomplete__input");
        Assert.Equal("", input.GetAttribute("value"));
    }

    private class AutocompleteTestItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
