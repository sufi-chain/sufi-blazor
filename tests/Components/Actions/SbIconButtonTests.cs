using Bunit;
using Microsoft.AspNetCore.Components.Web;
using SufiChain.SufiBlazor.Components;
using SufiChain.SufiBlazor.Components.Actions;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Actions;

public class SbIconButtonTests : BunitContext
{
    [Fact]
    public void RendersAsButtonByDefault()
    {
        // Arrange & Act
        var cut = Render<SbIconButton>(parameters => parameters
            .AddChildContent("<span>★</span>"));

        // Assert
        var button = cut.Find("button");
        Assert.NotNull(button);
        Assert.Equal("button", button.GetAttribute("type"));
    }

    [Fact]
    public void RendersChildContent()
    {
        // Arrange & Act
        var cut = Render<SbIconButton>(parameters => parameters
            .AddChildContent("<span class=\"test-icon\">★</span>"));

        // Assert
        Assert.NotNull(cut.Find(".test-icon"));
        Assert.Contains("★", cut.Markup);
    }

    [Fact]
    public void RendersIconParameter()
    {
        // Arrange & Act
        var cut = Render<SbIconButton>(parameters => parameters
            .Add(p => p.Icon, builder => builder.AddMarkupContent(0, "<span class=\"icon-content\">🔍</span>")));

        // Assert
        Assert.NotNull(cut.Find(".icon-content"));
        Assert.Contains("🔍", cut.Markup);
    }

    [Fact]
    public void AppliesDefaultClasses()
    {
        // Arrange & Act
        var cut = Render<SbIconButton>(parameters => parameters
            .AddChildContent("<span>★</span>"));

        // Assert
        var button = cut.Find("button");
        Assert.Contains("sb-icon-button", button.ClassList);
        Assert.Contains("sb-icon-button--ghost", button.ClassList);
        Assert.Contains("sb-icon-button--md", button.ClassList);
    }

    [Fact]
    public void DoesNotAddColorClassWhenDefault()
    {
        // Arrange & Act
        var cut = Render<SbIconButton>(parameters => parameters
            .Add(p => p.Color, SbColor.Default)
            .AddChildContent("<span>★</span>"));

        // Assert
        var button = cut.Find("button");
        Assert.DoesNotContain("sb-icon-button--default", button.ClassList);
    }

    [Fact]
    public void AppliesAriaLabel()
    {
        // Arrange & Act
        var cut = Render<SbIconButton>(parameters => parameters
            .Add(p => p.AriaLabel, "Search")
            .AddChildContent("<span>🔍</span>"));

        // Assert
        var button = cut.Find("button");
        Assert.Equal("Search", button.GetAttribute("aria-label"));
    }

    [Fact]
    public void AppliesTitle()
    {
        // Arrange & Act
        var cut = Render<SbIconButton>(parameters => parameters
            .Add(p => p.Title, "Tooltip text")
            .AddChildContent("<span>★</span>"));

        // Assert
        var button = cut.Find("button");
        Assert.Equal("Tooltip text", button.GetAttribute("title"));
    }

    [Fact]
    public void AppliesDisabledState()
    {
        // Arrange & Act
        var cut = Render<SbIconButton>(parameters => parameters
            .Add(p => p.Disabled, true)
            .AddChildContent("<span>★</span>"));

        // Assert
        var button = cut.Find("button");
        Assert.True(button.HasAttribute("disabled"));
        Assert.Contains("sb-icon-button--disabled", button.ClassList);
    }

    [Fact]
    public void AppliesLoadingState()
    {
        // Arrange & Act
        var cut = Render<SbIconButton>(parameters => parameters
            .Add(p => p.Loading, true)
            .AddChildContent("<span>★</span>"));

        // Assert
        var button = cut.Find("button");
        Assert.True(button.HasAttribute("disabled"));
        Assert.Equal("true", button.GetAttribute("aria-busy"));
        Assert.Contains("sb-icon-button--loading", button.ClassList);
        Assert.NotNull(cut.Find(".sb-icon-button__spinner"));
    }

    [Fact]
    public void AppliesCustomClass()
    {
        // Arrange & Act
        var cut = Render<SbIconButton>(parameters => parameters
            .Add(p => p.Class, "my-custom-class")
            .AddChildContent("<span>★</span>"));

        // Assert
        var button = cut.Find("button");
        Assert.Contains("my-custom-class", button.ClassList);
    }

    [Fact]
    public void AppliesVariant()
    {
        // Arrange & Act
        var cut = Render<SbIconButton>(parameters => parameters
            .Add(p => p.Variant, SbButtonVariant.Outline)
            .AddChildContent("<span>★</span>"));

        // Assert
        var button = cut.Find("button");
        Assert.Contains("sb-icon-button--outline", button.ClassList);
    }

    [Fact]
    public void AppliesSize()
    {
        // Arrange & Act
        var cut = Render<SbIconButton>(parameters => parameters
            .Add(p => p.Size, SbSize.Sm)
            .AddChildContent("<span>★</span>"));

        // Assert
        var button = cut.Find("button");
        Assert.Contains("sb-icon-button--sm", button.ClassList);
    }

    [Fact]
    public void AppliesColor()
    {
        // Arrange & Act
        var cut = Render<SbIconButton>(parameters => parameters
            .Add(p => p.Color, SbColor.Primary)
            .AddChildContent("<span>★</span>"));

        // Assert
        var button = cut.Find("button");
        Assert.Contains("sb-icon-button--primary", button.ClassList);
    }

    [Fact]
    public void SetsTypeAttribute()
    {
        // Arrange & Act
        var cut = Render<SbIconButton>(parameters => parameters
            .Add(p => p.Type, "submit")
            .AddChildContent("<span>★</span>"));

        // Assert
        var button = cut.Find("button");
        Assert.Equal("submit", button.GetAttribute("type"));
    }

    [Fact]
    public void InvokesOnClickWhenClicked()
    {
        // Arrange
        var clicked = false;
        var cut = Render<SbIconButton>(parameters => parameters
            .Add(p => p.OnClick, _ => { clicked = true; return Task.CompletedTask; })
            .AddChildContent("<span>★</span>"));

        // Act
        cut.Find("button").Click();

        // Assert
        Assert.True(clicked);
    }

    [Fact]
    public void DoesNotInvokeOnClickWhenDisabled()
    {
        // Arrange
        var clicked = false;
        var cut = Render<SbIconButton>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.OnClick, _ => { clicked = true; return Task.CompletedTask; })
            .AddChildContent("<span>★</span>"));

        // Act
        cut.Find("button").Click();

        // Assert
        Assert.False(clicked);
    }

    [Fact]
    public void DoesNotInvokeOnClickWhenLoading()
    {
        // Arrange
        var clicked = false;
        var cut = Render<SbIconButton>(parameters => parameters
            .Add(p => p.Loading, true)
            .Add(p => p.OnClick, _ => { clicked = true; return Task.CompletedTask; })
            .AddChildContent("<span>★</span>"));

        // Act
        cut.Find("button").Click();

        // Assert
        Assert.False(clicked);
    }
}
