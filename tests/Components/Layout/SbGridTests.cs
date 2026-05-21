using Microsoft.AspNetCore.Components;
using Bunit;
using SufiChain.SufiBlazor.Components.Layout;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Layout;

public class SbGridTests : BunitContext
{
    private IRenderedComponent<SbGrid> RenderGrid(
        Action<ComponentParameterCollectionBuilder<SbGrid>>? configure = null)
    {
        return Render<SbGrid>(p => configure?.Invoke(p));
    }

    private IRenderedComponent<SbGridItem> RenderGridItem(
        Action<ComponentParameterCollectionBuilder<SbGridItem>>? configure = null)
    {
        return Render<SbGridItem>(p => configure?.Invoke(p));
    }

    // --- SbGrid tests ---

    [Fact]
    public void RendersGridStructure()
    {
        // Arrange & Act
        var cut = RenderGrid();

        // Assert
        var div = cut.Find(".sb-grid");
        Assert.NotNull(div);
        Assert.Equal("div", div.TagName.ToLowerInvariant());
    }

    [Fact]
    public void RendersChildContent()
    {
        // Arrange & Act
        var cut = RenderGrid(p => p.AddChildContent("<span>Grid content</span>"));

        // Assert
        Assert.Contains("Grid content", cut.Markup);
        Assert.NotNull(cut.Find("span"));
    }

    [Fact]
    public void SetsDefaultColumnsInStyle()
    {
        // Arrange & Act
        var cut = RenderGrid();

        // Assert - default Columns is 12
        var div = cut.Find(".sb-grid");
        var style = div.GetAttribute("style");
        Assert.Contains("--sb-grid-columns: 12", style);
    }

    [Fact]
    public void AppliesCustomColumns()
    {
        // Arrange & Act
        var cut = RenderGrid(p => p.Add(x => x.Columns, 6));

        // Assert
        var div = cut.Find(".sb-grid");
        var style = div.GetAttribute("style");
        Assert.Contains("--sb-grid-columns: 6", style);
    }

    [Fact]
    public void AppliesResponsiveColumnClasses()
    {
        // Arrange & Act
        var cut = RenderGrid(p => p
            .Add(x => x.ColumnsSm, 6)
            .Add(x => x.ColumnsMd, 8)
            .Add(x => x.ColumnsLg, 10)
            .Add(x => x.ColumnsXl, 12));

        // Assert
        var div = cut.Find(".sb-grid");
        Assert.Contains("sb-grid--sm-6", div.ClassList);
        Assert.Contains("sb-grid--md-8", div.ClassList);
        Assert.Contains("sb-grid--lg-10", div.ClassList);
        Assert.Contains("sb-grid--xl-12", div.ClassList);
    }

    [Fact]
    public void DoesNotAddResponsiveClassWhenNull()
    {
        // Arrange & Act
        var cut = RenderGrid(p => p.Add(x => x.ColumnsSm, 6));

        // Assert - only sm, not md/lg/xl
        var div = cut.Find(".sb-grid");
        Assert.Contains("sb-grid--sm-6", div.ClassList);
        Assert.DoesNotContain("sb-grid--md-", div.ClassList);
    }

    [Fact]
    public void SetsDefaultGapInStyle()
    {
        // Arrange & Act
        var cut = RenderGrid();

        // Assert - default Gap is 4
        var div = cut.Find(".sb-grid");
        var style = div.GetAttribute("style");
        Assert.Contains("gap: var(--sb-space-4)", style);
    }

    [Fact]
    public void AppliesCustomGap()
    {
        // Arrange & Act
        var cut = RenderGrid(p => p.Add(x => x.Gap, 8));

        // Assert
        var div = cut.Find(".sb-grid");
        var style = div.GetAttribute("style");
        Assert.Contains("gap: var(--sb-space-8)", style);
    }

    [Fact]
    public void UsesRowGapAndColumnGapWhenProvided()
    {
        // Arrange & Act
        var cut = RenderGrid(p => p
            .Add(x => x.RowGap, 2)
            .Add(x => x.ColumnGap, 6));

        // Assert
        var div = cut.Find(".sb-grid");
        var style = div.GetAttribute("style");
        Assert.Contains("row-gap: var(--sb-space-2)", style);
        Assert.Contains("column-gap: var(--sb-space-6)", style);
    }

    [Fact]
    public void UsesGapAsFallbackWhenOnlyRowGapSet()
    {
        // Arrange & Act
        var cut = RenderGrid(p => p.Add(x => x.RowGap, 2));

        // Assert - column-gap falls back to Gap (4)
        var div = cut.Find(".sb-grid");
        var style = div.GetAttribute("style");
        Assert.Contains("row-gap: var(--sb-space-2)", style);
        Assert.Contains("column-gap: var(--sb-space-4)", style);
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderGrid(p => p.Add(x => x.Class, "my-grid"));

        // Assert
        var div = cut.Find(".sb-grid");
        Assert.Contains("my-grid", div.ClassList);
    }

    [Fact]
    public void AppliesStyleParameter()
    {
        // Arrange & Act
        var cut = RenderGrid(p => p.Add(x => x.Style, "min-height: 100px;"));

        // Assert
        var div = cut.Find(".sb-grid");
        var style = div.GetAttribute("style");
        Assert.Contains("min-height: 100px", style);
    }

