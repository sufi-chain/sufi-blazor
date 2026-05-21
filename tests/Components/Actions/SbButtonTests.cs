using Bunit;
using Microsoft.AspNetCore.Components.Web;
using SufiChain.SufiBlazor.Components;
using SufiChain.SufiBlazor.Components.Actions;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Actions;

public class SbButtonTests : BunitContext
{
    [Fact]
    public void RendersAsButtonByDefault()
    {
        // Arrange & Act
        var cut = Render<SbButton>(parameters => parameters
            .AddChildContent("Click me"));

        // Assert
        var button = cut.Find("button");
        Assert.NotNull(button);
        Assert.Equal("button", button.GetAttribute("type"));
    }

    [Fact]
    public void RendersChildContent()
    {
        // Arrange
        const string expectedContent = "Submit";

        // Act
        var cut = Render<SbButton>(parameters => parameters
            .AddChildContent(expectedContent));

        // Assert
        var content = cut.Find(".sb-button__content");
        Assert.NotNull(content);
        Assert.Contains(expectedContent, content.TextContent);
    }

    [Fact]
    public void AppliesDefaultClasses()
    {
        // Arrange & Act
        var cut = Render<SbButton>(parameters => parameters
            .AddChildContent("Click"));

        // Assert
        var button = cut.Find("button");
        Assert.Contains("sb-button", button.ClassList);
        Assert.Contains("sb-button--solid", button.ClassList);
        Assert.Contains("sb-button--md", button.ClassList);
        Assert.Contains("sb-button--primary", button.ClassList);
    }

    [Fact]
    public void RendersAsAnchorWhenHrefIsSet()
    {
        // Arrange & Act
        var cut = Render<SbButton>(parameters => parameters
            .Add(p => p.Href, "https://example.com")
            .AddChildContent("Link"));

        // Assert
        var anchor = cut.Find("a");
        Assert.NotNull(anchor);
        Assert.Equal("https://example.com", anchor.GetAttribute("href"));
        Assert.Empty(cut.FindAll("button"));
    }

    [Fact]
    public void AppliesDisabledState()
    {
        // Arrange & Act
        var cut = Render<SbButton>(parameters => parameters
            .Add(p => p.Disabled, true)
            .AddChildContent("Disabled"));

        // Assert
        var button = cut.Find("button");
        Assert.True(button.HasAttribute("disabled"));
        Assert.Contains("sb-button--disabled", button.ClassList);
    }

    [Fact]
    public void AppliesLoadingState()
    {
        // Arrange & Act
        var cut = Render<SbButton>(parameters => parameters
            .Add(p => p.Loading, true)
            .AddChildContent("Loading"));

        // Assert
        var button = cut.Find("button");
        Assert.True(button.HasAttribute("disabled"));
        Assert.Equal("true", button.GetAttribute("aria-busy"));
        Assert.Contains("sb-button--loading", button.ClassList);
        Assert.NotNull(cut.Find(".sb-button__spinner"));
    }

    [Fact]
    public void AppliesFullWidth()
    {
        // Arrange & Act
        var cut = Render<SbButton>(parameters => parameters
            .Add(p => p.FullWidth, true)
            .AddChildContent("Full Width"));

        // Assert
        var button = cut.Find("button");
        Assert.Contains("sb-button--full-width", button.ClassList);
    }

    [Fact]
    public void AppliesCustomClass()
    {
        // Arrange & Act
        var cut = Render<SbButton>(parameters => parameters
            .Add(p => p.Class, "my-custom-class")
            .AddChildContent("Click"));

        // Assert
        var button = cut.Find("button");
        Assert.Contains("my-custom-class", button.ClassList);
    }

    [Fact]
    public void AppliesVariant()
    {
        // Arrange & Act
        var cut = Render<SbButton>(parameters => parameters
            .Add(p => p.Variant, SbButtonVariant.Outline)
            .AddChildContent("Outline"));

        // Assert
        var button = cut.Find("button");
        Assert.Contains("sb-button--outline", button.ClassList);
    }

    [Fact]
    public void AppliesSize()
    {
        // Arrange & Act
        var cut = Render<SbButton>(parameters => parameters
            .Add(p => p.Size, SbSize.Sm)
            .AddChildContent("Small"));

        // Assert
        var button = cut.Find("button");
        Assert.Contains("sb-button--sm", button.ClassList);
    }

    [Fact]
    public void AppliesColor()
    {
        // Arrange & Act
        var cut = Render<SbButton>(parameters => parameters
            .Add(p => p.Color, SbColor.Danger)
            .AddChildContent("Danger"));

        // Assert
        var button = cut.Find("button");
        Assert.Contains("sb-button--danger", button.ClassList);
    }

    [Fact]
    public void DoesNotAddColorClassWhenDefault()
    {
        // Arrange & Act
        var cut = Render<SbButton>(parameters => parameters
            .Add(p => p.Color, SbColor.Default)
            .AddChildContent("Default"));

        // Assert
        var button = cut.Find("button");
        Assert.DoesNotContain("sb-button--default", button.ClassList);
    }

    [Fact]
    public void SetsTypeAttribute()
    {
        // Arrange & Act
        var cut = Render<SbButton>(parameters => parameters
            .Add(p => p.Type, "submit")
            .AddChildContent("Submit"));

        // Assert
        var button = cut.Find("button");
        Assert.Equal("submit", button.GetAttribute("type"));
    }

    [Fact]
    public void InvokesOnClickWhenClicked()
    {
        // Arrange
        var clicked = false;
        var cut = Render<SbButton>(parameters => parameters
            .Add(p => p.OnClick, args => { clicked = true; return Task.CompletedTask; })
            .AddChildContent("Click me"));

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
        var cut = Render<SbButton>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.OnClick, args => { clicked = true; return Task.CompletedTask; })
            .AddChildContent("Disabled"));

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
        var cut = Render<SbButton>(parameters => parameters
            .Add(p => p.Loading, true)
            .Add(p => p.OnClick, args => { clicked = true; return Task.CompletedTask; })
            .AddChildContent("Loading"));

        // Act
        cut.Find("button").Click();

        // Assert
        Assert.False(clicked);
    }
}
