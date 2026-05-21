using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Bunit;
using SufiChain.SufiBlazor.Components.Data;
using SufiChain.SufiBlazor.Contracts.Data;
using SufiChain.SufiBlazor.Localization;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Data;

/// <summary>
/// Stub localizer that returns the key as the value (for testing).
/// </summary>
file class StubStringLocalizer : IStringLocalizer<SufiBlazorResource>
{
    public LocalizedString this[string name] => new(name, name);
    public LocalizedString this[string name, params object[] arguments] => new(name, string.Format(CultureInfo.InvariantCulture, name, arguments));
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => Array.Empty<LocalizedString>();
}

public class SbDataGridTests : BunitContext
{
    public SbDataGridTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new StubStringLocalizer());
    }

    private static List<TestItem> CreateTestItems(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => new TestItem { Id = i, Name = $"Item {i}", Value = i * 10 })
            .ToList();
    }

    private static RenderFragment ColumnsTemplate => builder =>
    {
        builder.OpenComponent<SbColumn<TestItem>>(0);
        builder.AddAttribute(1, "Field", "Name");
        builder.AddAttribute(2, "Title", "Name");
        builder.CloseComponent();
        builder.OpenComponent<SbColumn<TestItem>>(3);
        builder.AddAttribute(4, "Field", "Value");
        builder.AddAttribute(5, "Title", "Value");
        builder.AddAttribute(6, "FieldType", typeof(int));
        builder.CloseComponent();
    };

    private static RenderFragment SortableColumnsTemplate => builder =>
    {
        builder.OpenComponent<SbColumn<TestItem>>(0);
        builder.AddAttribute(1, "Field", "Name");
        builder.AddAttribute(2, "Title", "Name");
        builder.AddAttribute(3, "Sortable", true);
        builder.CloseComponent();
        builder.OpenComponent<SbColumn<TestItem>>(4);
        builder.AddAttribute(5, "Field", "Value");
        builder.AddAttribute(6, "Title", "Value");
        builder.AddAttribute(7, "FieldType", typeof(int));
        builder.AddAttribute(8, "Sortable", true);
        builder.CloseComponent();
    };

    private IRenderedComponent<SbDataGrid<TestItem>> RenderDataGrid(
        List<TestItem>? items = null,
        Action<ComponentParameterCollectionBuilder<SbDataGrid<TestItem>>>? configure = null)
    {
        items ??= CreateTestItems(3);
        return Render<SbDataGrid<TestItem>>(p =>
        {
            p.Add(x => x.Items, items)
                .Add(x => x.KeySelector, (Func<TestItem, string>)(item => item.Id.ToString()))
                .Add(x => x.ShowPagination, false)
                .Add(x => x.ShowColumnFilters, false)
                .AddChildContent(ColumnsTemplate);
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersGridWithTableStructure()
    {
        // Arrange & Act
        var cut = RenderDataGrid();

        // Assert
        var grid = cut.Find(".sb-datagrid");
        Assert.NotNull(grid);
        Assert.NotNull(cut.Find("table.sb-datagrid__table"));
        Assert.NotNull(cut.Find("thead.sb-datagrid__header"));
        Assert.NotNull(cut.Find("tbody.sb-datagrid__body"));
    }

    [Fact]
    public void RendersColumnHeaders()
    {
        // Arrange & Act
        var cut = RenderDataGrid();

        // Assert
        Assert.Contains("Name", cut.Markup);
        Assert.Contains("Value", cut.Markup);
    }

    [Fact]
    public void RendersDataRows()
    {
        // Arrange
        var items = CreateTestItems(2);

        // Act
        var cut = RenderDataGrid(items);

        // Assert
        Assert.Contains("Item 1", cut.Markup);
        Assert.Contains("Item 2", cut.Markup);
        Assert.Contains("10", cut.Markup);
        Assert.Contains("20", cut.Markup);
    }

    [Fact]
    public void RendersEmptyStateWhenNoItems()
    {
        // Arrange & Act
        var cut = RenderDataGrid(new List<TestItem>());

        // Assert
        Assert.Contains("No data available", cut.Markup);
    }

    [Fact]
    public void RendersEmptyStateWhenItemsIsNull()
    {
        // Arrange & Act
        var cut = Render<SbDataGrid<TestItem>>(p => p
            .Add(x => x.Items, (IEnumerable<TestItem>?)null)
            .Add(x => x.ShowPagination, false)
            .Add(x => x.ShowColumnFilters, false)
            .AddChildContent(ColumnsTemplate));

        // Assert
        Assert.Contains("No data available", cut.Markup);
    }

    [Fact]
    public void RendersCustomEmptyTemplate()
    {
        // Arrange & Act
        var cut = Render<SbDataGrid<TestItem>>(p => p
            .Add(x => x.Items, new List<TestItem>())
            .Add(x => x.ShowPagination, false)
            .Add(x => x.EmptyTemplate, ec => ec.AddMarkupContent(0, "<span class=\"custom-empty\">No items found</span>"))
            .AddChildContent(ColumnsTemplate));

        // Assert
        var empty = cut.Find(".custom-empty");
        Assert.NotNull(empty);
        Assert.Contains("No items found", empty.TextContent);
    }

    [Fact]
    public void ShowsLoadingState()
    {
        // Arrange & Act
        var cut = RenderDataGrid(configure: p => p.Add(x => x.Loading, true));

        // Assert
        var loading = cut.Find(".sb-datagrid__loading");
        Assert.NotNull(loading);
        Assert.Contains("Loading", cut.Markup);
    }

    [Fact]
    public void AppliesCustomClass()
    {
        // Arrange & Act
        var cut = RenderDataGrid(configure: p => p.Add(x => x.Class, "my-grid"));

        // Assert
        var grid = cut.Find(".sb-datagrid");
        Assert.Contains("my-grid", grid.ClassList);
    }

    [Fact]
    public void AppliesStripedClass()
    {
        // Arrange & Act
        var cut = RenderDataGrid(configure: p => p.Add(x => x.Striped, true));

        // Assert
        var grid = cut.Find(".sb-datagrid");
        Assert.Contains("sb-datagrid--striped", grid.ClassList);
    }

    [Fact]
    public void AppliesBorderedClass()
    {
        // Arrange & Act
        var cut = RenderDataGrid();

        // Assert
        var grid = cut.Find(".sb-datagrid");
        Assert.Contains("sb-datagrid--bordered", grid.ClassList);
    }

    [Fact]
    public void AppliesBorderlessWhenBorderedFalse()
    {
        // Arrange & Act
        var cut = RenderDataGrid(configure: p => p.Add(x => x.Bordered, false));

        // Assert
        var grid = cut.Find(".sb-datagrid");
        Assert.Contains("sb-datagrid--borderless", grid.ClassList);
    }

    [Fact]
    public void AppliesDensityCompact()
    {
        // Arrange & Act
        var cut = RenderDataGrid(configure: p => p.Add(x => x.Density, SbDataGridDensity.Compact));

        // Assert
        var grid = cut.Find(".sb-datagrid");
        Assert.Contains("sb-datagrid--density-compact", grid.ClassList);
    }

    [Fact]
    public void AppliesDensityComfortable()
    {
        // Arrange & Act
        var cut = RenderDataGrid(configure: p => p.Add(x => x.Density, SbDataGridDensity.Comfortable));

        // Assert
        var grid = cut.Find(".sb-datagrid");
        Assert.Contains("sb-datagrid--density-comfortable", grid.ClassList);
    }

    [Fact]
    public void RendersSelectionColumnWhenSelectionModeMultiple()
    {
        // Arrange & Act
        var cut = RenderDataGrid(configure: p => p.Add(x => x.SelectionMode, SbSelectionMode.MultipleRows));

        // Assert
        var header = cut.Find(".sb-datagrid__cell--selection");
        Assert.NotNull(header);
        Assert.NotNull(cut.Find("input[type=\"checkbox\"]"));
    }

    [Fact]
    public void RendersSelectionColumnWhenSelectionModeSingle()
    {
        // Arrange & Act
        var cut = RenderDataGrid(configure: p => p.Add(x => x.SelectionMode, SbSelectionMode.SingleRow));

        // Assert
        Assert.NotNull(cut.Find(".sb-datagrid__cell--selection"));
    }

    [Fact]
    public void DoesNotRenderSelectionColumnWhenSelectionModeNone()
    {
        // Arrange & Act
        var cut = RenderDataGrid();

        // Assert
        Assert.Empty(cut.FindAll(".sb-datagrid__cell--selection"));
    }

    [Fact]
    public void RendersPaginationWhenEnabledAndHasData()
    {
        // Arrange & Act - use separate render to avoid duplicate ShowPagination (base sets false)
        var items = CreateTestItems(5);
        var cut = Render<SbDataGrid<TestItem>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<TestItem, string>)(item => item.Id.ToString()))
            .Add(x => x.ShowPagination, true)
            .Add(x => x.TotalCount, 10)
            .Add(x => x.ShowColumnFilters, false)
            .AddChildContent(ColumnsTemplate));

        // Assert
        Assert.NotNull(cut.Find(".sb-datagrid__pagination"));
    }

    [Fact]
    public void HasAriaLabelWhenSet()
    {
        // Arrange & Act
        var cut = RenderDataGrid(configure: p => p.Add(x => x.AriaLabel, "Products grid"));

        // Assert
        var grid = cut.Find(".sb-datagrid");
        Assert.Equal("Products grid", grid.GetAttribute("aria-label"));
    }

    [Fact]
    public void HasRoleApplication()
    {
        // Arrange & Act
        var cut = RenderDataGrid();

        // Assert
        var grid = cut.Find(".sb-datagrid");
        Assert.Equal("application", grid.GetAttribute("role"));
    }

    #region Sorting

    [Fact]
    public void RendersSortableHeaderWhenColumnSortable()
    {
        // Arrange & Act
        var cut = RenderDataGrid(configure: p => p.AddChildContent(SortableColumnsTemplate));

        // Assert
        var sortableHeader = cut.Find(".sb-datagrid__cell--sortable");
        Assert.NotNull(sortableHeader);
    }

    [Fact]
    public async Task InvokesOnSortChangedWhenSortableHeaderClicked()
    {
        // Arrange
        SbSort? capturedSort = null;
        var items = CreateTestItems(3);

        var cut = Render<SbDataGrid<TestItem>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<TestItem, string>)(item => item.Id.ToString()))
            .Add(x => x.ShowPagination, false)
            .Add(x => x.ShowColumnFilters, false)
            .Add(x => x.OnSortChanged, EventCallback.Factory.Create<SbSort?>(this, s => capturedSort = s))
            .AddChildContent(SortableColumnsTemplate));

        // Act - click Name column header (first sortable column)
        var nameHeader = cut.Find(".sb-datagrid__cell--sortable");
        await cut.InvokeAsync(() => nameHeader.Click());

        // Assert
        Assert.NotNull(capturedSort);
        Assert.Equal("Name", capturedSort!.Field);
        Assert.Equal(SbSortDirection.Ascending, capturedSort.Direction);
    }

    [Fact]
    public async Task SortsDataAscendingWhenHeaderClicked()
    {
        // Arrange - use items that have different sort orders: Zeta, Alpha, Beta
        var items = new List<TestItem>
        {
            new() { Id = 1, Name = "Zeta", Value = 10 },
            new() { Id = 2, Name = "Alpha", Value = 20 },
            new() { Id = 3, Name = "Beta", Value = 30 }
        };

        var cut = Render<SbDataGrid<TestItem>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<TestItem, string>)(item => item.Id.ToString()))
            .Add(x => x.ShowPagination, false)
            .Add(x => x.ShowColumnFilters, false)
            .AddChildContent(SortableColumnsTemplate));

        // Wait for initial render
        cut.WaitForState(() => cut.Markup.Contains("Zeta"));

        // Act - click Name header to sort ascending
        var nameHeader = cut.Find(".sb-datagrid__cell--sortable");
        await cut.InvokeAsync(() => nameHeader.Click());

        cut.WaitForState(() =>
        {
            var rows = cut.FindAll(".sb-datagrid__row:not(.sb-datagrid__row--empty) .sb-datagrid__cell");
            return rows.Count >= 2;
        });

        // Assert - first data row should be Alpha (ascending: Alpha, Beta, Zeta)
        var firstDataRow = cut.FindAll("tbody .sb-datagrid__row").FirstOrDefault();
        Assert.NotNull(firstDataRow);
        Assert.Contains("Alpha", firstDataRow.TextContent);
    }

    [Fact]
    public async Task SortsDataDescendingWhenHeaderClickedTwice()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new() { Id = 1, Name = "Alpha", Value = 10 },
            new() { Id = 2, Name = "Beta", Value = 20 },
            new() { Id = 3, Name = "Zeta", Value = 30 }
        };

        var cut = Render<SbDataGrid<TestItem>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<TestItem, string>)(item => item.Id.ToString()))
            .Add(x => x.ShowPagination, false)
            .Add(x => x.ShowColumnFilters, false)
            .AddChildContent(SortableColumnsTemplate));

        cut.WaitForState(() => cut.Markup.Contains("Alpha"));

        var nameHeader = cut.Find(".sb-datagrid__cell--sortable");

        // Act - click once (ascending), then twice (descending)
        await cut.InvokeAsync(() => nameHeader.Click());
        await cut.InvokeAsync(() => nameHeader.Click());

        cut.WaitForState(() =>
        {
            var rows = cut.FindAll(".sb-datagrid__row:not(.sb-datagrid__row--empty)");
            return rows.Any(r => r.TextContent.Contains("Zeta"));
        });

        // Assert - first data row should be Zeta (descending: Zeta, Beta, Alpha)
        var firstDataRow = cut.FindAll("tbody .sb-datagrid__row").FirstOrDefault();
        Assert.NotNull(firstDataRow);
        Assert.Contains("Zeta", firstDataRow.TextContent);
    }

    #endregion

    #region Filtering

    [Fact]
    public async Task FiltersDataWhenSetFilterAsyncCalled()
    {
        // Arrange
        var items = CreateTestItems(5);

        var cut = Render<SbDataGrid<TestItem>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<TestItem, string>)(item => item.Id.ToString()))
            .Add(x => x.ShowPagination, false)
            .Add(x => x.ShowColumnFilters, false)
            .Add(x => x.ShowFilterBar, true)
            .AddChildContent(ColumnsTemplate));

        cut.WaitForState(() => cut.Markup.Contains("Item 1"));

        // Act - filter by Name equals "Item 2"
        await cut.InvokeAsync(async () =>
        {
            await cut.Instance.SetFilterAsync("Name", SbFilterOperator.Equals, "Item 2");
        });

        cut.WaitForState(() => cut.Markup.Contains("Item 2") && !cut.Markup.Contains("Item 1"));

        // Assert
        Assert.Contains("Item 2", cut.Markup);
        Assert.DoesNotContain("Item 1", cut.Markup);
        Assert.DoesNotContain("Item 3", cut.Markup);
    }

    [Fact]
    public async Task ShowsFilterBarWhenFilterActive()
    {
        // Arrange
        var items = CreateTestItems(3);

        var cut = Render<SbDataGrid<TestItem>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<TestItem, string>)(item => item.Id.ToString()))
            .Add(x => x.ShowPagination, false)
            .Add(x => x.ShowColumnFilters, false)
            .Add(x => x.ShowFilterBar, true)
            .AddChildContent(ColumnsTemplate));

        cut.WaitForState(() => cut.Markup.Contains("Item 1"));

        // Act
        await cut.InvokeAsync(async () =>
        {
            await cut.Instance.SetFilterAsync("Name", SbFilterOperator.Contains, "Item");
        });

        cut.WaitForState(() => cut.FindAll(".sb-datagrid__filter-bar").Count > 0);

        // Assert
        Assert.NotNull(cut.Find(".sb-datagrid__filter-bar"));
        Assert.Contains("Filters:", cut.Markup);
    }

    [Fact]
    public async Task ClearsFiltersWhenClearAllFiltersAsyncCalled()
    {
        // Arrange
        var items = CreateTestItems(3);

        var cut = Render<SbDataGrid<TestItem>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<TestItem, string>)(item => item.Id.ToString()))
            .Add(x => x.ShowPagination, false)
            .Add(x => x.ShowColumnFilters, false)
            .Add(x => x.ShowFilterBar, true)
            .AddChildContent(ColumnsTemplate));

        cut.WaitForState(() => cut.Markup.Contains("Item 1"));

        await cut.InvokeAsync(async () =>
        {
            await cut.Instance.SetFilterAsync("Name", SbFilterOperator.Equals, "Item 2");
        });

        cut.WaitForState(() => !cut.Markup.Contains("Item 1"));

        // Act
        await cut.InvokeAsync(async () =>
        {
            await cut.Instance.ClearAllFiltersAsync();
        });

        cut.WaitForState(() => cut.Markup.Contains("Item 1"));

        // Assert
        Assert.Contains("Item 1", cut.Markup);
        Assert.Contains("Item 2", cut.Markup);
        Assert.Contains("Item 3", cut.Markup);
    }

    [Fact]
    public async Task InvokesOnFiltersChangedWhenFilterSet()
    {
        // Arrange
        IReadOnlyList<SbFilter>? capturedFilters = null;
        var items = CreateTestItems(3);

        var cut = Render<SbDataGrid<TestItem>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<TestItem, string>)(item => item.Id.ToString()))
            .Add(x => x.ShowPagination, false)
            .Add(x => x.ShowColumnFilters, false)
            .Add(x => x.OnFiltersChanged, EventCallback.Factory.Create<IReadOnlyList<SbFilter>>(this, f => capturedFilters = f))
            .AddChildContent(ColumnsTemplate));

        cut.WaitForState(() => cut.Markup.Contains("Item 1"));

        // Act
        await cut.InvokeAsync(async () =>
        {
            await cut.Instance.SetFilterAsync("Name", SbFilterOperator.Equals, "Item 2");
        });

        cut.WaitForState(() => capturedFilters != null);

        // Assert
        Assert.NotNull(capturedFilters);
        Assert.Single(capturedFilters);
        Assert.Equal("Name", capturedFilters![0].Field);
        Assert.Equal(SbFilterOperator.Equals, capturedFilters[0].Operator);
        Assert.Equal("Item 2", capturedFilters[0].Value);
    }

    [Fact]
    public async Task RemovesFilterWhenRemoveFilterAsyncCalled()
    {
        // Arrange
        var items = CreateTestItems(3);

        var cut = Render<SbDataGrid<TestItem>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<TestItem, string>)(item => item.Id.ToString()))
            .Add(x => x.ShowPagination, false)
            .Add(x => x.ShowColumnFilters, false)
            .AddChildContent(ColumnsTemplate));

        cut.WaitForState(() => cut.Markup.Contains("Item 1"));

        await cut.InvokeAsync(async () =>
        {
            await cut.Instance.SetFilterAsync("Name", SbFilterOperator.Equals, "Item 2");
        });

        cut.WaitForState(() => !cut.Markup.Contains("Item 1"));

        // Act
        await cut.InvokeAsync(async () =>
        {
            await cut.Instance.RemoveFilterAsync("Name");
        });

        cut.WaitForState(() => cut.Markup.Contains("Item 1"));

        // Assert - all items visible again
        Assert.Contains("Item 1", cut.Markup);
        Assert.Contains("Item 2", cut.Markup);
        Assert.Contains("Item 3", cut.Markup);
    }

    [Fact]
    public async Task FiltersWithSetOnlyFilterAsyncReplacesAllFilters()
    {
        // Arrange - set one filter, then use SetOnlyFilterAsync to replace with another
        var items = CreateTestItems(5);

        var cut = Render<SbDataGrid<TestItem>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<TestItem, string>)(item => item.Id.ToString()))
            .Add(x => x.ShowPagination, false)
            .Add(x => x.ShowColumnFilters, false)
            .AddChildContent(ColumnsTemplate));

        cut.WaitForState(() => cut.Markup.Contains("Item 1"));

        await cut.InvokeAsync(async () =>
        {
            await cut.Instance.SetFilterAsync("Name", SbFilterOperator.Equals, "Item 1");
        });

        cut.WaitForState(() => cut.Markup.Contains("Item 1") && !cut.Markup.Contains("Item 2"));

        // Act - SetOnlyFilterAsync should clear Item 1 filter and set Value filter
        await cut.InvokeAsync(async () =>
        {
            await cut.Instance.SetOnlyFilterAsync("Value", SbFilterOperator.Equals, 30);
        });

        cut.WaitForState(() => cut.Markup.Contains("Item 3") && !cut.Markup.Contains("Item 1"));

        // Assert - only Item 3 has Value=30
        Assert.Contains("Item 3", cut.Markup);
        Assert.Contains("30", cut.Markup);
        Assert.DoesNotContain("Item 1", cut.Markup);
        Assert.DoesNotContain("Item 2", cut.Markup);
    }

    #endregion

    #region Selection

    [Fact]
    public void AppliesSelectedRowClassWhenRowSelected()
    {
        // Arrange
        var items = CreateTestItems(2);
        var selectedKeys = new HashSet<string> { "2" };

        var cut = RenderDataGrid(items, p => p
            .Add(x => x.SelectionMode, SbSelectionMode.SingleRow)
            .Add(x => x.SelectedKeys, selectedKeys));

        // Assert - row 2 should have selected class
        cut.WaitForState(() => cut.FindAll(".sb-datagrid__row--selected").Count > 0);
        var selectedRow = cut.Find(".sb-datagrid__row--selected");
        Assert.NotNull(selectedRow);
        Assert.Contains("Item 2", selectedRow.TextContent);
    }

    [Fact]
    public async Task InvokesSelectedKeysChangedWhenRowClickedInSingleMode()
    {
        // Arrange
        IReadOnlySet<string>? capturedKeys = null;
        var items = CreateTestItems(3);

        var cut = Render<SbDataGrid<TestItem>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<TestItem, string>)(item => item.Id.ToString()))
            .Add(x => x.ShowPagination, false)
            .Add(x => x.ShowColumnFilters, false)
            .Add(x => x.SelectionMode, SbSelectionMode.SingleRow)
            .Add(x => x.SelectedKeysChanged, EventCallback.Factory.Create<IReadOnlySet<string>>(this, k => capturedKeys = k))
            .AddChildContent(ColumnsTemplate));

        cut.WaitForState(() => cut.Markup.Contains("Item 2"));

        // Act - click row 2 (second data row)
        var rows = cut.FindAll("tbody .sb-datagrid__row");
        var row2 = rows.FirstOrDefault(r => r.TextContent.Contains("Item 2"));
        Assert.NotNull(row2);
        await cut.InvokeAsync(() => row2!.Click());

        cut.WaitForState(() => capturedKeys != null);

        // Assert
        Assert.NotNull(capturedKeys);
        Assert.Single(capturedKeys);
        Assert.Contains("2", capturedKeys!);
    }

    [Fact]
    public async Task InvokesSelectedKeysChangedWhenCheckboxClickedInMultipleMode()
    {
        // Arrange
        IReadOnlySet<string>? capturedKeys = null;
        var items = CreateTestItems(2);

        var cut = Render<SbDataGrid<TestItem>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<TestItem, string>)(item => item.Id.ToString()))
            .Add(x => x.ShowPagination, false)
            .Add(x => x.ShowColumnFilters, false)
            .Add(x => x.SelectionMode, SbSelectionMode.MultipleRows)
            .Add(x => x.SelectedKeysChanged, EventCallback.Factory.Create<IReadOnlySet<string>>(this, k => capturedKeys = k))
            .AddChildContent(ColumnsTemplate));

        cut.WaitForState(() => cut.FindAll("input[type=\"checkbox\"]").Count > 0);

        // Act - change first row checkbox (SbCheckbox uses @onchange, not @onclick)
        var checkboxes = cut.FindAll("tbody input[type=\"checkbox\"]");
        var firstRowCheckbox = checkboxes.FirstOrDefault();
        Assert.NotNull(firstRowCheckbox);
        await cut.InvokeAsync(() => firstRowCheckbox!.Change(true));

        cut.WaitForState(() => capturedKeys != null && capturedKeys.Count > 0);

        // Assert
        Assert.NotNull(capturedKeys);
        Assert.Single(capturedKeys);
    }

    [Fact]
    public async Task SelectsAllRowsWhenHeaderCheckboxClickedInMultipleMode()
    {
        // Arrange
        IReadOnlySet<string>? capturedKeys = null;
        var items = CreateTestItems(3);

        var cut = Render<SbDataGrid<TestItem>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<TestItem, string>)(item => item.Id.ToString()))
            .Add(x => x.ShowPagination, false)
            .Add(x => x.ShowColumnFilters, false)
            .Add(x => x.SelectionMode, SbSelectionMode.MultipleRows)
            .Add(x => x.SelectedKeysChanged, EventCallback.Factory.Create<IReadOnlySet<string>>(this, k => capturedKeys = k))
            .AddChildContent(ColumnsTemplate));

        cut.WaitForState(() => cut.Find("thead input[type=\"checkbox\"]") != null);

        // Act - change header "select all" checkbox (SbCheckbox uses @onchange, not @onclick)
        var selectAllCheckbox = cut.Find("thead input[type=\"checkbox\"]");
        await cut.InvokeAsync(() => selectAllCheckbox!.Change(true));

        cut.WaitForState(() => capturedKeys != null && capturedKeys.Count == 3);

        // Assert
        Assert.NotNull(capturedKeys);
        Assert.Equal(3, capturedKeys!.Count);
        Assert.Contains("1", capturedKeys);
        Assert.Contains("2", capturedKeys);
        Assert.Contains("3", capturedKeys);
    }

    [Fact]
    public async Task InvokesOnRowClickedWhenRowClicked()
    {
        // Arrange
        TestItem? capturedItem = null;
        var items = CreateTestItems(3);

        var cut = Render<SbDataGrid<TestItem>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<TestItem, string>)(item => item.Id.ToString()))
            .Add(x => x.ShowPagination, false)
            .Add(x => x.ShowColumnFilters, false)
            .Add(x => x.OnRowClicked, EventCallback.Factory.Create<TestItem>(this, item => capturedItem = item))
            .AddChildContent(ColumnsTemplate));

        cut.WaitForState(() => cut.Markup.Contains("Item 2"));

        // Act
        var rows = cut.FindAll("tbody .sb-datagrid__row");
        var row2 = rows.FirstOrDefault(r => r.TextContent.Contains("Item 2"));
        await cut.InvokeAsync(() => row2!.Click());

        cut.WaitForState(() => capturedItem != null);

        // Assert
        Assert.NotNull(capturedItem);
        Assert.Equal(2, capturedItem!.Id);
        Assert.Equal("Item 2", capturedItem.Name);
    }

    [Fact]
    public async Task DeselectsRowWhenCheckboxUncheckedInMultipleMode()
    {
        // Arrange - start with row 1 selected
        IReadOnlySet<string>? capturedKeys = null;
        var items = CreateTestItems(2);
        var selectedKeys = new HashSet<string> { "1" };

        var cut = Render<SbDataGrid<TestItem>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<TestItem, string>)(item => item.Id.ToString()))
            .Add(x => x.ShowPagination, false)
            .Add(x => x.ShowColumnFilters, false)
            .Add(x => x.SelectionMode, SbSelectionMode.MultipleRows)
            .Add(x => x.SelectedKeys, selectedKeys)
            .Add(x => x.SelectedKeysChanged, EventCallback.Factory.Create<IReadOnlySet<string>>(this, k => capturedKeys = k))
            .AddChildContent(ColumnsTemplate));

        cut.WaitForState(() => cut.FindAll("tbody input[type=\"checkbox\"]").Count > 0);

        // Act - uncheck first row (Item 1) by triggering change to false
        var checkboxes = cut.FindAll("tbody input[type=\"checkbox\"]");
        var firstRowCheckbox = checkboxes.FirstOrDefault();
        Assert.NotNull(firstRowCheckbox);
        await cut.InvokeAsync(() => firstRowCheckbox!.Change(false));

        cut.WaitForState(() => capturedKeys != null && capturedKeys.Count == 0);

        // Assert
        Assert.NotNull(capturedKeys);
        Assert.Empty(capturedKeys!);
    }

    [Fact]
    public async Task ActiveFiltersReflectsSetFilterAsync()
    {
        // Arrange
        var items = CreateTestItems(3);

        var cut = Render<SbDataGrid<TestItem>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<TestItem, string>)(item => item.Id.ToString()))
            .Add(x => x.ShowPagination, false)
            .Add(x => x.ShowColumnFilters, false)
            .AddChildContent(ColumnsTemplate));

        // Act
        await cut.InvokeAsync(async () =>
        {
            await cut.Instance.SetFilterAsync("Name", SbFilterOperator.Contains, "Item");
        });

        cut.WaitForState(() => cut.Instance.ActiveFilters.Count == 1);

        // Assert
        var filters = cut.Instance.ActiveFilters;
        Assert.Single(filters);
        Assert.Equal("Name", filters[0].Field);
        Assert.Equal(SbFilterOperator.Contains, filters[0].Operator);
        Assert.Equal("Item", filters[0].Value);
    }

    #endregion

    private class TestItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}
