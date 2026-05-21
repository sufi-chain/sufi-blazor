using Bunit;
using SufiChain.SufiBlazor.Components.Navigation;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Navigation;

public class SbBreadcrumbTests : BunitContext
{
    private IRenderedComponent<SbBreadcrumb> RenderBreadcrumb(
        Action<ComponentParameterCollectionBuilder<SbBreadcrumb>>? configure = null)
    {
        return Render<SbBreadcrumb>(p => configure?.Invoke(p));
    }

    [Fact]
    public void RendersBreadcrumbStructure()
    {
        // Arrange & Act
        var cut = RenderBreadcrumb(p => p.Add(x => x.Items, new List<SbBreadcrumbItem> { new("Home", "/") }));

        // Assert
        var nav = cut.Find("nav.sb-breadcrumb");
        Assert.NotNull(nav);
        Assert.Equal("Breadcrumb", nav.GetAttribute("aria-label"));
        var list = cut.Find("ol.sb-breadcrumb__list");
        Assert.NotNull(list);
    }

    [Fact]
    public void RendersEmptyListWhenNoItems()
    {
        // Arrange & Act
        var cut = RenderBreadcrumb(p => p.Add(x => x.Items, new List<SbBreadcrumbItem>()));

        // Assert
        var nav = cut.Find("nav.sb-breadcrumb");
        Assert.NotNull(nav);
        var items = cut.FindAll(".sb-breadcrumb__item");
        Assert.Empty(items);
    }

    [Fact]
    public void RendersSingleItemAsCurrent()
    {
        // Arrange & Act
        var cut = RenderBreadcrumb(p => p.Add(x => x.Items, new List<SbBreadcrumbItem>
        {
            new("Products")
        }));

        // Assert
        var item = cut.Find(".sb-breadcrumb__item");
        Assert.NotNull(item);
        Assert.Contains("sb-breadcrumb__item--current", item.ClassList);
        var text = cut.Find(".sb-breadcrumb__text");
        Assert.NotNull(text);
        Assert.Contains("Products", text.TextContent);
        Assert.Equal("page", text.GetAttribute("aria-current"));
        Assert.Empty(cut.FindAll(".sb-breadcrumb__separator"));
    }

    [Fact]
    public void RendersMultipleItemsWithLinksAndSeparator()
    {
        // Arrange & Act
        var cut = RenderBreadcrumb(p => p.Add(x => x.Items, new List<SbBreadcrumbItem>
        {
            new("Home", "/"),
            new("Products", "/products"),
            new("Detail")
        }));

        // Assert
        var items = cut.FindAll(".sb-breadcrumb__item");
        Assert.Equal(3, items.Count);

        var links = cut.FindAll("a.sb-breadcrumb__link");
        Assert.Equal(2, links.Count);
        Assert.Equal("/", links[0].GetAttribute("href"));
        Assert.Contains("Home", links[0].TextContent);
        Assert.Equal("/products", links[1].GetAttribute("href"));
        Assert.Contains("Products", links[1].TextContent);

        var current = cut.Find(".sb-breadcrumb__text");
        Assert.NotNull(current);
        Assert.Contains("Detail", current.TextContent);

        var separators = cut.FindAll(".sb-breadcrumb__separator");
        Assert.Equal(2, separators.Count);
        Assert.All(separators, s => Assert.Equal("true", s.GetAttribute("aria-hidden")));
    }

    [Fact]
    public void UsesDefaultSeparator()
    {
        // Arrange & Act
        var cut = RenderBreadcrumb(p => p.Add(x => x.Items, new List<SbBreadcrumbItem>
        {
            new("A", "/a"),
            new("B")
        }));

        // Assert
        var separators = cut.FindAll(".sb-breadcrumb__separator");
        Assert.Single(separators);
        Assert.Equal("/", separators[0].TextContent);
    }

    [Fact]
    public void AppliesCustomSeparator()
    {
        // Arrange & Act
        var cut = RenderBreadcrumb(p => p
            .Add(x => x.Items, new List<SbBreadcrumbItem> { new("A", "/a"), new("B") })
            .Add(x => x.Separator, " > "));

        // Assert
        var separator = cut.Find(".sb-breadcrumb__separator");
        Assert.NotNull(separator);
        Assert.Equal(" > ", separator.TextContent);
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderBreadcrumb(p => p
            .Add(x => x.Items, new List<SbBreadcrumbItem> { new("Home") })
            .Add(x => x.Class, "my-breadcrumb"));

        // Assert
        var nav = cut.Find("nav.sb-breadcrumb");
        Assert.Contains("my-breadcrumb", nav.ClassList);
    }

    [Fact]
    public void ItemWithEmptyHrefRendersAsSpan()
    {
        // Arrange - last item has no Href, penultimate has empty Href
        var cut = RenderBreadcrumb(p => p.Add(x => x.Items, new List<SbBreadcrumbItem>
        {
            new("Home", "/"),
            new("Current") { Href = "" }
        }));

        // Assert - "Current" is last, so it renders as span (current page). Second item has Href="" so !string.IsNullOrEmpty("") is false - it goes to else branch and renders as span
        var items = cut.FindAll(".sb-breadcrumb__item");
        Assert.Equal(2, items.Count);
        // First: Home has Href="/" -> link. Second: Current has Href="" and is last -> span
        var spans = cut.FindAll(".sb-breadcrumb__text");
        Assert.Single(spans); // Last item renders as span
        var links = cut.FindAll("a.sb-breadcrumb__link");
        Assert.Single(links); // Only Home is a link
    }

    [Fact]
    public void ItemWithNullHrefRendersAsSpan()
    {
        // Arrange - not last item but Href is null
        var cut = RenderBreadcrumb(p => p.Add(x => x.Items, new List<SbBreadcrumbItem>
        {
            new("Home", "/"),
            new("Disabled") { Href = null },
            new("Current")
        }));

        // Assert - "Disabled" has null Href, so it goes to else branch (span)
        var links = cut.FindAll("a.sb-breadcrumb__link");
        Assert.Single(links); // Only Home
        Assert.Contains("Disabled", cut.Markup);
        Assert.Contains("Current", cut.Markup);
    }

    [Fact]
    public void RendersIconWhenPresent()
    {
        // Arrange & Act
        var cut = RenderBreadcrumb(p => p.Add(x => x.Items, new List<SbBreadcrumbItem>
        {
            new("Home", "/") { Icon = "home" },
            new("Current")
        }));

        // Assert
        var icon = cut.Find(".sb-breadcrumb__icon");
        Assert.NotNull(icon);
        Assert.Contains("home", icon.TextContent);
    }
}
