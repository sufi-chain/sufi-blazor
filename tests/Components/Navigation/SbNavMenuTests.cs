using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using SufiChain.SufiBlazor.Components.Navigation;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Navigation;

public class SbNavMenuTests : BunitContext
{
    private IRenderedComponent<SbNavMenu> RenderNavMenu(
        Action<ComponentParameterCollectionBuilder<SbNavMenu>>? configure = null)
    {
        return Render<SbNavMenu>(p => configure?.Invoke(p));
    }

    private IRenderedComponent<SbNavItem> RenderNavItem(
        Action<ComponentParameterCollectionBuilder<SbNavItem>>? configure = null)
    {
        return Render<SbNavItem>(p => configure?.Invoke(p));
    }

    // --- SbNavMenu tests ---

    [Fact]
    public void RendersNavMenuStructure()
    {
        // Arrange & Act
        var cut = RenderNavMenu(p => p.AddChildContent("<span>Nav content</span>"));

        // Assert
        var nav = cut.Find("nav.sb-nav-menu");
        Assert.NotNull(nav);
        Assert.Equal("nav", nav.TagName.ToLowerInvariant());
        Assert.Equal("Navigation", nav.GetAttribute("aria-label"));
    }

    [Fact]
    public void RendersChildContent()
    {
        // Arrange & Act
        var cut = RenderNavMenu(p => p.AddChildContent("<ul><li>Item</li></ul>"));

        // Assert
        Assert.Contains("Item", cut.Markup);
        Assert.NotNull(cut.Find("ul"));
    }

    [Fact]
    public void UsesCustomAriaLabel()
    {
        // Arrange & Act
        var cut = RenderNavMenu(p => p
            .Add(x => x.AriaLabel, "Main menu")
            .AddChildContent("<span></span>"));

        // Assert
        var nav = cut.Find("nav");
        Assert.Equal("Main menu", nav.GetAttribute("aria-label"));
    }

    [Fact]
    public void AddsCollapsedClassWhenCollapsedTrue()
    {
        // Arrange & Act
        var cut = RenderNavMenu(p => p
            .Add(x => x.Collapsed, true)
            .AddChildContent("<span></span>"));

        // Assert
        var nav = cut.Find("nav.sb-nav-menu");
        Assert.Contains("sb-nav-menu--collapsed", nav.ClassList);
    }

    [Fact]
    public void DoesNotAddCollapsedClassWhenCollapsedFalse()
    {
        // Arrange & Act
        var cut = RenderNavMenu(p => p
            .Add(x => x.Collapsed, false)
            .AddChildContent("<span></span>"));

        // Assert
        var nav = cut.Find("nav.sb-nav-menu");
        Assert.DoesNotContain("sb-nav-menu--collapsed", nav.ClassList);
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderNavMenu(p => p
            .Add(x => x.Class, "sidebar-menu")
            .AddChildContent("<span></span>"));

        // Assert
        var nav = cut.Find("nav.sb-nav-menu");
        Assert.Contains("sidebar-menu", nav.ClassList);
    }

    [Fact]
    public void AppliesStyleParameter()
    {
        // Arrange & Act
        var cut = RenderNavMenu(p => p
            .Add(x => x.Style, "width: 200px;")
            .AddChildContent("<span></span>"));

        // Assert
        var nav = cut.Find("nav");
        var style = nav.GetAttribute("style");
        Assert.Contains("width: 200px", style);
    }

    [Fact]
    public async Task ToggleAsync_FlipsCollapsedAndInvokesCallback()
    {
        // Arrange
        var received = false;
        var cut = RenderNavMenu(p => p
            .Add(x => x.Collapsed, false)
            .Add(x => x.CollapsedChanged, EventCallback.Factory.Create<bool>(this, v => received = v))
            .AddChildContent("<span></span>"));

        // Act
        await cut.InvokeAsync(() => cut.Instance.ToggleAsync());

        // Assert
        Assert.True(received);
        cut.Render();
        var nav = cut.Find("nav.sb-nav-menu");
        Assert.Contains("sb-nav-menu--collapsed", nav.ClassList);
    }

    [Fact]
    public void PassesAdditionalAttributes()
    {
        // Arrange & Act
        var cut = RenderNavMenu(p => p
            .Add(x => x.AdditionalAttributes, new Dictionary<string, object> { { "data-testid", "main-nav" } })
            .AddChildContent("<span></span>"));

        // Assert
        var nav = cut.Find("nav");
        Assert.Equal("main-nav", nav.GetAttribute("data-testid"));
    }

    [Fact]
    public void RendersNavItemChildren()
    {
        // Arrange & Act
        var cut = Render<SbNavMenu>(p => p.AddChildContent(b =>
        {
            b.OpenComponent<SbNavItem>(0);
            b.AddAttribute(1, "Href", "/");
            b.AddAttribute(2, "Text", "Home");
            b.CloseComponent();
            b.OpenComponent<SbNavItem>(3);
            b.AddAttribute(4, "Href", "/about");
            b.AddAttribute(5, "Text", "About");
            b.CloseComponent();
        }));

        // Assert
        var nav = cut.Find("nav.sb-nav-menu");
        Assert.NotNull(nav);
        var items = cut.FindAll("a.sb-nav-item");
        Assert.Equal(2, items.Count);
        Assert.Contains("Home", cut.Markup);
        Assert.Contains("About", cut.Markup);
    }

