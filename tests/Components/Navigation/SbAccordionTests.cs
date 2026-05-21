using Microsoft.AspNetCore.Components;
using Bunit;
using SufiChain.SufiBlazor.Components.Navigation;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Navigation;

public class SbAccordionTests : BunitContext
{
    private IRenderedComponent<SbAccordion> RenderAccordion(
        Action<ComponentParameterCollectionBuilder<SbAccordion>>? configure = null)
    {
        return Render<SbAccordion>(p => configure?.Invoke(p));
    }

    private IRenderedComponent<SbAccordionItem> RenderAccordionItem(
        Action<ComponentParameterCollectionBuilder<SbAccordionItem>>? configure = null)
    {
        return Render<SbAccordionItem>(p => configure?.Invoke(p));
    }

    // --- SbAccordion tests ---

    [Fact]
    public void RendersAccordionStructure()
    {
        // Arrange & Act
        var cut = RenderAccordion();

        // Assert
        var div = cut.Find(".sb-accordion");
        Assert.NotNull(div);
        Assert.Equal("div", div.TagName.ToLowerInvariant());
    }

    [Fact]
    public void RendersChildContent()
    {
        // Arrange & Act
        var cut = RenderAccordion(p => p.AddChildContent("<span>Accordion content</span>"));

        // Assert
        Assert.Contains("Accordion content", cut.Markup);
        Assert.NotNull(cut.Find("span"));
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderAccordion(p => p.Add(x => x.Class, "my-accordion"));

        // Assert
        var div = cut.Find(".sb-accordion");
        Assert.Contains("my-accordion", div.ClassList);
    }

