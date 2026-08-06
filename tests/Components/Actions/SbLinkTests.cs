using Bunit;
using Microsoft.AspNetCore.Components.Web;
using SufiChain.SufiBlazor.Components;
using SufiChain.SufiBlazor.Components.Actions;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Actions;

public class SbLinkTests : BunitContext
{
    [Fact]
    public void RendersAsAnchor()
    {
        // Arrange & Act
        var cut = Render<SbLink>(parameters => parameters
            .Add(p => p.Href, "https://example.com")
            .AddChildContent("Example Link"));

        // Assert
        var anchor = cut.Find("a");
        Assert.NotNull(anchor);
        Assert.Equal("https://example.com", anchor.GetAttribute("href"));
    }

    [Fact]
    public void RendersChildContent()
    {
        // Arrange
        const string expectedContent = "Click here";

        // Act
        var cut = Render<SbLink>(parameters => parameters
            .Add(p => p.Href, "#")
            .AddChildContent(expectedContent));

        // Assert
        Assert.Contains(expectedContent, cut.Markup);
    }

    [Fact]
    public void AppliesDefaultClasses()
    {
        // Arrange & Act
        var cut = Render<SbLink>(parameters => parameters
            .Add(p => p.Href, "#")
            .AddChildContent("Link"));

        // Assert
        var anchor = cut.Find("a");
        Assert.Contains("sb-link", anchor.ClassList);
        Assert.Contains("sb-link--primary", anchor.ClassList);
        Assert.Contains("sb-link--underline-hover", anchor.ClassList);
    }

    [Fact]
    public void SetsTargetAttribute()
    {
        // Arrange & Act
        var cut = Render<SbLink>(parameters => parameters
            .Add(p => p.Href, "https://example.com")
            .Add(p => p.Target, "_blank")
            .AddChildContent("External"));

        // Assert
        var anchor = cut.Find("a");
        Assert.Equal("_blank", anchor.GetAttribute("target"));
    }

    [Fact]
    public void SetsRelToNoopenerNoreferrerWhenTargetBlank()
    {
        // Arrange & Act
        var cut = Render<SbLink>(parameters => parameters
            .Add(p => p.Href, "https://example.com")
            .Add(p => p.Target, "_blank")
            .AddChildContent("External"));

        // Assert
        var anchor = cut.Find("a");
        Assert.Equal("noopener noreferrer", anchor.GetAttribute("rel"));
    }

    [Fact]
    public void UsesExplicitRelWhenSet()
    {
        // Arrange & Act
        var cut = Render<SbLink>(parameters => parameters
            .Add(p => p.Href, "#")
            .Add(p => p.Rel, "nofollow")
            .AddChildContent("Link"));

        // Assert
        var anchor = cut.Find("a");
        Assert.Equal("nofollow", anchor.GetAttribute("rel"));
    }

    [Fact]
    public void AppliesDisabledState()
    {
        // Arrange & Act
        var cut = Render<SbLink>(parameters => parameters
            .Add(p => p.Href, "https://example.com")
            .Add(p => p.Disabled, true)
            .AddChildContent("Disabled"));

        // Assert
        var anchor = cut.Find("a");
        Assert.Equal("true", anchor.GetAttribute("aria-disabled"));
        Assert.Equal("-1", anchor.GetAttribute("tabindex"));
        Assert.Null(anchor.GetAttribute("href"));
        Assert.Contains("sb-link--disabled", anchor.ClassList);
    }

    [Fact]
    public void AppliesExternalState()
    {
        // Arrange & Act
        var cut = Render<SbLink>(parameters => parameters
            .Add(p => p.Href, "https://external.com")
            .Add(p => p.External, true)
            .AddChildContent("External Link"));

        // Assert
        var anchor = cut.Find("a");
        Assert.Contains("sb-link--external", anchor.ClassList);
        Assert.NotNull(cut.Find(".sb-link__external-icon"));
        Assert.Equal("noopener noreferrer", anchor.GetAttribute("rel"));
    }

    [Fact]
    public void AppliesCustomClass()
    {
        // Arrange & Act
        var cut = Render<SbLink>(parameters => parameters
            .Add(p => p.Href, "#")
            .Add(p => p.Class, "my-custom-class")
            .AddChildContent("Link"));

        // Assert
        var anchor = cut.Find("a");
        Assert.Contains("my-custom-class", anchor.ClassList);
    }

    [Fact]
    public void AppliesColor()
    {
        // Arrange & Act
        var cut = Render<SbLink>(parameters => parameters
            .Add(p => p.Href, "#")
            .Add(p => p.Color, SbColor.Secondary)
            .AddChildContent("Link"));

        // Assert
        var anchor = cut.Find("a");
        Assert.Contains("sb-link--secondary", anchor.ClassList);
    }

    [Fact]
    public void AppliesUnderlineAlways()
    {
        // Arrange & Act
        var cut = Render<SbLink>(parameters => parameters
            .Add(p => p.Href, "#")
            .Add(p => p.Underline, SbLinkUnderline.Always)
            .AddChildContent("Link"));

        // Assert
        var anchor = cut.Find("a");
        Assert.Contains("sb-link--underline-always", anchor.ClassList);
    }

    [Fact]
    public void AppliesUnderlineNone()
    {
        // Arrange & Act
        var cut = Render<SbLink>(parameters => parameters
            .Add(p => p.Href, "#")
            .Add(p => p.Underline, SbLinkUnderline.None)
            .AddChildContent("Link"));

        // Assert
        var anchor = cut.Find("a");
        Assert.Contains("sb-link--underline-none", anchor.ClassList);
    }

    [Fact]
    public void InvokesOnClickWhenClicked()
    {
        // Arrange
        var clicked = false;
        var cut = Render<SbLink>(parameters => parameters
            .Add(p => p.Href, "#")
            .Add(p => p.OnClick, _ => { clicked = true; return Task.CompletedTask; })
            .AddChildContent("Link"));

        // Act
        cut.Find("a").Click();

        // Assert
        Assert.True(clicked);
    }

    [Fact]
    public void DoesNotInvokeOnClickWhenDisabled()
    {
        // Arrange
        var clicked = false;
        var cut = Render<SbLink>(parameters => parameters
            .Add(p => p.Href, "https://example.com")
            .Add(p => p.Disabled, true)
            .Add(p => p.OnClick, _ => { clicked = true; return Task.CompletedTask; })
            .AddChildContent("Disabled"));

        // Act
        cut.Find("a").Click();

        // Assert
        Assert.False(clicked);
    }
}
