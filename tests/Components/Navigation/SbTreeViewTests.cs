using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Bunit;
using SufiChain.SufiBlazor.Components.Navigation;
using SufiChain.SufiBlazor.Localization;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Navigation;

file class StubStringLocalizer : IStringLocalizer<SufiBlazorResource>
{
    public LocalizedString this[string name] => new(name, name);
    public LocalizedString this[string name, params object[] arguments] => new(name, string.Format(CultureInfo.InvariantCulture, name, arguments));
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => Array.Empty<LocalizedString>();
}

public class SbTreeViewTests : BunitContext
{
    private class TreeTestItem
    {
        public string Name { get; set; } = "";
        public string? Icon { get; set; }
        public List<TreeTestItem> Children { get; set; } = new();
    }

    public SbTreeViewTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new StubStringLocalizer());
    }

    private IRenderedComponent<SbTreeView<TreeTestItem>> RenderTreeView(
        Action<ComponentParameterCollectionBuilder<SbTreeView<TreeTestItem>>>? configure = null)
    {
        return Render<SbTreeView<TreeTestItem>>(p => configure?.Invoke(p));
    }

    private static List<TreeTestItem> CreateSimpleTree() => new()
    {
        new TreeTestItem { Name = "Root 1" },
        new TreeTestItem
        {
            Name = "Root 2",
            Children = new()
            {
                new TreeTestItem { Name = "Child 2.1" },
                new TreeTestItem { Name = "Child 2.2" }
            }
        }
    };

    // --- SbTreeView tests ---

    [Fact]
    public void RendersTreeViewStructure()
    {
        // Arrange & Act
        var items = new List<TreeTestItem> { new() { Name = "Item 1" } };
        var cut = RenderTreeView(p => p
            .Add(x => x.Items, items)
            .Add(x => x.TextSelector, i => i.Name)
            .Add(x => x.ChildSelector, i => i.Children));

        // Assert
        var tree = cut.Find(".sb-treeview");
        Assert.NotNull(tree);
        Assert.Equal("tree", tree.GetAttribute("role"));
    }

    [Fact]
    public void RendersEmptyWhenNoItems()
    {
        // Arrange & Act
        var cut = RenderTreeView(p => p
            .Add(x => x.Items, new List<TreeTestItem>())
            .Add(x => x.TextSelector, i => i.Name)
            .Add(x => x.ChildSelector, i => i.Children));

        // Assert
        var tree = cut.Find(".sb-treeview");
        Assert.NotNull(tree);
        Assert.Empty(cut.FindAll(".sb-treeview-node"));
    }

    [Fact]
    public void RendersRootNodes()
    {
        // Arrange & Act
        var items = CreateSimpleTree();
        var cut = RenderTreeView(p => p
            .Add(x => x.Items, items)
            .Add(x => x.TextSelector, i => i.Name)
            .Add(x => x.ChildSelector, i => i.Children));

        // Assert
        Assert.Contains("Root 1", cut.Markup);
        Assert.Contains("Root 2", cut.Markup);
        var nodes = cut.FindAll(".sb-treeview-node");
        Assert.Equal(2, nodes.Count);
    }

    [Fact]
    public void RendersChildrenWhenExpanded()
    {
        // Arrange - Root 2 has children, expand via IsExpandedSelector
        var items = CreateSimpleTree();
        var cut = RenderTreeView(p => p
            .Add(x => x.Items, items)
            .Add(x => x.TextSelector, i => i.Name)
            .Add(x => x.ChildSelector, i => i.Children)
            .Add(x => x.IsExpandedSelector, i => i.Name == "Root 2"));

        // Assert
        Assert.Contains("Child 2.1", cut.Markup);
        Assert.Contains("Child 2.2", cut.Markup);
    }

    [Fact]
    public void DoesNotRenderChildrenWhenCollapsed()
    {
        // Arrange - all collapsed (default)
        var items = CreateSimpleTree();
        var cut = RenderTreeView(p => p
            .Add(x => x.Items, items)
            .Add(x => x.TextSelector, i => i.Name)
            .Add(x => x.ChildSelector, i => i.Children));

        // Assert - children not visible when collapsed
        Assert.DoesNotContain("Child 2.1", cut.Markup);
        Assert.DoesNotContain("Child 2.2", cut.Markup);
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var items = new List<TreeTestItem> { new() { Name = "Item" } };
        var cut = RenderTreeView(p => p
            .Add(x => x.Items, items)
            .Add(x => x.TextSelector, i => i.Name)
            .Add(x => x.ChildSelector, i => i.Children)
            .Add(x => x.Class, "my-tree"));

        // Assert
        var tree = cut.Find(".sb-treeview");
        Assert.Contains("my-tree", tree.ClassList);
    }

    [Fact]
    public void RendersIconWhenIconSelectorProvided()
    {
        // Arrange & Act
        var items = new List<TreeTestItem> { new() { Name = "Folder", Icon = "folder" } };
        var cut = RenderTreeView(p => p
            .Add(x => x.Items, items)
            .Add(x => x.TextSelector, i => i.Name)
            .Add(x => x.ChildSelector, i => i.Children)
            .Add(x => x.IconSelector, i => i.Icon));

        // Assert
        var icon = cut.Find(".sb-treeview-node__icon");
        Assert.NotNull(icon);
    }

    [Fact]
    public void AppliesSelectedClassWhenSelectedItemMatches()
    {
        // Arrange
        var items = CreateSimpleTree();
        var selected = items[0];
        var cut = RenderTreeView(p => p
            .Add(x => x.Items, items)
            .Add(x => x.TextSelector, i => i.Name)
            .Add(x => x.ChildSelector, i => i.Children)
            .Add(x => x.SelectedItem, selected));

        // Assert
        var selectedContent = cut.Find(".sb-treeview-node__content--selected");
        Assert.NotNull(selectedContent);
        Assert.Contains("Root 1", selectedContent.TextContent);
    }

    [Fact]
    public async Task ClickingNodeInvokesSelectedItemChanged()
    {
        // Arrange
        TreeTestItem? received = null;
        var items = CreateSimpleTree();
        var cut = RenderTreeView(p => p
            .Add(x => x.Items, items)
            .Add(x => x.TextSelector, i => i.Name)
            .Add(x => x.ChildSelector, i => i.Children)
            .Add(x => x.SelectedItemChanged, EventCallback.Factory.Create<TreeTestItem>(this, item => received = item)));

        // Act - click Root 1 content
        var content = cut.Find(".sb-treeview-node__content");
        await cut.InvokeAsync(() => content.Click());

        // Assert
        Assert.NotNull(received);
        Assert.Equal("Root 1", received.Name);
    }

    [Fact]
    public void ShowsCheckboxesWhenShowCheckboxesTrue()
    {
        // Arrange & Act
        var items = new List<TreeTestItem> { new() { Name = "Item" } };
        var cut = RenderTreeView(p => p
            .Add(x => x.Items, items)
            .Add(x => x.TextSelector, i => i.Name)
            .Add(x => x.ChildSelector, i => i.Children)
            .Add(x => x.ShowCheckboxes, true));

        // Assert
        var checkbox = cut.Find("input.sb-treeview-node__checkbox");
        Assert.NotNull(checkbox);
        Assert.Equal("checkbox", checkbox.GetAttribute("type"));
    }

    [Fact]
    public void DoesNotShowCheckboxesByDefault()
    {
        // Arrange & Act
        var items = new List<TreeTestItem> { new() { Name = "Item" } };
        var cut = RenderTreeView(p => p
            .Add(x => x.Items, items)
            .Add(x => x.TextSelector, i => i.Name)
            .Add(x => x.ChildSelector, i => i.Children));

        // Assert
        Assert.Empty(cut.FindAll("input.sb-treeview-node__checkbox"));
    }

    [Fact]
    public async Task ToggleButtonExpandsNode()
    {
        // Arrange
        var items = CreateSimpleTree();
        var cut = RenderTreeView(p => p
            .Add(x => x.Items, items)
            .Add(x => x.TextSelector, i => i.Name)
            .Add(x => x.ChildSelector, i => i.Children));

        // Initially children not visible
        Assert.DoesNotContain("Child 2.1", cut.Markup);

        // Act - click toggle on Root 2
        var toggle = cut.FindAll("button.sb-treeview-node__toggle").Last();
        await cut.InvokeAsync(() => toggle.Click());
        cut.Render();

        // Assert - children now visible
        Assert.Contains("Child 2.1", cut.Markup);
    }

    [Fact]
    public async Task ToggleInvokesOnToggleCallback()
    {
        // Arrange
        TreeTestItem? toggled = null;
        var items = CreateSimpleTree();
        var cut = RenderTreeView(p => p
            .Add(x => x.Items, items)
            .Add(x => x.TextSelector, i => i.Name)
            .Add(x => x.ChildSelector, i => i.Children)
            .Add(x => x.OnToggle, EventCallback.Factory.Create<TreeTestItem>(this, item => toggled = item)));

        // Act
        var toggle = cut.FindAll("button.sb-treeview-node__toggle").Last();
        await cut.InvokeAsync(() => toggle.Click());

        // Assert
        Assert.NotNull(toggled);
        Assert.Equal("Root 2", toggled.Name);
    }

    [Fact]
    public void NodeHasCorrectRoleAndLevel()
    {
        // Arrange & Act
        var items = new List<TreeTestItem> { new() { Name = "Item" } };
        var cut = RenderTreeView(p => p
            .Add(x => x.Items, items)
            .Add(x => x.TextSelector, i => i.Name)
            .Add(x => x.ChildSelector, i => i.Children));

        // Assert
        var node = cut.Find(".sb-treeview-node");
        Assert.Equal("treeitem", node.GetAttribute("role"));
        Assert.Equal("1", node.GetAttribute("aria-level"));
    }

    // --- SbTreeViewNode tests (via SbTreeView) ---

    [Fact]
    public void Node_LeafHasSpacerInsteadOfToggle()
    {
        // Arrange - Root 1 has no children
        var items = CreateSimpleTree();
        var cut = RenderTreeView(p => p
            .Add(x => x.Items, items)
            .Add(x => x.TextSelector, i => i.Name)
            .Add(x => x.ChildSelector, i => i.Children));

        // Assert - leaf has spacer
        var spacer = cut.Find(".sb-treeview-node__spacer");
        Assert.NotNull(spacer);
    }

    [Fact]
    public void Node_WithChildrenHasToggleButton()
    {
        // Arrange & Act
        var items = CreateSimpleTree();
        var cut = RenderTreeView(p => p
            .Add(x => x.Items, items)
            .Add(x => x.TextSelector, i => i.Name)
            .Add(x => x.ChildSelector, i => i.Children));

        // Assert
        var toggles = cut.FindAll("button.sb-treeview-node__toggle");
        Assert.Single(toggles); // Root 2 has children
    }

    [Fact]
    public void Node_ExpandedHasExpandedClass()
    {
        // Arrange
        var items = CreateSimpleTree();
        var cut = RenderTreeView(p => p
            .Add(x => x.Items, items)
            .Add(x => x.TextSelector, i => i.Name)
            .Add(x => x.ChildSelector, i => i.Children)
            .Add(x => x.IsExpandedSelector, i => i.Name == "Root 2"));

        // Assert
        var expandedNode = cut.Find(".sb-treeview-node--expanded");
        Assert.NotNull(expandedNode);
    }
}