    [Fact]
    public void RendersAccordionItemChildren()
    {
        // Arrange & Act
        var cut = Render<SbAccordion>(p => p.AddChildContent(b =>
        {
            b.OpenComponent<SbAccordionItem>(0);
            b.AddAttribute(1, "Title", "Section 1");
            b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "<p>Content 1</p>")));
            b.CloseComponent();
            b.OpenComponent<SbAccordionItem>(3);
            b.AddAttribute(4, "Title", "Section 2");
            b.AddAttribute(5, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "<p>Content 2</p>")));
            b.CloseComponent();
        }));

        // Assert
        var accordion = cut.Find(".sb-accordion");
        Assert.NotNull(accordion);
        var items = cut.FindAll(".sb-accordion-item");
        Assert.Equal(2, items.Count);
        Assert.Contains("Section 1", cut.Markup);
        Assert.Contains("Section 2", cut.Markup);
        Assert.Contains("Content 1", cut.Markup);
        Assert.Contains("Content 2", cut.Markup);
    }

    // --- SbAccordionItem tests ---

    [Fact]
    public void AccordionItem_RendersStructure()
    {
        // Arrange & Act
        var cut = RenderAccordionItem(p => p.Add(x => x.Title, "Item Title"));

        // Assert
        var div = cut.Find(".sb-accordion-item");
        Assert.NotNull(div);
        Assert.Equal("div", div.TagName.ToLowerInvariant());
        var button = cut.Find(".sb-accordion-item__header");
        Assert.NotNull(button);
        Assert.Equal("button", button.TagName.ToLowerInvariant());
    }

    [Fact]
    public void AccordionItem_RendersTitle()
    {
        // Arrange & Act
        var cut = RenderAccordionItem(p => p.Add(x => x.Title, "My Section Title"));

        // Assert
        var title = cut.Find(".sb-accordion-item__title");
        Assert.NotNull(title);
        Assert.Contains("My Section Title", title.TextContent);
    }

    [Fact]
    public void AccordionItem_RendersChildContent()
    {
        // Arrange & Act
        var cut = RenderAccordionItem(p => p
            .Add(x => x.Title, "Section")
            .AddChildContent("<p>Body content</p>"));

        // Assert
        Assert.Contains("Body content", cut.Markup);
        var body = cut.Find(".sb-accordion-item__body");
        Assert.NotNull(body);
        Assert.NotNull(cut.Find("p"));
    }

    [Fact]
    public void AccordionItem_IsCollapsedByDefault()
    {
        // Arrange & Act
        var cut = RenderAccordionItem(p => p.Add(x => x.Title, "Section"));

        // Assert
        var item = cut.Find(".sb-accordion-item");
        Assert.DoesNotContain("sb-accordion-item--expanded", item.ClassList);
        var content = cut.Find(".sb-accordion-item__content");
        Assert.NotNull(content.GetAttribute("hidden"));
    }

    [Fact]
    public void AccordionItem_ShowsExpandedWhenDefaultExpandedTrue()
    {
        // Arrange & Act
        var cut = RenderAccordionItem(p => p
            .Add(x => x.Title, "Section")
            .Add(x => x.DefaultExpanded, true));

        // Assert - standalone item uses its own _isExpanded
        var item = cut.Find(".sb-accordion-item");
        Assert.Contains("sb-accordion-item--expanded", item.ClassList);
        var content = cut.Find(".sb-accordion-item__content");
        Assert.Null(content.GetAttribute("hidden"));
    }

    [Fact]
    public void AccordionItem_AddsDisabledClassWhenDisabledTrue()
    {
        // Arrange & Act
        var cut = RenderAccordionItem(p => p
            .Add(x => x.Title, "Section")
            .Add(x => x.Disabled, true));

        // Assert
        var item = cut.Find(".sb-accordion-item");
        Assert.Contains("sb-accordion-item--disabled", item.ClassList);
        var button = cut.Find("button");
        Assert.NotNull(button.GetAttribute("disabled"));
    }

    [Fact]
    public async Task AccordionItem_Standalone_ClickTogglesExpansion()
    {
        // Arrange - standalone item (no Parent)
        var cut = RenderAccordionItem(p => p
            .Add(x => x.Title, "Section")
            .AddChildContent("<p>Content</p>"));

        // Assert initially collapsed
        var item = cut.Find(".sb-accordion-item");
        Assert.DoesNotContain("sb-accordion-item--expanded", item.ClassList);

        // Act - click to expand
        var button = cut.Find("button");
        await cut.InvokeAsync(() => button.Click());

        // Assert expanded
        cut.Render();
        item = cut.Find(".sb-accordion-item");
        Assert.Contains("sb-accordion-item--expanded", item.ClassList);

        // Act - click to collapse
        button = cut.Find("button");
        await cut.InvokeAsync(() => button.Click());

        // Assert collapsed again
        cut.Render();
        item = cut.Find(".sb-accordion-item");
        Assert.DoesNotContain("sb-accordion-item--expanded", item.ClassList);
    }

    [Fact]
    public void AccordionItem_InsideAccordion_DefaultExpandedShowsExpanded()
    {
        // Arrange & Act
        var cut = Render<SbAccordion>(p => p.AddChildContent(b =>
        {
            b.OpenComponent<SbAccordionItem>(0);
            b.AddAttribute(1, "Title", "Expanded Section");
            b.AddAttribute(2, "DefaultExpanded", true);
            b.AddAttribute(3, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "Expanded content")));
            b.CloseComponent();
        }));

        // Assert
        var item = cut.Find(".sb-accordion-item");
        Assert.Contains("sb-accordion-item--expanded", item.ClassList);
        Assert.Contains("Expanded content", cut.Markup);
    }

    [Fact]
    public async Task AccordionItem_InsideAccordion_ClickTogglesExpansion()
    {
        // Arrange
        var cut = Render<SbAccordion>(p => p.AddChildContent(b =>
        {
            b.OpenComponent<SbAccordionItem>(0);
            b.AddAttribute(1, "Title", "Section A");
            b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "Content A")));
            b.CloseComponent();
        }));

        // Assert initially collapsed
        var item = cut.Find(".sb-accordion-item");
        Assert.DoesNotContain("sb-accordion-item--expanded", item.ClassList);

        // Act - click to expand
        var button = cut.Find("button.sb-accordion-item__header");
        await cut.InvokeAsync(() => button.Click());

        // Assert expanded
        cut.Render();
        item = cut.Find(".sb-accordion-item");
        Assert.Contains("sb-accordion-item--expanded", item.ClassList);
    }

    [Fact]
    public async Task AccordionItem_Disabled_DoesNotToggleOnClick()
    {
        // Arrange
        var cut = Render<SbAccordion>(p => p.AddChildContent(b =>
        {
            b.OpenComponent<SbAccordionItem>(0);
            b.AddAttribute(1, "Title", "Disabled Section");
            b.AddAttribute(2, "Disabled", true);
            b.AddAttribute(3, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "Hidden")));
            b.CloseComponent();
        }));

        // Act - click disabled button
        var button = cut.Find("button.sb-accordion-item__header");
        await cut.InvokeAsync(() => button.Click());

        // Assert - still collapsed (disabled prevents toggle)
        var item = cut.Find(".sb-accordion-item");
        Assert.DoesNotContain("sb-accordion-item--expanded", item.ClassList);
    }

    [Fact]
    public async Task Accordion_Multiple_AllowsMultipleExpanded()
    {
        // Arrange
        var cut = Render<SbAccordion>(p => p
            .Add(x => x.Multiple, true)
            .AddChildContent(b =>
            {
                b.OpenComponent<SbAccordionItem>(0);
                b.AddAttribute(1, "Title", "Item 1");
                b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "C1")));
                b.CloseComponent();
                b.OpenComponent<SbAccordionItem>(3);
                b.AddAttribute(4, "Title", "Item 2");
                b.AddAttribute(5, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "C2")));
                b.CloseComponent();
            }));

        var items = cut.FindAll(".sb-accordion-item");
        var buttons = cut.FindAll("button.sb-accordion-item__header");
        Assert.Equal(2, buttons.Count);

        // Act - expand first, then second
        await cut.InvokeAsync(() => buttons[0].Click());
        cut.Render();
        await cut.InvokeAsync(() => buttons[1].Click());
        cut.Render();

        // Assert - both expanded
        items = cut.FindAll(".sb-accordion-item");
        Assert.Contains("sb-accordion-item--expanded", items[0].ClassList);
        Assert.Contains("sb-accordion-item--expanded", items[1].ClassList);
    }

    [Fact]
    public async Task Accordion_OnExpandedChanged_FiresWhenItemToggled()
    {
        // Arrange
        HashSet<SbAccordionItem>? receivedItems = null;
        var cut = Render<SbAccordion>(p => p
            .Add(x => x.OnExpandedChanged, EventCallback.Factory.Create<HashSet<SbAccordionItem>>(this, items => receivedItems = items))
            .AddChildContent(b =>
            {
                b.OpenComponent<SbAccordionItem>(0);
                b.AddAttribute(1, "Title", "Section");
                b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "Content")));
                b.CloseComponent();
            }));

        // Act - expand
        var button = cut.Find("button.sb-accordion-item__header");
        await cut.InvokeAsync(() => button.Click());

        // Assert
        Assert.NotNull(receivedItems);
        Assert.Single(receivedItems);
    }
}
