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

public class SbMultiSelectTests : BunitContext
{
    public SbMultiSelectTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new StubStringLocalizer());
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private static List<MultiSelectTestItem> CreateItems(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => new MultiSelectTestItem { Id = i, Name = $"Item {i}" })
            .ToList();
    }

    private IRenderedComponent<SbMultiSelect<MultiSelectTestItem, int>> RenderMultiSelect(
        Action<ComponentParameterCollectionBuilder<SbMultiSelect<MultiSelectTestItem, int>>>? configure = null)
    {
        var items = CreateItems(5);
        return Render<SbMultiSelect<MultiSelectTestItem, int>>(p =>
        {
            p.Add(x => x.Items, items)
                .Add(x => x.TextField, (Func<MultiSelectTestItem, string>)(item => item.Name))
                .Add(x => x.ValueField, (Func<MultiSelectTestItem, int>)(item => item.Id));
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersMultiSelectStructure()
    {
        // Arrange & Act
        var cut = RenderMultiSelect();

        // Assert
        var anchor = cut.Find(".sb-multi-select-anchor");
        Assert.NotNull(anchor);
        Assert.NotNull(cut.Find(".sb-multi-select-trigger"));
    }

    [Fact]
    public void RendersLabelWhenProvided()
    {
        // Arrange & Act
        var cut = RenderMultiSelect(p => p
            .Add(x => x.Label, "Skills")
            .Add(x => x.Id, "skills"));

        // Assert
        var label = cut.Find(".sb-multi-select__label");
        Assert.NotNull(label);
        Assert.Contains("Skills", label.TextContent);
    }

    [Fact]
    public void DoesNotRenderLabelWhenEmpty()
    {
        // Arrange & Act
        var cut = RenderMultiSelect();

        // Assert
        Assert.Empty(cut.FindAll(".sb-multi-select__label"));
    }

    [Fact]
    public void RendersRequiredAsteriskWhenRequired()
    {
        // Arrange & Act
        var cut = RenderMultiSelect(p => p
            .Add(x => x.Label, "Tags")
            .Add(x => x.Required, true));

        // Assert
        var required = cut.Find(".sb-multi-select__required");
        Assert.NotNull(required);
        Assert.Contains("*", required.TextContent);
    }

    [Fact]
    public void RendersPlaceholderWhenNoValues()
    {
        // Arrange & Act
        var cut = RenderMultiSelect();

        // Assert - StubStringLocalizer returns key as value
        var placeholder = cut.Find(".sb-multi-select__placeholder");
        Assert.NotNull(placeholder);
        Assert.Equal("Select_Placeholder", placeholder.TextContent);
    }

    [Fact]
    public void RendersCustomPlaceholderWhenProvided()
    {
        // Arrange & Act
        var cut = RenderMultiSelect(p => p.Add(x => x.Placeholder, "Choose items..."));

        // Assert
        var placeholder = cut.Find(".sb-multi-select__placeholder");
        Assert.NotNull(placeholder);
        Assert.Contains("Choose items...", placeholder.TextContent);
    }

    [Fact]
    public void RendersChipsWhenValuesSet()
    {
        // Arrange
        var items = CreateItems(3);

        // Act
        var cut = Render<SbMultiSelect<MultiSelectTestItem, int>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.Values, new List<int> { 1, 2 })
            .Add(x => x.TextField, (Func<MultiSelectTestItem, string>)(item => item.Name))
            .Add(x => x.ValueField, (Func<MultiSelectTestItem, int>)(item => item.Id)));

        // Assert
        var chips = cut.FindAll(".sb-multi-select__chip");
        Assert.Equal(2, chips.Count);
        Assert.Contains("Item 1", cut.Markup);
        Assert.Contains("Item 2", cut.Markup);
        Assert.NotNull(cut.FindAll(".sb-multi-select__chip-remove"));
    }

    [Fact]
    public void RendersItemTemplateInChipsWhenSet()
    {
        // Arrange
        var items = CreateItems(3);

        // Act
        var cut = Render<SbMultiSelect<MultiSelectTestItem, int>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.Values, new List<int> { 1, 2 })
            .Add(x => x.TextField, (Func<MultiSelectTestItem, string>)(item => item.Name))
            .Add(x => x.ValueField, (Func<MultiSelectTestItem, int>)(item => item.Id))
            .Add(x => x.ItemTemplate, (RenderFragment<MultiSelectTestItem>)(item => builder =>
                builder.AddContent(0, $"Localized {item.Name}"))));

        // Assert
        var chips = cut.FindAll(".sb-multi-select__chip");
        Assert.Equal(2, chips.Count);
        Assert.Contains("Localized Item 1", cut.Markup);
        Assert.Contains("Localized Item 2", cut.Markup);
    }

    [Fact]
    public void AppliesDisabledClassWhenDisabled()
    {
        // Arrange & Act
        var cut = RenderMultiSelect(p => p.Add(x => x.Disabled, true));

        // Assert
        var trigger = cut.Find(".sb-multi-select-trigger");
        Assert.Contains("sb-multi-select--disabled", trigger.ClassList);
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderMultiSelect(p => p.Add(x => x.Class, "my-multiselect"));

        // Assert
        var anchor = cut.Find(".sb-multi-select-anchor");
        Assert.Contains("my-multiselect", anchor.ClassList);
    }

    [Fact]
    public void AppliesStyleParameter()
    {
        // Arrange & Act
        var cut = RenderMultiSelect(p => p.Add(x => x.Style, "max-width: 400px;"));

        // Assert
        var anchor = cut.Find(".sb-multi-select-anchor");
        Assert.Contains("max-width: 400px", anchor.GetAttribute("style"));
    }

    [Fact]
    public async Task OpensDropdownWhenTriggerClicked()
    {
        // Arrange
        var cut = RenderMultiSelect();

        // Act
        var trigger = cut.Find(".sb-multi-select-trigger");
        await cut.InvokeAsync(() => trigger!.Click());

        // Assert
        var dropdown = cut.Find(".sb-select-dropdown");
        Assert.NotNull(dropdown);
        Assert.Equal("listbox", dropdown.GetAttribute("role"));
        Assert.Equal("true", dropdown.GetAttribute("aria-multiselectable"));
    }

    [Fact]
    public async Task ShowsOptionsWhenOpen()
    {
        // Arrange
        var cut = RenderMultiSelect();

        // Act
        await cut.InvokeAsync(() => cut.Find(".sb-multi-select-trigger")!.Click());

        // Assert
        var options = cut.FindAll(".sb-select-option");
        Assert.Equal(5, options.Count);
        Assert.Contains(options, o => o.TextContent.Contains("Item 1"));
    }

    [Fact]
    public async Task InvokesValuesChangedWhenOptionClicked()
    {
        // Arrange
        IReadOnlyList<int>? received = null;
        var cut = Render<SbMultiSelect<MultiSelectTestItem, int>>(p => p
            .Add(x => x.Items, CreateItems(3))
            .Add(x => x.TextField, (Func<MultiSelectTestItem, string>)(item => item.Name))
            .Add(x => x.ValueField, (Func<MultiSelectTestItem, int>)(item => item.Id))
            .Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<int>>(this, v => received = v)));

        // Act
        await cut.InvokeAsync(() => cut.Find(".sb-multi-select-trigger")!.Click());
        var option = cut.FindAll(".sb-select-option").First(o => o.TextContent.Contains("Item 1"));
        await cut.InvokeAsync(() => option!.Click());

        // Assert
        Assert.NotNull(received);
        Assert.Single(received!);
        Assert.Equal(1, received![0]);
    }

    [Fact]
    public async Task DoesNotSelectDisabledItems()
    {
        IReadOnlyList<int>? received = null;
        var cut = Render<SbMultiSelect<MultiSelectTestItem, int>>(p => p
            .Add(x => x.Items, CreateItems(3))
            .Add(x => x.TextField, (Func<MultiSelectTestItem, string>)(item => item.Name))
            .Add(x => x.ValueField, (Func<MultiSelectTestItem, int>)(item => item.Id))
            .Add(x => x.ItemDisabledField, (Func<MultiSelectTestItem, bool>)(item => item.Id == 2))
            .Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<int>>(this, v => received = v)));

        await cut.InvokeAsync(() => cut.Find(".sb-multi-select-trigger")!.Click());
        var disabled = cut.FindAll(".sb-select-option").First(o => o.TextContent.Contains("Item 2"));
        Assert.Contains("sb-select-option--disabled", disabled.ClassList);
        Assert.True(disabled.HasAttribute("disabled"));

        await cut.InvokeAsync(() => disabled.Click());
        Assert.Null(received);
    }

    [Fact]
    public async Task InvokesValuesChangedWhenChipRemoveClicked()
    {
        // Arrange
        IReadOnlyList<int>? received = null;
        var items = CreateItems(3);
        var cut = Render<SbMultiSelect<MultiSelectTestItem, int>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.Values, new List<int> { 1, 2 })
            .Add(x => x.TextField, (Func<MultiSelectTestItem, string>)(item => item.Name))
            .Add(x => x.ValueField, (Func<MultiSelectTestItem, int>)(item => item.Id))
            .Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<int>>(this, v => received = v)));

        // Act - click remove on first chip (Item 1, Id=1)
        var removeButtons = cut.FindAll(".sb-multi-select__chip-remove");
        await cut.InvokeAsync(() => removeButtons[0].Click());

        // Assert
        Assert.NotNull(received);
        Assert.Single(received!);
        Assert.Equal(2, received![0]); // Only Item 2 (Id=2) remains
    }

    [Fact]
    public async Task RendersSearchInputWhenSearchable()
    {
        // Arrange & Act
        var cut = RenderMultiSelect(p => p.Add(x => x.Searchable, true));
        await cut.InvokeAsync(() => cut.Find(".sb-multi-select-trigger")!.Click());

        // Assert
        var searchInput = cut.Find(".sb-select-search__input");
        Assert.NotNull(searchInput);
        Assert.Equal("Search_Placeholder", searchInput.GetAttribute("placeholder"));
    }

    [Fact]
    public async Task FiltersOptionsWhenSearching()
    {
        // Arrange
        var cut = RenderMultiSelect(p => p.Add(x => x.Searchable, true));
        await cut.InvokeAsync(() => cut.Find(".sb-multi-select-trigger")!.Click());

        // Act
        var searchInput = cut.Find(".sb-select-search__input");
        await cut.InvokeAsync(() => searchInput!.Input("Item 2"));

        // Assert
        var options = cut.FindAll(".sb-select-option");
        Assert.Single(options);
        Assert.Contains("Item 2", options[0].TextContent);
    }

    [Fact]
    public async Task RapidSearchInputPreservesCompleteBrowserValue()
    {
        var cut = RenderMultiSelect(p => p.Add(x => x.Searchable, true));
        await cut.InvokeAsync(() => cut.Find(".sb-multi-select-trigger").Click());

        var searchInput = cut.Find(".sb-select-search__input");
        searchInput.Input("I");
        searchInput.Input("It");
        searchInput.Input("Item");
        searchInput.Input("Item 2");

        Assert.Equal("Item 2", cut.Find(".sb-select-search__input").GetAttribute("value"));
        Assert.Single(cut.FindAll(".sb-select-option"));
    }

    [Fact]
    public async Task ShowsNoResultsWhenSearchHasNoMatch()
    {
        // Arrange
        var cut = RenderMultiSelect(p => p.Add(x => x.Searchable, true));
        await cut.InvokeAsync(() => cut.Find(".sb-multi-select-trigger")!.Click());

        // Act
        var searchInput = cut.Find(".sb-select-search__input");
        await cut.InvokeAsync(() => searchInput!.Input("xyznonexistent"));

        // Assert
        var empty = cut.Find(".sb-select-empty");
        Assert.NotNull(empty);
        Assert.Contains("NoResultsFound", empty.TextContent);
    }

    [Fact]
    public async Task RespectsMaxSelected()
    {
        // Arrange - start with 2 values, max 2, so clicking Item 3 should not add
        IReadOnlyList<int>? received = null;
        var cut = Render<SbMultiSelect<MultiSelectTestItem, int>>(p => p
            .Add(x => x.Items, CreateItems(5))
            .Add(x => x.Values, new List<int> { 1, 2 })
            .Add(x => x.MaxSelected, 2)
            .Add(x => x.TextField, (Func<MultiSelectTestItem, string>)(item => item.Name))
            .Add(x => x.ValueField, (Func<MultiSelectTestItem, int>)(item => item.Id))
            .Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<int>>(this, v => received = v)));

        // Act - open and try to select Item 3 (already at max)
        await cut.InvokeAsync(() => cut.Find(".sb-multi-select-trigger")!.Click());
        var option3 = cut.FindAll(".sb-select-option").First(o => o.TextContent.Contains("Item 3"));
        await cut.InvokeAsync(() => option3!.Click());

        // Assert - ValuesChanged should not have been called
        Assert.Null(received);
    }

    [Fact]
    public void UsesCustomRemoveAriaLabel()
    {
        // Arrange
        var items = CreateItems(2);

        // Act
        var cut = Render<SbMultiSelect<MultiSelectTestItem, int>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.Values, new List<int> { 1 })
            .Add(x => x.TextField, (Func<MultiSelectTestItem, string>)(item => item.Name))
            .Add(x => x.ValueField, (Func<MultiSelectTestItem, int>)(item => item.Id))
            .Add(x => x.RemoveAriaLabel, "Remove item"));

        // Assert
        var removeBtn = cut.Find(".sb-multi-select__chip-remove");
        Assert.NotNull(removeBtn);
        Assert.Equal("Remove item", removeBtn.GetAttribute("aria-label"));
    }

    private class MultiSelectTestItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