    [Fact]
    public void AppliesAdditionalAttributes()
    {
        // Arrange & Act
        var cut = RenderGrid(p => p.Add(x => x.AdditionalAttributes, new Dictionary<string, object>
        {
            { "data-testid", "layout-grid" },
            { "role", "presentation" }
        }));

        // Assert
        var div = cut.Find(".sb-grid");
        Assert.Equal("layout-grid", div.GetAttribute("data-testid"));
        Assert.Equal("presentation", div.GetAttribute("role"));
    }

    [Fact]
    public void RendersGridItemChildren()
    {
        // Arrange & Act
        var cut = Render<SbGrid>(p => p.AddChildContent(b =>
        {
            b.OpenComponent<SbGridItem>(0);
            b.AddAttribute(1, "Span", 4);
            b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "Item 1")));
            b.CloseComponent();
            b.OpenComponent<SbGridItem>(3);
            b.AddAttribute(4, "Span", 8);
            b.AddAttribute(5, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "Item 2")));
            b.CloseComponent();
        }));

        // Assert
        var grid = cut.Find(".sb-grid");
        Assert.NotNull(grid);
        var items = cut.FindAll(".sb-grid-item");
        Assert.Equal(2, items.Count);
        Assert.Contains("Item 1", cut.Markup);
        Assert.Contains("Item 2", cut.Markup);
    }

    // --- SbGridItem tests ---

    [Fact]
    public void GridItem_RendersStructure()
    {
        // Arrange & Act
        var cut = RenderGridItem();

        // Assert
        var div = cut.Find(".sb-grid-item");
        Assert.NotNull(div);
        Assert.Equal("div", div.TagName.ToLowerInvariant());
    }

    [Fact]
    public void GridItem_RendersChildContent()
    {
        // Arrange & Act
        var cut = RenderGridItem(p => p.AddChildContent("<p>Item content</p>"));

        // Assert
        Assert.Contains("Item content", cut.Markup);
        Assert.NotNull(cut.Find("p"));
    }

    [Fact]
    public void GridItem_SetsDefaultSpanInStyle()
    {
        // Arrange & Act
        var cut = RenderGridItem();

        // Assert - default Span is 1
        var div = cut.Find(".sb-grid-item");
        var style = div.GetAttribute("style");
        Assert.Contains("--sb-grid-span: 1", style);
    }

    [Fact]
    public void GridItem_AppliesSpanInStyle()
    {
        // Arrange & Act
        var cut = RenderGridItem(p => p.Add(x => x.Span, 6));

        // Assert
        var div = cut.Find(".sb-grid-item");
        var style = div.GetAttribute("style");
        Assert.Contains("--sb-grid-span: 6", style);
    }

    [Fact]
    public void GridItem_AppliesResponsiveSpanClasses()
    {
        // Arrange & Act
        var cut = RenderGridItem(p => p
            .Add(x => x.SpanSm, 2)
            .Add(x => x.SpanMd, 4)
            .Add(x => x.SpanLg, 6)
            .Add(x => x.SpanXl, 8));

        // Assert
        var div = cut.Find(".sb-grid-item");
        Assert.Contains("sb-grid-item--sm-2", div.ClassList);
        Assert.Contains("sb-grid-item--md-4", div.ClassList);
        Assert.Contains("sb-grid-item--lg-6", div.ClassList);
        Assert.Contains("sb-grid-item--xl-8", div.ClassList);
    }

    [Fact]
    public void GridItem_DoesNotAddResponsiveClassWhenNull()
    {
        // Arrange & Act
        var cut = RenderGridItem(p => p.Add(x => x.SpanMd, 6));

        // Assert
        var div = cut.Find(".sb-grid-item");
        Assert.Contains("sb-grid-item--md-6", div.ClassList);
        Assert.DoesNotContain("sb-grid-item--sm-", div.ClassList);
        Assert.DoesNotContain("sb-grid-item--lg-", div.ClassList);
        Assert.DoesNotContain("sb-grid-item--xl-", div.ClassList);
    }

    [Fact]
    public void GridItem_AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderGridItem(p => p.Add(x => x.Class, "sidebar"));

        // Assert
        var div = cut.Find(".sb-grid-item");
        Assert.Contains("sidebar", div.ClassList);
    }

    [Fact]
    public void GridItem_AppliesStyleParameter()
    {
        // Arrange & Act
        var cut = RenderGridItem(p => p.Add(x => x.Style, "background: white;"));

        // Assert
        var div = cut.Find(".sb-grid-item");
        var style = div.GetAttribute("style");
        Assert.Contains("background: white", style);
    }

    [Fact]
    public void GridItem_AppliesAdditionalAttributes()
    {
        // Arrange & Act
        var cut = RenderGridItem(p => p.Add(x => x.AdditionalAttributes, new Dictionary<string, object>
        {
            { "data-testid", "grid-cell" },
            { "aria-label", "Main content" }
        }));

        // Assert
        var div = cut.Find(".sb-grid-item");
        Assert.Equal("grid-cell", div.GetAttribute("data-testid"));
        Assert.Equal("Main content", div.GetAttribute("aria-label"));
    }

    [Fact]
    public void GridItem_WorksInsideSbGrid()
    {
        // Arrange & Act
        var cut = Render<SbGrid>(p => p.AddChildContent(b =>
        {
            b.OpenComponent<SbGridItem>(0);
            b.AddAttribute(1, "Span", 12);
            b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "Full width")));
            b.CloseComponent();
        }));

        // Assert
        var grid = cut.Find(".sb-grid");
        var item = cut.Find(".sb-grid-item");
        Assert.NotNull(grid);
        Assert.NotNull(item);
        Assert.Contains("Full width", cut.Markup);
        Assert.Contains("--sb-grid-span: 12", item.GetAttribute("style"));
    }
}
