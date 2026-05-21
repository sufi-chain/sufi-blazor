using Microsoft.AspNetCore.Components;
using Bunit;
using SufiChain.SufiBlazor.Components.Navigation;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Navigation;

public class SbTabsTests : BunitContext
{
    private IRenderedComponent<SbTabs> RenderTabs(
        Action<ComponentParameterCollectionBuilder<SbTabs>>? configure = null)
    {
        return Render<SbTabs>(p => configure?.Invoke(p));
    }

    // --- SbTabs tests ---

    [Fact]
    public void RendersTabsStructure()
    {
        // Arrange & Act
        var cut = RenderTabs(p => p.AddChildContent(b =>
        {
            b.OpenComponent<SbTab>(0);
            b.AddAttribute(1, "Label", "Tab 1");
            b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "<p>Content 1</p>")));
            b.CloseComponent();
        }));

        // Assert
        var tabs = cut.Find(".sb-tabs");
        Assert.NotNull(tabs);
        Assert.NotNull(cut.Find(".sb-tabs__list"));
        Assert.Equal("tablist", cut.Find(".sb-tabs__list").GetAttribute("role"));
        Assert.NotNull(cut.Find(".sb-tabs__content"));
    }

    [Fact]
    public void RendersTabLabels()
    {
        // Arrange & Act
        var cut = RenderTabs(p => p.AddChildContent(b =>
        {
            b.OpenComponent<SbTab>(0);
            b.AddAttribute(1, "Label", "First");
            b.CloseComponent();
            b.OpenComponent<SbTab>(2);
            b.AddAttribute(3, "Label", "Second");
            b.CloseComponent();
        }));

        // Assert
        Assert.Contains("First", cut.Markup);
        Assert.Contains("Second", cut.Markup);
        var tabButtons = cut.FindAll("button[role=\"tab\"]");
        Assert.Equal(2, tabButtons.Count);
    }

    [Fact]
    public void FirstTabActiveByDefault()
    {
        // Arrange & Act
        var cut = RenderTabs(p => p.AddChildContent(b =>
        {
            b.OpenComponent<SbTab>(0);
            b.AddAttribute(1, "Label", "Tab 1");
            b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "Content 1")));
            b.CloseComponent();
            b.OpenComponent<SbTab>(3);
            b.AddAttribute(4, "Label", "Tab 2");
            b.AddAttribute(5, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "Content 2")));
            b.CloseComponent();
        }));

        // Assert
        var firstButton = cut.FindAll("button.sb-tabs__tab")[0];
        Assert.Contains("sb-tabs__tab--active", firstButton.ClassList);
        Assert.Contains("Content 1", cut.Markup);
    }

    [Fact]
    public void ActiveIndexSelectsCorrectTab()
    {
        // Arrange & Act
        var cut = RenderTabs(p => p
            .Add(x => x.ActiveIndex, 1)
            .AddChildContent(b =>
            {
                b.OpenComponent<SbTab>(0);
                b.AddAttribute(1, "Label", "Tab 1");
                b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "Content 1")));
                b.CloseComponent();
                b.OpenComponent<SbTab>(3);
                b.AddAttribute(4, "Label", "Tab 2");
                b.AddAttribute(5, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "Content 2")));
                b.CloseComponent();
            }));

        // Assert
        var buttons = cut.FindAll("button.sb-tabs__tab");
        Assert.Contains("sb-tabs__tab--active", buttons[1].ClassList);
        Assert.Contains("Content 2", cut.Markup);
    }

    [Fact]
    public void ActiveTabSelectsById()
    {
        // Arrange & Act
        var cut = RenderTabs(p => p
            .Add(x => x.ActiveTab, "tab-b")
            .AddChildContent(b =>
            {
                b.OpenComponent<SbTab>(0);
                b.AddAttribute(1, "Id", "tab-a");
                b.AddAttribute(2, "Label", "A");
                b.AddAttribute(3, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "A content")));
                b.CloseComponent();
                b.OpenComponent<SbTab>(4);
                b.AddAttribute(5, "Id", "tab-b");
                b.AddAttribute(6, "Label", "B");
                b.AddAttribute(7, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "B content")));
                b.CloseComponent();
            }));

        // Assert
        var buttons = cut.FindAll("button.sb-tabs__tab");
        Assert.Contains("sb-tabs__tab--active", buttons[1].ClassList);
        Assert.Contains("B content", cut.Markup);
    }

    [Fact]
    public async Task ClickingTabInvokesActiveIndexChanged()
    {
        // Arrange
        var received = -1;
        var cut = RenderTabs(p => p
            .Add(x => x.ActiveIndexChanged, EventCallback.Factory.Create<int>(this, i => received = i))
            .AddChildContent(b =>
            {
                b.OpenComponent<SbTab>(0);
                b.AddAttribute(1, "Label", "Tab 1");
                b.CloseComponent();
                b.OpenComponent<SbTab>(2);
                b.AddAttribute(3, "Label", "Tab 2");
                b.CloseComponent();
            }));

        // Act - click second tab
        var buttons = cut.FindAll("button.sb-tabs__tab");
        await cut.InvokeAsync(() => buttons[1].Click());

        // Assert
        Assert.Equal(1, received);
    }

    [Fact]
    public async Task ClickingTabInvokesActiveTabChanged()
    {
        // Arrange
        string? receivedId = null;
        var cut = RenderTabs(p => p
            .Add(x => x.ActiveTabChanged, EventCallback.Factory.Create<string?>(this, id => receivedId = id))
            .AddChildContent(b =>
            {
                b.OpenComponent<SbTab>(0);
                b.AddAttribute(1, "Id", "first");
                b.AddAttribute(2, "Label", "First");
                b.CloseComponent();
                b.OpenComponent<SbTab>(3);
                b.AddAttribute(4, "Id", "second");
                b.AddAttribute(5, "Label", "Second");
                b.CloseComponent();
            }));

        // Act
        var buttons = cut.FindAll("button.sb-tabs__tab");
        await cut.InvokeAsync(() => buttons[1].Click());

        // Assert
        Assert.Equal("second", receivedId);
    }

    [Fact]
    public void DisabledTabHasDisabledClassAndAttribute()
    {
        // Arrange & Act
        var cut = RenderTabs(p => p.AddChildContent(b =>
        {
            b.OpenComponent<SbTab>(0);
            b.AddAttribute(1, "Label", "Active");
            b.CloseComponent();
            b.OpenComponent<SbTab>(2);
            b.AddAttribute(3, "Label", "Disabled");
            b.AddAttribute(4, "Disabled", true);
            b.CloseComponent();
        }));

        // Assert
        var buttons = cut.FindAll("button.sb-tabs__tab");
        Assert.Contains("sb-tabs__tab--disabled", buttons[1].ClassList);
        Assert.NotNull(buttons[1].GetAttribute("disabled"));
    }

    [Fact]
    public async Task ClickingDisabledTabDoesNotChangeSelection()
    {
        // Arrange
        var received = -1;
        var cut = RenderTabs(p => p
            .Add(x => x.ActiveIndex, 0)
            .Add(x => x.ActiveIndexChanged, EventCallback.Factory.Create<int>(this, i => received = i))
            .AddChildContent(b =>
            {
                b.OpenComponent<SbTab>(0);
                b.AddAttribute(1, "Label", "Active");
                b.CloseComponent();
                b.OpenComponent<SbTab>(2);
                b.AddAttribute(3, "Label", "Disabled");
                b.AddAttribute(4, "Disabled", true);
                b.CloseComponent();
            }));

        // Act - try to click disabled tab
        var buttons = cut.FindAll("button.sb-tabs__tab");
        await cut.InvokeAsync(() => buttons[1].Click());

        // Assert - callback not invoked
        Assert.Equal(-1, received);
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderTabs(p => p
            .Add(x => x.Class, "my-tabs")
            .AddChildContent(b =>
            {
                b.OpenComponent<SbTab>(0);
                b.AddAttribute(1, "Label", "Tab 1");
                b.CloseComponent();
            }));

        // Assert
        var tabs = cut.Find(".sb-tabs");
        Assert.Contains("my-tabs", tabs.ClassList);
    }

    [Fact]
    public void VerticalAddsVerticalClass()
    {
        // Arrange & Act
        var cut = RenderTabs(p => p
            .Add(x => x.Vertical, true)
            .AddChildContent(b =>
            {
                b.OpenComponent<SbTab>(0);
                b.AddAttribute(1, "Label", "Tab 1");
                b.CloseComponent();
            }));

        // Assert
        var tabs = cut.Find(".sb-tabs");
        Assert.Contains("sb-tabs--vertical", tabs.ClassList);
    }

    [Fact]
    public void RendersTabIconWhenProvided()
    {
        // Arrange & Act
        var cut = RenderTabs(p => p.AddChildContent(b =>
        {
            b.OpenComponent<SbTab>(0);
            b.AddAttribute(1, "Label", "With Icon");
            b.AddAttribute(2, "Icon", (RenderFragment)(c => c.AddMarkupContent(0, "<span>ico</span>")));
            b.CloseComponent();
        }));

        // Assert
        var icon = cut.Find(".sb-tabs__tab-icon");
        Assert.NotNull(icon);
        Assert.Contains("ico", icon.InnerHtml);
    }

    [Fact]
    public void RendersTabBadgeWhenProvided()
    {
        // Arrange & Act
        var cut = RenderTabs(p => p.AddChildContent(b =>
        {
            b.OpenComponent<SbTab>(0);
            b.AddAttribute(1, "Label", "Inbox");
            b.AddAttribute(2, "Badge", (RenderFragment)(c => c.AddMarkupContent(0, "5")));
            b.CloseComponent();
        }));

        // Assert
        var badge = cut.Find(".sb-tabs__tab-badge");
        Assert.NotNull(badge);
        Assert.Contains("5", badge.TextContent);
    }

    [Fact]
    public void TabPanelHasCorrectRoleAndId()
    {
        // Arrange & Act
        var cut = RenderTabs(p => p.AddChildContent(b =>
        {
            b.OpenComponent<SbTab>(0);
            b.AddAttribute(1, "Label", "Tab 1");
            b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "Content")));
            b.CloseComponent();
        }));

        // Assert
        var panel = cut.Find(".sb-tab-panel");
        Assert.NotNull(panel);
        Assert.Equal("tabpanel", panel.GetAttribute("role"));
        Assert.NotNull(panel.GetAttribute("id"));
        Assert.StartsWith("tab-panel-", panel.GetAttribute("id")!);
    }

    [Fact]
    public void ActiveTabPanelNotHidden()
    {
        // Arrange & Act
        var cut = RenderTabs(p => p.AddChildContent(b =>
        {
            b.OpenComponent<SbTab>(0);
            b.AddAttribute(1, "Label", "Tab 1");
            b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "Visible")));
            b.CloseComponent();
        }));

        // Assert
        var panel = cut.Find(".sb-tab-panel--active");
        Assert.NotNull(panel);
        Assert.Null(panel.GetAttribute("hidden"));
    }

    [Fact]
    public void LazyLoadOnlyRendersActiveTabContent()
    {
        // Arrange & Act
        var cut = RenderTabs(p => p
            .Add(x => x.LazyLoad, true)
            .Add(x => x.ActiveIndex, 0)
            .AddChildContent(b =>
            {
                b.OpenComponent<SbTab>(0);
                b.AddAttribute(1, "Label", "Tab 1");
                b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "Lazy 1")));
                b.CloseComponent();
                b.OpenComponent<SbTab>(3);
                b.AddAttribute(4, "Label", "Tab 2");
                b.AddAttribute(5, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "Lazy 2")));
                b.CloseComponent();
            }));

        // Assert - only first tab content rendered
        Assert.Contains("Lazy 1", cut.Markup);
        Assert.DoesNotContain("Lazy 2", cut.Markup);
    }

    [Fact]
    public async Task LazyLoadRendersTabContentAfterActivation()
    {
        // Arrange
        var cut = RenderTabs(p => p
            .Add(x => x.LazyLoad, true)
            .AddChildContent(b =>
            {
                b.OpenComponent<SbTab>(0);
                b.AddAttribute(1, "Label", "Tab 1");
                b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "Lazy 1")));
                b.CloseComponent();
                b.OpenComponent<SbTab>(3);
                b.AddAttribute(4, "Label", "Tab 2");
                b.AddAttribute(5, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "Lazy 2")));
                b.CloseComponent();
            }));

        // Act - click second tab
        var buttons = cut.FindAll("button.sb-tabs__tab");
        await cut.InvokeAsync(() => buttons[1].Click());
        cut.Render();

        // Assert - now both visible (second was activated)
        Assert.Contains("Lazy 1", cut.Markup);
        Assert.Contains("Lazy 2", cut.Markup);
    }

    // --- SbTab tests ---

    [Fact]
    public void Tab_RendersContentWhenActive()
    {
        // Arrange & Act
        var cut = RenderTabs(p => p
            .Add(x => x.ActiveIndex, 0)
            .AddChildContent(b =>
            {
                b.OpenComponent<SbTab>(0);
                b.AddAttribute(1, "Label", "Tab 1");
                b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "Tab content")));
                b.CloseComponent();
            }));

        // Assert
        var panel = cut.Find(".sb-tab-panel");
        Assert.NotNull(panel);
        Assert.Contains("Tab content", cut.Markup);
    }

    [Fact]
    public void Tab_WorksInsideSbTabs()
    {
        // Arrange & Act
        var cut = RenderTabs(p => p.AddChildContent(b =>
        {
            b.OpenComponent<SbTab>(0);
            b.AddAttribute(1, "Id", "settings");
            b.AddAttribute(2, "Label", "Settings");
            b.AddAttribute(3, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "Settings panel")));
            b.CloseComponent();
        }));

        // Assert
        var tabs = cut.Find(".sb-tabs");
        Assert.NotNull(tabs);
        Assert.Contains("Settings", cut.Markup);
        Assert.Contains("Settings panel", cut.Markup);
    }
}
