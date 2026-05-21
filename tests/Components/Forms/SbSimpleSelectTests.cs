using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
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

public class SbSimpleSelectTests : BunitContext
{
    public SbSimpleSelectTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new StubStringLocalizer());
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private static RenderFragment DefaultOptions => builder =>
    {
        builder.OpenComponent<SbSelectOption<string>>(0);
        builder.AddAttribute(1, "Value", "a");
        builder.AddAttribute(2, "Text", "Option A");
        builder.CloseComponent();
        builder.OpenComponent<SbSelectOption<string>>(3);
        builder.AddAttribute(4, "Value", "b");
        builder.AddAttribute(5, "Text", "Option B");
        builder.CloseComponent();
        builder.OpenComponent<SbSelectOption<string>>(6);
        builder.AddAttribute(7, "Value", "c");
        builder.AddAttribute(8, "Text", "Option C");
        builder.CloseComponent();
    };

    private IRenderedComponent<SbSimpleSelect<string>> RenderSimpleSelect(
        Action<ComponentParameterCollectionBuilder<SbSimpleSelect<string>>>? configure = null)
    {
        return Render<SbSimpleSelect<string>>(p =>
        {
            p.AddChildContent(DefaultOptions);
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersSelectStructure()
    {
        // Arrange & Act
        var cut = RenderSimpleSelect();

        // Assert
        var anchor = cut.Find(".sb-select-anchor");
        Assert.NotNull(anchor);
        Assert.NotNull(cut.Find(".sb-select-trigger"));
        Assert.NotNull(cut.Find(".sb-select-trigger__main"));
    }

    [Fact]
    public void RendersLabelWhenProvided()
    {
        // Arrange & Act
        var cut = RenderSimpleSelect(p => p.Add(x => x.Label, "Country"));

        // Assert
        var label = cut.Find(".sb-select-label");
        Assert.NotNull(label);
        Assert.Contains("Country", label.TextContent);
    }

    [Fact]
    public void DoesNotRenderLabelWhenEmpty()
    {
        // Arrange & Act
        var cut = RenderSimpleSelect();

        // Assert
        Assert.Empty(cut.FindAll(".sb-select-label"));
    }

    [Fact]
    public void RendersPlaceholderWhenNoValue()
    {
        // Arrange & Act
        var cut = RenderSimpleSelect();

        // Assert - StubStringLocalizer returns key as value
        var placeholder = cut.Find(".sb-select-trigger__placeholder");
        Assert.NotNull(placeholder);
        Assert.Equal("Select_Placeholder", placeholder.TextContent);
    }

    [Fact]
    public void RendersCustomPlaceholderWhenProvided()
    {
        // Arrange & Act
        var cut = RenderSimpleSelect(p => p.Add(x => x.Placeholder, "Choose..."));

        // Assert
        var placeholder = cut.Find(".sb-select-trigger__placeholder");
        Assert.NotNull(placeholder);
        Assert.Contains("Choose...", placeholder.TextContent);
    }

    [Fact]
    public void DisplaysValueWhenValueSet()
    {
        // Arrange - render first so SbSelectOption children register; then set value so GetDisplayText finds them
        var cut = RenderSimpleSelect();
        cut.Render(p => p.Add(x => x.Value, "b"));

        // Assert
        var valueSpan = cut.Find(".sb-select-trigger__value");
        Assert.NotNull(valueSpan);
        Assert.Contains("Option B", valueSpan.TextContent);
    }

    [Fact]
    public void ButtonIsDisabledWhenDisabledTrue()
    {
        // Arrange & Act
        var cut = RenderSimpleSelect(p => p.Add(x => x.Disabled, true));

        // Assert
        var button = cut.Find(".sb-select-trigger__main");
        Assert.NotNull(button.GetAttribute("disabled"));
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderSimpleSelect(p => p.Add(x => x.Class, "my-select"));

        // Assert
        var anchor = cut.Find(".sb-select-anchor");
        Assert.Contains("my-select", anchor.ClassList);
    }

    [Fact]
    public async Task OpensDropdownWhenTriggerClicked()
    {
        // Arrange
        var cut = RenderSimpleSelect();

        // Act
        var trigger = cut.Find(".sb-select-trigger__main");
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
        var cut = RenderSimpleSelect();

        // Act
        await cut.InvokeAsync(() => cut.Find(".sb-select-trigger__main")!.Click());

        // Assert
        var options = cut.FindAll(".sb-select-option");
        Assert.Equal(3, options.Count);
        Assert.Contains(options, o => o.TextContent.Contains("Option A"));
    }

    [Fact]
    public async Task InvokesValueChangedWhenOptionClicked()
    {
        // Arrange
        string? received = null;
        var cut = RenderSimpleSelect(p => p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => received = v)));

        // Act
        await cut.InvokeAsync(() => cut.Find(".sb-select-trigger__main")!.Click());
        var option = cut.FindAll(".sb-select-option").First(o => o.TextContent.Contains("Option B"));
        await cut.InvokeAsync(() => option!.Click());

        // Assert
        Assert.Equal("b", received);
    }

    [Fact]
    public void RendersClearButtonWhenClearableAndValueSet()
    {
        // Arrange & Act
        var cut = RenderSimpleSelect(p => p
            .Add(x => x.Value, "a")
            .Add(x => x.Clearable, true));

        // Assert
        var clearBtn = cut.Find(".sb-select-trigger__clear");
        Assert.NotNull(clearBtn);
    }

    [Fact]
    public void DoesNotRenderClearButtonWhenClearableFalse()
    {
        // Arrange & Act
        var cut = RenderSimpleSelect(p => p
            .Add(x => x.Value, "a")
            .Add(x => x.Clearable, false));

        // Assert
        Assert.Empty(cut.FindAll(".sb-select-trigger__clear"));
    }

    [Fact]
    public async Task InvokesValueChangedWhenClearClicked()
    {
        // Arrange
        string? received = "x";
        var cut = RenderSimpleSelect(p => p
            .Add(x => x.Value, "a")
            .Add(x => x.Clearable, true)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => received = v)));

        // Act
        var clearBtn = cut.Find(".sb-select-trigger__clear");
        await cut.InvokeAsync(() => clearBtn!.Click());

        // Assert
        Assert.Null(received);
    }

    [Fact]
    public async Task RendersSearchInputWhenSearchable()
    {
        // Arrange & Act
        var cut = RenderSimpleSelect(p => p.Add(x => x.Searchable, true));
        await cut.InvokeAsync(() => cut.Find(".sb-select-trigger__main")!.Click());

        // Assert
        var searchInput = cut.Find(".sb-select-search__input");
        Assert.NotNull(searchInput);
        Assert.Equal("Search_Placeholder", searchInput.GetAttribute("placeholder"));
    }

    [Fact]
    public async Task FiltersOptionsWhenSearching()
    {
        // Arrange
        var cut = RenderSimpleSelect(p => p.Add(x => x.Searchable, true));
        await cut.InvokeAsync(() => cut.Find(".sb-select-trigger__main")!.Click());

        // Act
        var searchInput = cut.Find(".sb-select-search__input");
        await cut.InvokeAsync(() => searchInput!.Input("Option B"));

        // Assert
        var options = cut.FindAll(".sb-select-option");
        Assert.Single(options);
        Assert.Contains("Option B", options[0].TextContent);
    }

    [Fact]
    public async Task ShowsNoResultsWhenSearchHasNoMatch()
    {
        // Arrange
        var cut = RenderSimpleSelect(p => p.Add(x => x.Searchable, true));
        await cut.InvokeAsync(() => cut.Find(".sb-select-trigger__main")!.Click());

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
        var cut = RenderSimpleSelect(p => p.Add(x => x.Value, "b"));

        // Act
        await cut.InvokeAsync(() => cut.Find(".sb-select-trigger__main")!.Click());

        // Assert
        var options = cut.FindAll(".sb-select-option");
        var selectedOption = options.FirstOrDefault(o => o.ClassList.Contains("sb-select-option--selected"));
        Assert.NotNull(selectedOption);
        Assert.Contains("Option B", selectedOption.TextContent);
    }

    [Fact]
    public async Task DisabledOptionHasDisabledClass()
    {
        // Arrange
        var optionsWithDisabled = (RenderFragment)(b =>
        {
            b.OpenComponent<SbSelectOption<string>>(0);
            b.AddAttribute(1, "Value", "a");
            b.AddAttribute(2, "Text", "Option A");
            b.CloseComponent();
            b.OpenComponent<SbSelectOption<string>>(3);
            b.AddAttribute(4, "Value", "b");
            b.AddAttribute(5, "Text", "Option B");
            b.AddAttribute(6, "Disabled", true);
            b.CloseComponent();
        });
        var cut = Render<SbSimpleSelect<string>>(p => p
            .AddChildContent(optionsWithDisabled));

        // Act
        await cut.InvokeAsync(() => cut.Find(".sb-select-trigger__main")!.Click());

        // Assert
        var options = cut.FindAll(".sb-select-option");
        var disabledOption = options.FirstOrDefault(o => o.ClassList.Contains("sb-select-option--disabled"));
        Assert.NotNull(disabledOption);
        Assert.Contains("Option B", disabledOption.TextContent);
    }

    [Fact]
    public void AppliesStyleParameter()
    {
        // Arrange & Act
        var cut = RenderSimpleSelect(p => p.Add(x => x.Style, "max-width: 300px;"));

        // Assert
        var anchor = cut.Find(".sb-select-anchor");
        Assert.Contains("max-width: 300px", anchor.GetAttribute("style"));
    }
}
