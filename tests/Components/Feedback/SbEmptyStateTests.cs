using Microsoft.AspNetCore.Components;
using Bunit;
using SufiChain.SufiBlazor.Components.Feedback;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Feedback;

public class SbEmptyStateTests : BunitContext
{
    private IRenderedComponent<SbEmptyState> RenderEmptyState(
        Action<ComponentParameterCollectionBuilder<SbEmptyState>>? configure = null)
    {
        return Render<SbEmptyState>(p => configure?.Invoke(p));
    }

    [Fact]
    public void RendersEmptyStateStructure()
    {
        // Arrange & Act
        var cut = RenderEmptyState();

        // Assert
        var container = cut.Find(".sb-empty-state");
        Assert.NotNull(container);
    }

    [Fact]
    public void RendersDefaultTitle()
    {
        // Arrange & Act
        var cut = RenderEmptyState();

        // Assert
        var title = cut.Find(".sb-empty-state__title");
        Assert.NotNull(title);
        Assert.Contains("No items found", title.TextContent);
    }

    [Fact]
    public void RendersDefaultIconText()
    {
        // Arrange & Act
        var cut = RenderEmptyState();

        // Assert
        var iconText = cut.Find(".sb-empty-state__icon-text");
        Assert.NotNull(iconText);
        Assert.Contains("📭", iconText.TextContent);
    }

    [Fact]
    public void RendersCustomTitle()
    {
        // Arrange & Act
        var cut = RenderEmptyState(p => p.Add(x => x.Title, "No documents"));

        // Assert
        var title = cut.Find(".sb-empty-state__title");
        Assert.NotNull(title);
        Assert.Contains("No documents", title.TextContent);
    }

    [Fact]
    public void RendersDescriptionWhenProvided()
    {
        // Arrange & Act
        var cut = RenderEmptyState(p => p.Add(x => x.Description, "Create your first item to get started."));

        // Assert
        var description = cut.Find(".sb-empty-state__description");
        Assert.NotNull(description);
        Assert.Contains("Create your first item to get started.", description.TextContent);
    }

    [Fact]
    public void DoesNotRenderDescriptionWhenEmpty()
    {
        // Arrange & Act - Description defaults to null
        var cut = RenderEmptyState();

        // Assert
        Assert.Empty(cut.FindAll(".sb-empty-state__description"));
    }

    [Fact]
    public void RendersIconWhenProvided()
    {
        // Arrange & Act
        var cut = RenderEmptyState(p => p
            .Add(x => x.Icon, i => i.AddMarkupContent(0, "<span class=\"custom-icon\">📦</span>"))
            .Add(x => x.IconText, (string?)null)); // clear default to test Icon takes precedence

        // Assert
        var iconContainer = cut.Find(".sb-empty-state__icon");
        Assert.NotNull(iconContainer);
        var customIcon = cut.Find(".custom-icon");
        Assert.NotNull(customIcon);
        Assert.Contains("📦", customIcon.TextContent);
    }

    [Fact]
    public void RendersIconTextWhenIconNull()
    {
        // Arrange & Act
        var cut = RenderEmptyState(p => p.Add(x => x.IconText, "🔍"));

        // Assert
        var iconText = cut.Find(".sb-empty-state__icon-text");
        Assert.NotNull(iconText);
        Assert.Contains("🔍", iconText.TextContent);
    }

    [Fact]
    public void IconTakesPrecedenceOverIconText()
    {
        // Arrange & Act
        var cut = RenderEmptyState(p => p
            .Add(x => x.Icon, i => i.AddMarkupContent(0, "<span>Custom</span>"))
            .Add(x => x.IconText, "Fallback"));

        // Assert - Icon used, not IconText
        Assert.Contains("Custom", cut.Markup);
        Assert.Empty(cut.FindAll(".sb-empty-state__icon-text"));
    }

    [Fact]
    public void RendersChildContentWhenProvided()
    {
        // Arrange & Act
        var cut = RenderEmptyState(p => p
            .AddChildContent("<a href=\"#\" class=\"create-link\">Create item</a>"));

        // Assert
        var content = cut.Find(".sb-empty-state__content");
        Assert.NotNull(content);
        var link = cut.Find(".create-link");
        Assert.NotNull(link);
    }

    [Fact]
    public void DoesNotRenderContentWhenChildContentNull()
    {
        // Arrange & Act
        var cut = RenderEmptyState();

        // Assert
        Assert.Empty(cut.FindAll(".sb-empty-state__content"));
    }

    [Fact]
    public void RendersActionsWhenProvided()
    {
        // Arrange & Act
        var cut = RenderEmptyState(p => p
            .Add(x => x.Actions, a => a.AddMarkupContent(0, "<button class=\"action-btn\">Add</button>")));

        // Assert
        var actions = cut.Find(".sb-empty-state__actions");
        Assert.NotNull(actions);
        var btn = cut.Find(".action-btn");
        Assert.NotNull(btn);
        Assert.Contains("Add", btn.TextContent);
    }

    [Fact]
    public void DoesNotRenderActionsWhenNull()
    {
        // Arrange & Act
        var cut = RenderEmptyState();

        // Assert
        Assert.Empty(cut.FindAll(".sb-empty-state__actions"));
    }

    [Fact]
    public void AppliesCustomClass()
    {
        // Arrange & Act
        var cut = RenderEmptyState(p => p.Add(x => x.Class, "my-empty-state"));

        // Assert
        var container = cut.Find(".sb-empty-state");
        Assert.Contains("my-empty-state", container.ClassList);
    }

    [Fact]
    public void RendersCompleteWithAllParts()
    {
        // Arrange & Act
        var cut = Render<SbEmptyState>(p => p
            .Add(x => x.IconText, "📋")
            .Add(x => x.Title, "No tasks")
            .Add(x => x.Description, "Add a task to begin.")
            .AddChildContent("<p>Or import from file.</p>")
            .Add(x => x.Actions, a => a.AddMarkupContent(0, "<button>New Task</button>")));

        // Assert
        Assert.Contains("📋", cut.Markup);
        Assert.Contains("No tasks", cut.Markup);
        Assert.Contains("Add a task to begin.", cut.Markup);
        Assert.Contains("Or import from file.", cut.Markup);
        Assert.Contains("New Task", cut.Markup);
    }
}
