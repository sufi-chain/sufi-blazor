using Microsoft.AspNetCore.Components;
using Bunit;
using SufiChain.SufiBlazor.Components.Data;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Data;

public class SbTableTests : BunitContext
{
    private static List<TableTestItem> CreateTestItems(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => new TableTestItem { Id = i, Name = $"Item {i}", Value = i * 10 })
            .ToList();
    }

    private static RenderFragment<TableTestItem> RowTemplate => item => builder =>
    {
        builder.OpenElement(0, "td");
        builder.AddContent(1, item.Name);
        builder.CloseElement();
        builder.OpenElement(2, "td");
        builder.AddContent(3, item.Value.ToString());
        builder.CloseElement();
    };

    private IRenderedComponent<SbTable<TableTestItem>> RenderTable(
        List<TableTestItem>? items = null,
        Action<ComponentParameterCollectionBuilder<SbTable<TableTestItem>>>? configure = null)
    {
        items ??= CreateTestItems(3);
        return Render<SbTable<TableTestItem>>(p =>
        {
            p.Add(x => x.Items, items)
                .Add(x => x.RowTemplate, RowTemplate);
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersTableStructure()
    {
        // Arrange & Act
        var cut = RenderTable();

        // Assert
        var container = cut.Find(".sb-table-container");
        Assert.NotNull(container);
        var table = cut.Find("table.sb-table");
        Assert.NotNull(table);
        Assert.NotNull(cut.Find("tbody.sb-table__body"));
    }

    [Fact]
    public void RendersDataRows()
    {
        // Arrange
        var items = CreateTestItems(2);

        // Act
        var cut = RenderTable(items);

        // Assert
        Assert.Contains("Item 1", cut.Markup);
        Assert.Contains("Item 2", cut.Markup);
        Assert.Contains("10", cut.Markup);
        Assert.Contains("20", cut.Markup);
        var rows = cut.FindAll(".sb-table__row");
        Assert.Equal(2, rows.Count);
    }

    [Fact]
    public void RendersHeaderWhenHeaderContentProvided()
    {
        // Arrange & Act
        var cut = RenderTable(configure: p => p
            .Add(x => x.HeaderContent, h => h.AddMarkupContent(0, "<tr><th>Name</th><th>Value</th></tr>")));

        // Assert
        var thead = cut.Find("thead.sb-table__head");
        Assert.NotNull(thead);
        Assert.Contains("Name", cut.Markup);
        Assert.Contains("Value", cut.Markup);
    }

    [Fact]
    public void DoesNotRenderHeaderWhenHeaderContentNull()
    {
        // Arrange & Act
        var cut = RenderTable();

        // Assert
        Assert.Empty(cut.FindAll("thead"));
    }

    [Fact]
    public void RendersFooterWhenFooterContentProvided()
    {
        // Arrange & Act
        var cut = RenderTable(configure: p => p
            .Add(x => x.FooterContent, f => f.AddMarkupContent(0, "<tr><td colspan=\"2\">Total: 3 items</td></tr>")));

        // Assert
        var tfoot = cut.Find("tfoot.sb-table__foot");
        Assert.NotNull(tfoot);
        Assert.Contains("Total: 3 items", cut.Markup);
    }

    [Fact]
    public void DoesNotRenderFooterWhenFooterContentNull()
    {
        // Arrange & Act
        var cut = RenderTable();

        // Assert
        Assert.Empty(cut.FindAll("tfoot"));
    }

    [Fact]
    public void RendersEmptyContentWhenNoItems()
    {
        // Arrange & Act
        var cut = Render<SbTable<TableTestItem>>(p => p
            .Add(x => x.Items, new List<TableTestItem>())
            .Add(x => x.RowTemplate, RowTemplate)
            .Add(x => x.EmptyContent, e => e.AddMarkupContent(0, "<span class=\"custom-empty\">No data</span>")));

        // Assert
        var emptyCell = cut.Find(".sb-table__empty");
        Assert.NotNull(emptyCell);
        var customEmpty = cut.Find(".custom-empty");
        Assert.NotNull(customEmpty);
        Assert.Contains("No data", customEmpty.TextContent);
    }

    [Fact]
    public void RendersEmptyContentWhenItemsNull()
    {
        // Arrange & Act
        var cut = Render<SbTable<TableTestItem>>(p => p
            .Add(x => x.Items, (IEnumerable<TableTestItem>?)null)
            .Add(x => x.RowTemplate, RowTemplate)
            .Add(x => x.EmptyContent, e => e.AddMarkupContent(0, "Nothing here")));

        // Assert
        Assert.Contains("Nothing here", cut.Markup);
    }

    [Fact]
    public void RendersEmptyBodyWhenNoItemsAndNoEmptyContent()
    {
        // Arrange & Act
        var cut = Render<SbTable<TableTestItem>>(p => p
            .Add(x => x.Items, new List<TableTestItem>())
            .Add(x => x.RowTemplate, RowTemplate));

        // Assert - tbody exists but has no data rows
        var tbody = cut.Find("tbody.sb-table__body");
        Assert.NotNull(tbody);
        Assert.Empty(cut.FindAll(".sb-table__row"));
    }

    [Fact]
    public void AppliesStripedClass()
    {
        // Arrange & Act
        var cut = RenderTable(configure: p => p.Add(x => x.Striped, true));

        // Assert
        var table = cut.Find("table.sb-table");
        Assert.Contains("sb-table--striped", table.ClassList);
    }

    [Fact]
    public void AppliesHoverClass()
    {
        // Arrange & Act
        var cut = RenderTable(configure: p => p.Add(x => x.Hover, true));

        // Assert - Hover defaults to true
        var table = cut.Find("table.sb-table");
        Assert.Contains("sb-table--hover", table.ClassList);
    }

    [Fact]
    public void DoesNotApplyHoverClassWhenHoverFalse()
    {
        // Arrange & Act
        var cut = RenderTable(configure: p => p.Add(x => x.Hover, false));

        // Assert
        var table = cut.Find("table.sb-table");
        Assert.DoesNotContain("sb-table--hover", table.ClassList);
    }

    [Fact]
    public void AppliesBorderedClass()
    {
        // Arrange & Act
        var cut = RenderTable(configure: p => p.Add(x => x.Bordered, true));

        // Assert
        var table = cut.Find("table.sb-table");
        Assert.Contains("sb-table--bordered", table.ClassList);
    }

    [Fact]
    public void AppliesCompactClass()
    {
        // Arrange & Act
        var cut = RenderTable(configure: p => p.Add(x => x.Compact, true));

        // Assert
        var table = cut.Find("table.sb-table");
        Assert.Contains("sb-table--compact", table.ClassList);
    }

    [Fact]
    public void AppliesCustomClassToContainer()
    {
        // Arrange & Act
        var cut = RenderTable(configure: p => p.Add(x => x.Class, "my-table"));

        // Assert
        var container = cut.Find(".sb-table-container");
        Assert.Contains("my-table", container.ClassList);
    }

    [Fact]
    public async Task InvokesOnRowClickWhenRowClicked()
    {
        // Arrange
        TableTestItem? capturedItem = null;
        var items = CreateTestItems(3);

        var cut = Render<SbTable<TableTestItem>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.RowTemplate, RowTemplate)
            .Add(x => x.OnRowClick, EventCallback.Factory.Create<TableTestItem>(this, item => capturedItem = item)));

        // Act - click second row (Item 2)
        var rows = cut.FindAll(".sb-table__row");
        var row2 = rows.FirstOrDefault(r => r.TextContent.Contains("Item 2"));
        Assert.NotNull(row2);
        await cut.InvokeAsync(() => row2!.Click());

        cut.WaitForState(() => capturedItem != null);

        // Assert
        Assert.NotNull(capturedItem);
        Assert.Equal(2, capturedItem!.Id);
        Assert.Equal("Item 2", capturedItem.Name);
        Assert.Equal(20, capturedItem.Value);
    }

    [Fact]
    public void RendersAllStyleVariantsTogether()
    {
        // Arrange & Act
        var cut = RenderTable(configure: p => p
            .Add(x => x.Striped, true)
            .Add(x => x.Hover, true)
            .Add(x => x.Bordered, true)
            .Add(x => x.Compact, true));

        // Assert
        var table = cut.Find("table.sb-table");
        Assert.Contains("sb-table--striped", table.ClassList);
        Assert.Contains("sb-table--hover", table.ClassList);
        Assert.Contains("sb-table--bordered", table.ClassList);
        Assert.Contains("sb-table--compact", table.ClassList);
    }

    private class TableTestItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}
