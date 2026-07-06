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

public class SbSelectTests : BunitContext
{
    public SbSelectTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new StubStringLocalizer());
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private static List<SelectTestItem> CreateItems(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => new SelectTestItem { Id = i, Name = $"Item {i}" })
            .ToList();
    }

    private IRenderedComponent<SbSelect<SelectTestItem, int>> RenderSelect(
        Action<ComponentParameterCollectionBuilder<SbSelect<SelectTestItem, int>>>? configure = null)
    {
        var items = CreateItems(5);
        return Render<SbSelect<SelectTestItem, int>>(p =>
        {
            p.Add(x => x.Items, items)
                .Add(x => x.TextField, (Func<SelectTestItem, string>)(item => item.Name))
                .Add(x => x.ValueField, (Func<SelectTestItem, int>)(item => item.Id));
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersSelectStructure()
    {
        // Arrange & Act
        var cut = RenderSelect();

        // Assert
        var anchor = cut.Find(".sb-select-anchor");
        Assert.NotNull(anchor);
        Assert.NotNull(cut.Find(".sb-select-trigger"));
    }

    [Fact]
    public void RendersLabelWhenProvided()
    {
        // Arrange & Act
        var cut = RenderSelect(p => p
            .Add(x => x.Label, "Country")
            .Add(x => x.Id, "country"));

        // Assert
        var label = cut.Find(".sb-select__label");
        Assert.NotNull(label);
        Assert.Contains("Country", label.TextContent);
    }

    [Fact]
    public void DoesNotRenderLabelWhenEmpty()
    {
        // Arrange & Act
        var cut = RenderSelect();

        // Assert
        Assert.Empty(cut.FindAll(".sb-select__label"));
    }

    [Fact]
    public void RendersRequiredAsteriskWhenRequired()
    {
        // Arrange & Act
        var cut = RenderSelect(p => p
            .Add(x => x.Label, "Option")
            .Add(x => x.Required, true));

        // Assert
        var required = cut.Find(".sb-select__required");
        Assert.NotNull(required);
        Assert.Contains("*", required.TextContent);
    }

    [Fact]
    public void RendersPlaceholderWhenNoValue()
    {
        // Arrange & Act
        var cut = RenderSelect();

        // Assert - StubStringLocalizer returns key as value
        var placeholder = cut.Find(".sb-select-trigger__placeholder");
        Assert.NotNull(placeholder);
        Assert.Equal("Select_Placeholder", placeholder.TextContent);
    }

    [Fact]
    public void RendersCustomPlaceholderWhenProvided()
    {
        // Arrange & Act
        var cut = RenderSelect(p => p.Add(x => x.Placeholder, "Choose an option..."));

        // Assert
        var placeholder = cut.Find(".sb-select-trigger__placeholder");
        Assert.NotNull(placeholder);
        Assert.Contains("Choose an option...", placeholder.TextContent);
    }

    [Fact]
    public void DisplaysValueWhenValueSet()
    {
        // Arrange
        var items = CreateItems(3);

        // Act
        var cut = Render<SbSelect<SelectTestItem, int>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.Value, 2)
            .Add(x => x.TextField, (Func<SelectTestItem, string>)(item => item.Name))
            .Add(x => x.ValueField, (Func<SelectTestItem, int>)(item => item.Id)));

        // Assert
        var valueSpan = cut.Find(".sb-select-trigger__value");
        Assert.NotNull(valueSpan);
        Assert.Contains("Item 2", valueSpan.TextContent);
    }

    [Fact]
    public void ButtonIsDisabledWhenDisabledTrue()
    {
        // Arrange & Act
        var cut = RenderSelect(p => p.Add(x => x.Disabled, true));

        // Assert
        var trigger = cut.Find(".sb-select-trigger");
        Assert.NotNull(trigger.GetAttribute("disabled"));
    }

    [Fact]
    public async Task OpensDropdownWhenTriggerClicked()
    {
        // Arrange
        var cut = RenderSelect();

        // Act
        var trigger = cut.Find(".sb-select-trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        // Assert
        var dropdown = cut.Find(".sb-select-dropdown");
        Assert.NotNull(dropdown);
        Assert.Equal("listbox", dropdown.GetAttribute("role"));
    }

    [Fact]
    public async Task ShowsOptionsWhenOpen()
    {
        // Arrange
        var cut = RenderSelect();

        // Act
        await cut.InvokeAsync(() => cut.Find(".sb-select-trigger")!.Click());

        // Assert
        var options = cut.FindAll(".sb-select-option");
        Assert.Equal(5, options.Count);
        Assert.Contains(options, o => o.TextContent.Contains("Item 1"));
    }

    [Fact]
    public async Task InvokesValueChangedWhenOptionClicked()
    {
        // Arrange - use string to avoid nullable value type EventCallback overload issues
        string? received = null;
        var items = new List<SelectTestItem> { new() { Id = 1, Name = "A" }, new() { Id = 2, Name = "B" } };
        var cut = Render<SbSelect<SelectTestItem, string>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.TextField, (Func<SelectTestItem, string>)(i => i.Name))
            .Add(x => x.ValueField, (Func<SelectTestItem, string>)(i => i.Name))
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => received = v)));

        // Act
        await cut.InvokeAsync(() => cut.Find(".sb-select-trigger")!.Click());
        var option = cut.FindAll(".sb-select-option").First(o => o.TextContent.Contains("B"));
        await cut.InvokeAsync(() => option!.Click());

        // Assert
        Assert.Equal("B", received);
    }

    [Fact]
    public async Task RendersSearchInputWhenSearchable()
    {
        // Arrange & Act
        var cut = RenderSelect(p => p.Add(x => x.Searchable, true));
        await cut.InvokeAsync(() => cut.Find(".sb-select-trigger")!.Click());

        // Assert
        var searchInput = cut.Find(".sb-select-search__input");
        Assert.NotNull(searchInput);
        Assert.Equal("Search_Placeholder", searchInput.GetAttribute("placeholder"));
    }

    [Fact]
    public async Task FiltersOptionsWhenSearching()
    {
        // Arrange
        var cut = RenderSelect(p => p.Add(x => x.Searchable, true));
        await cut.InvokeAsync(() => cut.Find(".sb-select-trigger")!.Click());

        // Act
        var searchInput = cut.Find(".sb-select-search__input");
        await cut.InvokeAsync(() => searchInput!.Input("Item 2"));

        // Assert
        var options = cut.FindAll(".sb-select-option");
        Assert.Single(options);
        Assert.Contains("Item 2", options[0].TextContent);
    }

    [Fact]
    public async Task ShowsNoResultsWhenSearchHasNoMatch()
    {
        // Arrange
        var cut = RenderSelect(p => p.Add(x => x.Searchable, true));
        await cut.InvokeAsync(() => cut.Find(".sb-select-trigger")!.Click());

        // Act
        var searchInput = cut.Find(".sb-select-search__input");
        await cut.InvokeAsync(() => searchInput!.Input("xyznonexistent"));

        // Assert
        var empty = cut.Find(".sb-select-empty");
        Assert.NotNull(empty);
        Assert.Contains("NoResultsFound", empty.TextContent);
    }

    [Fact]
    public async Task SelectedOptionHasSelectedClass()
    {
        // Arrange
        var cut = RenderSelect(p => p.Add(x => x.Value, 2));

        // Act
        await cut.InvokeAsync(() => cut.Find(".sb-select-trigger")!.Click());

        // Assert
        var options = cut.FindAll(".sb-select-option");
        var selectedOption = options.FirstOrDefault(o => o.ClassList.Contains("sb-select-option--selected"));
        Assert.NotNull(selectedOption);
        Assert.Contains("Item 2", selectedOption.TextContent);
    }

    [Fact]
    public void DisplaysItemTemplateInTriggerWhenSet()
    {
        // Arrange
        var items = CreateItems(3);

        // Act
        var cut = Render<SbSelect<SelectTestItem, int>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.Value, 2)
            .Add(x => x.TextField, (Func<SelectTestItem, string>)(item => item.Name))
            .Add(x => x.ValueField, (Func<SelectTestItem, int>)(item => item.Id))
            .Add(x => x.ItemTemplate, (RenderFragment<SelectTestItem>)(item => builder =>
                builder.AddContent(0, $"Localized {item.Name}"))));

        // Assert
        var valueSpan = cut.Find(".sb-select-trigger__value");
        Assert.NotNull(valueSpan);
        Assert.Contains("Localized Item 2", valueSpan.TextContent);
    }

    [Fact]
    public void TriggerHasAriaAttributes()
    {
        // Arrange & Act
        var cut = RenderSelect(p => p.Add(x => x.Id, "select-id"));

        // Assert
        var trigger = cut.Find(".sb-select-trigger");
        Assert.Equal("listbox", trigger.GetAttribute("aria-haspopup"));
        Assert.Equal("select-id", trigger.GetAttribute("aria-labelledby"));
    }

    private class SelectTestItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
