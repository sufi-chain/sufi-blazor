using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Bunit;
using SufiChain.SufiBlazor.Components.Surfaces;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Surfaces;

public class SbCardTests : BunitContext
{
    private IRenderedComponent<SbCard> RenderCard(
        Action<ComponentParameterCollectionBuilder<SbCard>>? configure = null)
    {
        return Render<SbCard>(p =>
        {
            p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "<span>Card body</span>")));
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersAsDivByDefault()
    {
        // Arrange & Act
        var cut = RenderCard();

        // Assert
        var card = cut.Find(".sb-card");
        Assert.NotNull(card);
        Assert.Equal("div", card.TagName.ToLowerInvariant());
    }

    [Fact]
    public void RendersChildContent()
    {
        // Arrange & Act
        var cut = RenderCard();

        // Assert
        Assert.Contains("Card body", cut.Markup);
        var body = cut.Find(".sb-card__body");
        Assert.NotNull(body);
    }

    [Fact]
    public void RendersHeaderWhenProvided()
    {
        // Arrange & Act
        var cut = RenderCard(p => p.Add(x => x.Header, (RenderFragment)(b => b.AddMarkupContent(0, "<h2>Card Title</h2>"))));

        // Assert
        var header = cut.Find(".sb-card__header");
        Assert.NotNull(header);
        Assert.Contains("Card Title", cut.Markup);
    }

    [Fact]
    public void RendersFooterWhenProvided()
    {
        // Arrange & Act
        var cut = RenderCard(p => p.Add(x => x.Footer, (RenderFragment)(b => b.AddMarkupContent(0, "<button>Action</button>"))));

        // Assert
        var footer = cut.Find(".sb-card__footer");
        Assert.NotNull(footer);
        Assert.Contains("Action", cut.Markup);
    }

    [Fact]
    public void AppliesElevationClass()
    {
        // Arrange & Act
        var cut = RenderCard(p => p.Add(x => x.Elevation, 2));

        // Assert
        var card = cut.Find(".sb-card");
        Assert.Contains("sb-card--elevation-2", card.ClassList);
    }

    [Theory]
    [InlineData(1, "1")]
    [InlineData(3, "3")]
    [InlineData(5, "5")]
    public void AppliesCorrectElevationClass(int elevation, string expectedSuffix)
    {
        // Arrange & Act
        var cut = RenderCard(p => p.Add(x => x.Elevation, elevation));

        // Assert
        var card = cut.Find(".sb-card");
        Assert.Contains($"sb-card--elevation-{expectedSuffix}", card.ClassList);
    }

    [Fact]
    public void ClampsElevationToFive()
    {
        // Arrange & Act
        var cut = RenderCard(p => p.Add(x => x.Elevation, 10));

        // Assert
        var card = cut.Find(".sb-card");
        Assert.Contains("sb-card--elevation-5", card.ClassList);
    }

    [Fact]
    public void AppliesOutlinedClassWhenOutlinedTrue()
    {
        // Arrange & Act
        var cut = RenderCard(p => p.Add(x => x.Outlined, true));

        // Assert
        var card = cut.Find(".sb-card");
        Assert.Contains("sb-card--outlined", card.ClassList);
    }

    [Fact]
    public void OutlinedSkipsElevationClass()
    {
        // Arrange & Act
        var cut = RenderCard(p => p
            .Add(x => x.Elevation, 2)
            .Add(x => x.Outlined, true));

        // Assert
        var card = cut.Find(".sb-card");
        Assert.Contains("sb-card--outlined", card.ClassList);
        Assert.DoesNotContain("sb-card--elevation", card.ClassList);
    }

    [Fact]
    public void AppliesNoPaddingClass()
    {
        // Arrange & Act
        var cut = RenderCard(p => p.Add(x => x.NoPadding, true));

        // Assert
        var card = cut.Find(".sb-card");
        Assert.Contains("sb-card--no-padding", card.ClassList);
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderCard(p => p.Add(x => x.Class, "my-card"));

        // Assert
        var card = cut.Find(".sb-card");
        Assert.Contains("my-card", card.ClassList);
    }

    [Fact]
    public void AppliesStyleParameter()
    {
        // Arrange & Act
        var cut = RenderCard(p => p.Add(x => x.Style, "width: 300px"));

        // Assert
        var card = cut.Find(".sb-card");
        Assert.Contains("width: 300px", card.GetAttribute("style") ?? "");
    }

    [Fact]
    public void RendersAsLinkWhenHrefSet()
    {
        // Arrange & Act
        var cut = RenderCard(p => p.Add(x => x.Href, "/details"));

        // Assert
        var card = cut.Find(".sb-card");
        Assert.Equal("a", card.TagName.ToLowerInvariant());
        Assert.Equal("/details", card.GetAttribute("href"));
        Assert.Contains("sb-card--clickable", card.ClassList);
    }

    [Fact]
    public void RendersAsButtonWhenOnClickSet()
    {
        // Arrange & Act
        var cut = RenderCard(p => p.Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => { })));

        // Assert
        var card = cut.Find(".sb-card");
        Assert.Equal("button", card.TagName.ToLowerInvariant());
        Assert.Contains("sb-card--clickable", card.ClassList);
    }

    [Fact]
    public void HrefTakesPrecedenceOverOnClick()
    {
        // When both Href and OnClick, renders as link
        var cut = RenderCard(p => p
            .Add(x => x.Href, "/page")
            .Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => { })));

        var card = cut.Find(".sb-card");
        Assert.Equal("a", card.TagName.ToLowerInvariant());
        Assert.Equal("/page", card.GetAttribute("href"));
    }

    [Fact]
    public async Task OnClickInvokesCallback()
    {
        // Arrange
        var clicked = false;
        var cut = RenderCard(p => p.Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => clicked = true)));

        // Act
        var button = cut.Find("button.sb-card");
        await cut.InvokeAsync(() => button.Click());

        // Assert
        Assert.True(clicked);
    }

    [Fact]
    public void PassesAdditionalAttributes()
    {
        // Arrange & Act
        var cut = RenderCard(p => p
            .Add(x => x.AdditionalAttributes, new Dictionary<string, object> { ["data-test"] = "card" }));

        // Assert
        var card = cut.Find(".sb-card");
        Assert.Equal("card", card.GetAttribute("data-test"));
    }

    [Fact]
    public void NoElevationClassWhenElevationZero()
    {
        // Arrange & Act
        var cut = RenderCard(p => p.Add(x => x.Elevation, 0));

        // Assert - Elevation > 0 check means no elevation class
        var card = cut.Find(".sb-card");
        Assert.DoesNotContain("sb-card--elevation", card.ClassList);
    }
}
