using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Bunit;
using Bunit.JSInterop;
using SufiChain.SufiBlazor.Components.Overlays;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Overlays;

public class SbMenuTests : BunitContext
{
    public SbMenuTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private static RenderFragment DefaultMenuItems => (b) =>
    {
        b.OpenComponent<SbMenuItem>(0);
        b.AddAttribute(1, "Text", "Item 1");
        b.CloseComponent();
        b.OpenComponent<SbMenuItem>(2);
        b.AddAttribute(3, "Text", "Item 2");
        b.CloseComponent();
    };

    private IRenderedComponent<SbMenu> RenderMenu(
        Action<ComponentParameterCollectionBuilder<SbMenu>>? configure = null)
    {
        return Render<SbMenu>(p =>
        {
            p.Add(x => x.AnchorContent, (RenderFragment)(b => b.AddMarkupContent(0, "<button class=\"anchor-btn\">Open</button>")));
            p.AddChildContent(DefaultMenuItems);
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersAnchorContent()
    {
        // Arrange & Act
        var cut = RenderMenu();

        // Assert
        var anchor = cut.Find(".sb-menu-anchor");
        Assert.NotNull(anchor);
        Assert.Contains("Open", cut.Markup);
        Assert.NotNull(cut.Find(".anchor-btn"));
    }

    [Fact]
    public void DoesNotRenderMenuWhenClosed()
    {
        // Arrange & Act
        var cut = RenderMenu();

        // Assert
        Assert.Empty(cut.FindAll(".sb-menu"));
    }

    [Fact]
    public void RendersMenuWhenOpen()
    {
        // Arrange & Act
        var cut = RenderMenu(p => p.Add(x => x.Open, true));

        // Assert
        var menu = cut.Find(".sb-menu");
        Assert.NotNull(menu);
        Assert.Equal("menu", menu.GetAttribute("role"));
    }

    [Fact]
    public void RendersMenuItemsWhenOpen()
    {
        // Arrange & Act
        var cut = RenderMenu(p => p.Add(x => x.Open, true));

        // Assert
        Assert.Contains("Item 1", cut.Markup);
        Assert.Contains("Item 2", cut.Markup);
        var items = cut.FindAll(".sb-menu-item");
        Assert.Equal(2, items.Count);
    }

    [Fact]
    public void MenuItemsHaveMenuitemRole()
    {
        // Arrange & Act
        var cut = RenderMenu(p => p.Add(x => x.Open, true));

        // Assert
        var items = cut.FindAll("[role=\"menuitem\"]");
        Assert.Equal(2, items.Count);
    }

    [Theory]
    [InlineData(SbPlacement.BottomStart, "bottom-start")]
    [InlineData(SbPlacement.Top, "top")]
    [InlineData(SbPlacement.End, "end")]
    [InlineData(SbPlacement.Start, "start")]
    public void AppliesPlacementClass(SbPlacement placement, string expectedClass)
    {
        // Arrange & Act
        var cut = RenderMenu(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Placement, placement));

        // Assert
        var menu = cut.Find(".sb-menu");
        Assert.Contains($"sb-menu--{expectedClass}", menu.ClassList);
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderMenu(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Class, "my-menu"));

        // Assert
        var menu = cut.Find(".sb-menu");
        Assert.Contains("my-menu", menu.ClassList);
    }

    [Fact]
    public void MenuItemRendersText()
    {
        // Arrange & Act
        var cut = RenderMenu(p => p
            .AddChildContent(b =>
            {
                b.OpenComponent<SbMenuItem>(0);
                b.AddAttribute(1, "Text", "Save File");
                b.CloseComponent();
            })
            .Add(x => x.Open, true));

        // Assert
        Assert.Contains("Save File", cut.Markup);
        var textSpan = cut.Find(".sb-menu-item__text");
        Assert.NotNull(textSpan);
    }

    [Fact]
    public void MenuItemRendersChildContentOverridesText()
    {
        // Arrange & Act
        var cut = RenderMenu(p => p
            .AddChildContent(b =>
            {
                b.OpenComponent<SbMenuItem>(0);
                b.AddAttribute(1, "Text", "Ignore");
                b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "<span>Custom</span>")));
                b.CloseComponent();
            })
            .Add(x => x.Open, true));

        // Assert
        Assert.Contains("Custom", cut.Markup);
    }

    [Fact]
    public void MenuItemRendersShortcut()
    {
        // Arrange & Act
        var cut = RenderMenu(p => p
            .AddChildContent(b =>
            {
                b.OpenComponent<SbMenuItem>(0);
                b.AddAttribute(1, "Text", "New");
                b.AddAttribute(2, "Shortcut", "Ctrl+N");
                b.CloseComponent();
            })
            .Add(x => x.Open, true));

        // Assert
        var shortcut = cut.Find(".sb-menu-item__shortcut");
        Assert.NotNull(shortcut);
        Assert.Equal("Ctrl+N", shortcut.TextContent.Trim());
    }

    [Fact]
    public void DisabledMenuItemHasDisabledClassAndAttribute()
    {
        // Arrange & Act
        var cut = Render<SbMenu>(p =>
        {
            p.Add(x => x.AnchorContent, (RenderFragment)(b => b.AddMarkupContent(0, "<button>Open</button>")));
            p.Add(x => x.Open, true);
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<SbMenuItem>(0);
                b.AddAttribute(1, "Text", "Disabled");
                b.AddAttribute(2, "Disabled", true);
                b.CloseComponent();
            }));
        });

        // Assert
        var item = cut.Find("button.sb-menu-item");
        Assert.Contains("sb-menu-item--disabled", item.GetAttribute("class") ?? "");
        Assert.NotNull(item.GetAttribute("disabled"));
    }

    [Fact]
    public void MenuItemAppliesClassParameter()
    {
        // Arrange & Act
        var cut = Render<SbMenu>(p =>
        {
            p.Add(x => x.AnchorContent, (RenderFragment)(b => b.AddMarkupContent(0, "<button>Open</button>")));
            p.Add(x => x.Open, true);
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<SbMenuItem>(0);
                b.AddAttribute(1, "Text", "Item");
                b.AddAttribute(2, "Class", "my-item");
                b.CloseComponent();
            }));
        });

        // Assert
        var item = cut.Find("button.sb-menu-item");
        Assert.Contains("my-item", item.GetAttribute("class") ?? "");
    }

    [Fact]
    public async Task MenuItemClickInvokesOnClick()
    {
        // Arrange - use default items with OnClick on first item via markup in ChildContent
        var clicked = false;
        var cut = Render<SbMenu>(p =>
        {
            p.Add(x => x.AnchorContent, (RenderFragment)(b => b.AddMarkupContent(0, "<button>Open</button>")));
            p.Add(x => x.Open, true);
            p.Add(x => x.CloseOnItemClick, false);
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<SbMenuItem>(0);
                b.AddAttribute(1, "Text", "Click Me");
                b.AddAttribute(2, "OnClick", EventCallback.Factory.Create(this, () => clicked = true));
                b.CloseComponent();
            }));
        });

        // Act
        var item = cut.Find("button.sb-menu-item");
        await cut.InvokeAsync(() => item.Click());

        // Assert
        Assert.True(clicked);
    }

    [Fact]
    public async Task MenuItemClickClosesMenuWhenCloseOnItemClickTrue()
    {
        // Arrange
        var openChangedValue = true;
        var cut = RenderMenu(p => p
            .Add(x => x.Open, true)
            .Add(x => x.CloseOnItemClick, true)
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, v => openChangedValue = v)));

        // Act
        var item = cut.Find("button.sb-menu-item");
        await cut.InvokeAsync(() => item.Click());

        // Assert - OpenChanged invoked with false
        Assert.False(openChangedValue);
    }

    [Fact]
    public async Task MenuItemClickDoesNotCloseMenuWhenCloseOnItemClickFalse()
    {
        // Arrange
        var openChangedValue = true;
        var cut = RenderMenu(p => p
            .Add(x => x.Open, true)
            .Add(x => x.CloseOnItemClick, false)
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, v => openChangedValue = v)));

        // Act
        var item = cut.Find("button.sb-menu-item");
        await cut.InvokeAsync(() => item.Click());

        // Assert
        Assert.True(openChangedValue);
        Assert.NotNull(cut.Find(".sb-menu"));
    }

    [Fact]
    public async Task DisabledMenuItemClickDoesNotInvokeOnClick()
    {
        // Arrange - disabled item should not fire OnClick
        var clicked = false;
        var cut = Render<SbMenu>(p =>
        {
            p.Add(x => x.AnchorContent, (RenderFragment)(b => b.AddMarkupContent(0, "<button>Open</button>")));
            p.Add(x => x.Open, true);
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<SbMenuItem>(0);
                b.AddAttribute(1, "Text", "Disabled");
                b.AddAttribute(2, "Disabled", true);
                b.AddAttribute(3, "OnClick", EventCallback.Factory.Create(this, () => clicked = true));
                b.CloseComponent();
            }));
        });

        // Act
        var item = cut.Find("button.sb-menu-item");
        await cut.InvokeAsync(() => item.Click());

        // Assert - disabled items don't respond to click
        Assert.False(clicked);
    }

    [Fact]
    public async Task AnchorKeyDownEnterOpensMenu()
    {
        // Arrange
        var openChangedValue = false;
        var cut = RenderMenu(p => p.Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, v => openChangedValue = v)));
        var anchorDiv = cut.Find(".sb-menu-anchor > div");

        // Act
        await cut.InvokeAsync(() => anchorDiv.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "Enter" }));

        // Assert
        Assert.True(openChangedValue);
    }

    [Fact]
    public async Task AnchorKeyDownSpaceOpensMenu()
    {
        // Arrange
        var openChangedValue = false;
        var cut = RenderMenu(p => p.Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, v => openChangedValue = v)));
        var anchorDiv = cut.Find(".sb-menu-anchor > div");

        // Act
        await cut.InvokeAsync(() => anchorDiv.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = " " }));

        // Assert
        Assert.True(openChangedValue);
    }

    [Fact]
    public async Task AnchorKeyDownArrowDownOpensMenu()
    {
        // Arrange
        var openChangedValue = false;
        var cut = RenderMenu(p => p.Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, v => openChangedValue = v)));
        var anchorDiv = cut.Find(".sb-menu-anchor > div");

        // Act
        await cut.InvokeAsync(() => anchorDiv.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "ArrowDown" }));

        // Assert
        Assert.True(openChangedValue);
    }

    [Fact]
    public async Task MenuKeyDownEscapeClosesMenu()
    {
        // Arrange
        var openChangedValue = true;
        var cut = RenderMenu(p => p
            .Add(x => x.Open, true)
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, v => openChangedValue = v)));
        var menu = cut.Find(".sb-menu");

        // Act
        await cut.InvokeAsync(() => menu.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "Escape" }));

        // Assert
        Assert.False(openChangedValue);
    }

    [Fact]
    public async Task MenuKeyDownTabClosesMenu()
    {
        // Arrange
        var openChangedValue = true;
        var cut = RenderMenu(p => p
            .Add(x => x.Open, true)
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, v => openChangedValue = v)));
        var menu = cut.Find(".sb-menu");

        // Act
        await cut.InvokeAsync(() => menu.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "Tab" }));

        // Assert
        Assert.False(openChangedValue);
    }


    [Fact]
    public void MenuItemRendersIcon()
    {
        // Arrange & Act
        var cut = RenderMenu(p => p
            .AddChildContent(b =>
            {
                b.OpenComponent<SbMenuItem>(0);
                b.AddAttribute(1, "Text", "Edit");
                b.AddAttribute(2, "Icon", (RenderFragment)(c => c.AddMarkupContent(0, "<span class=\"icon\">✎</span>")));
                b.CloseComponent();
            })
            .Add(x => x.Open, true));

        // Assert
        var icon = cut.Find(".sb-menu-item__icon");
        Assert.NotNull(icon);
        Assert.Contains("✎", cut.Markup);
    }
}