    // --- SbNavItem tests ---

    [Fact]
    public void NavItem_RendersAsAnchor()
    {
        // Arrange & Act
        var cut = RenderNavItem(p => p
            .Add(x => x.Href, "/dashboard")
            .Add(x => x.Text, "Dashboard"));

        // Assert
        var anchor = cut.Find("a.sb-nav-item");
        Assert.NotNull(anchor);
        Assert.Equal("/dashboard", anchor.GetAttribute("href"));
    }

    [Fact]
    public void NavItem_RendersText()
    {
        // Arrange & Act
        var cut = RenderNavItem(p => p
            .Add(x => x.Href, "/")
            .Add(x => x.Text, "Home"));

        // Assert
        var text = cut.Find(".sb-nav-item__text");
        Assert.NotNull(text);
        Assert.Contains("Home", text.TextContent);
    }

    [Fact]
    public void NavItem_RendersIconWhenProvided()
    {
        // Arrange & Act
        var cut = RenderNavItem(p => p
            .Add(x => x.Href, "/")
            .Add(x => x.Text, "Home")
            .Add(x => x.Icon, (RenderFragment)(b => b.AddMarkupContent(0, "<span>ico</span>"))));

        // Assert
        var icon = cut.Find(".sb-nav-item__icon");
        Assert.NotNull(icon);
        Assert.Contains("ico", icon.InnerHtml);
    }

    [Fact]
    public void NavItem_RendersBadgeWhenProvided()
    {
        // Arrange & Act
        var cut = RenderNavItem(p => p
            .Add(x => x.Href, "/inbox")
            .Add(x => x.Text, "Inbox")
            .Add(x => x.Badge, (RenderFragment)(b => b.AddMarkupContent(0, "3"))));

        // Assert
        var badge = cut.Find(".sb-nav-item__badge");
        Assert.NotNull(badge);
        Assert.Contains("3", badge.TextContent);
    }

    [Fact]
    public void NavItem_AddsActiveClassWhenCurrentPage()
    {
        // Arrange - navigate to /products then render item with Href="/products"
        var navMan = Services.GetRequiredService<BunitNavigationManager>();
        navMan.NavigateTo("http://localhost/products");

        var cut = RenderNavItem(p => p
            .Add(x => x.Href, "/products")
            .Add(x => x.Text, "Products"));

        // Assert
        var anchor = cut.Find("a.sb-nav-item");
        Assert.Contains("sb-nav-item--active", anchor.ClassList);
    }

    [Fact]
    public void NavItem_DoesNotAddActiveClassWhenNotCurrentPage()
    {
        // Arrange - default URI is base, render item with Href="/other"
        var cut = RenderNavItem(p => p
            .Add(x => x.Href, "/other-page")
            .Add(x => x.Text, "Other"));

        // Assert
        var anchor = cut.Find("a.sb-nav-item");
        Assert.DoesNotContain("sb-nav-item--active", anchor.ClassList);
    }

    [Fact]
    public void NavItem_AddsDisabledClassWhenDisabledTrue()
    {
        // Arrange & Act
        var cut = RenderNavItem(p => p
            .Add(x => x.Href, "/")
            .Add(x => x.Text, "Disabled")
            .Add(x => x.Disabled, true));

        // Assert
        var anchor = cut.Find("a.sb-nav-item");
        Assert.Contains("sb-nav-item--disabled", anchor.ClassList);
    }

    [Fact]
    public void NavItem_AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderNavItem(p => p
            .Add(x => x.Href, "/")
            .Add(x => x.Text, "Item")
            .Add(x => x.Class, "custom-item"));

        // Assert
        var anchor = cut.Find("a.sb-nav-item");
        Assert.Contains("custom-item", anchor.ClassList);
    }

    [Fact]
    public void NavItem_AppliesStyleParameter()
    {
        // Arrange & Act
        var cut = RenderNavItem(p => p
            .Add(x => x.Href, "/")
            .Add(x => x.Text, "Item")
            .Add(x => x.Style, "font-weight: bold;"));

        // Assert
        var anchor = cut.Find("a");
        var style = anchor.GetAttribute("style");
        Assert.Contains("font-weight: bold", style);
    }

    [Fact]
    public void NavItem_PassesAdditionalAttributes()
    {
        // Arrange & Act
        var cut = RenderNavItem(p => p
            .Add(x => x.Href, "/")
            .Add(x => x.Text, "Item")
            .Add(x => x.AdditionalAttributes, new Dictionary<string, object> { { "data-testid", "nav-home" } }));

        // Assert
        var anchor = cut.Find("a");
        Assert.Equal("nav-home", anchor.GetAttribute("data-testid"));
    }

    [Fact]
    public void NavItem_WorksInsideSbNavMenu()
    {
        // Arrange & Act
        var cut = Render<SbNavMenu>(p => p.AddChildContent(b =>
        {
            b.OpenComponent<SbNavItem>(0);
            b.AddAttribute(1, "Href", "/dashboard");
            b.AddAttribute(2, "Text", "Dashboard");
            b.CloseComponent();
        }));

        // Assert
        var nav = cut.Find("nav.sb-nav-menu");
        var item = cut.Find("a.sb-nav-item");
        Assert.NotNull(nav);
        Assert.NotNull(item);
        Assert.Equal("/dashboard", item.GetAttribute("href"));
        Assert.Contains("Dashboard", cut.Markup);
    }
}
