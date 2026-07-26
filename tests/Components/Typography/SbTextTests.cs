using Microsoft.AspNetCore.Components;
using Bunit;
using SufiChain.SufiBlazor.Components;
using SufiChain.SufiBlazor.Components.Typography;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Typography;

public class SbTextTests : BunitContext
{
    private IRenderedComponent<SbText> RenderText(
        Action<ComponentParameterCollectionBuilder<SbText>>? configure = null)
    {
        return Render<SbText>(p =>
        {
            p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "Text content")));
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void DefaultRendersParagraph()
    {
        // Arrange & Act
        var cut = RenderText();

        // Assert
        var p = cut.Find("p");
        Assert.NotNull(p);
        Assert.Equal("p", p.TagName.ToLowerInvariant());
    }

    [Fact]
    public void RendersChildContent()
    {
        // Arrange & Act
        var cut = RenderText();

        // Assert
        Assert.Contains("Text content", cut.Markup);
    }

    [Fact]
    public void HasBaseTextClass()
    {
        // Arrange & Act
        var cut = RenderText();

        // Assert
        var element = cut.Find("p");
        Assert.Contains("sb-text", element.ClassList);
    }

    [Theory]
    [InlineData(SbTextVariant.Body, "body")]
    [InlineData(SbTextVariant.BodySmall, "bodysmall")]
    [InlineData(SbTextVariant.Lead, "lead")]
    [InlineData(SbTextVariant.Caption, "caption")]
    [InlineData(SbTextVariant.Overline, "overline")]
    [InlineData(SbTextVariant.Code, "code")]
    public void AppliesVariantClass(SbTextVariant variant, string expectedSuffix)
    {
        // Arrange & Act
        var cut = RenderText(p => p.Add(x => x.Variant, variant));

        // Assert
        var element = cut.Find(".sb-text");
        Assert.Contains($"sb-text--{expectedSuffix}", element.ClassList);
    }

    [Theory]
    [InlineData(SbTextVariant.Body, "p")]
    [InlineData(SbTextVariant.BodySmall, "p")]
    [InlineData(SbTextVariant.Lead, "p")]
    [InlineData(SbTextVariant.Caption, "span")]
    [InlineData(SbTextVariant.Overline, "span")]
    [InlineData(SbTextVariant.Code, "code")]
    public void RendersDefaultTagForVariant(SbTextVariant variant, string expectedTag)
    {
        // Arrange & Act
        var cut = RenderText(p => p.Add(x => x.Variant, variant));

        // Assert
        var element = cut.Find(expectedTag);
        Assert.NotNull(element);
        Assert.Equal(expectedTag, element.TagName.ToLowerInvariant());
    }

    [Theory]
    [InlineData("div", "div")]
    [InlineData("span", "span")]
    [InlineData("label", "label")]
    [InlineData("small", "small")]
    [InlineData("code", "code")]
    public void TagOverrideRendersCorrectElement(string tag, string expectedTag)
    {
        // Arrange & Act
        var cut = RenderText(p => p.Add(x => x.Tag, tag));

        // Assert
        var element = cut.Find(expectedTag);
        Assert.NotNull(element);
        Assert.Equal(expectedTag, element.TagName.ToLowerInvariant());
    }

    [Theory]
    [InlineData(SbSize.Xs, "xs")]
    [InlineData(SbSize.Sm, "sm")]
    [InlineData(SbSize.Lg, "lg")]
    public void AppliesSizeClass(SbSize size, string expectedSuffix)
    {
        // Arrange & Act
        var cut = RenderText(p => p.Add(x => x.Size, size));

        // Assert
        var element = cut.Find(".sb-text");
        Assert.Contains($"sb-text--size-{expectedSuffix}", element.ClassList);
    }

    [Theory]
    [InlineData(SbColor.Primary, "primary")]
    [InlineData(SbColor.Danger, "danger")]
    [InlineData(SbColor.Muted, "muted")]
    public void AppliesColorClass(SbColor color, string expectedSuffix)
    {
        // Arrange & Act
        var cut = RenderText(p => p.Add(x => x.Color, color));

        // Assert
        var element = cut.Find(".sb-text");
        Assert.Contains($"sb-text--{expectedSuffix}", element.ClassList);
    }

    [Theory]
    [InlineData(SbFontWeight.Normal, "normal")]
    [InlineData(SbFontWeight.Medium, "medium")]
    [InlineData(SbFontWeight.Bold, "bold")]
    public void AppliesWeightClass(SbFontWeight weight, string expectedSuffix)
    {
        // Arrange & Act
        var cut = RenderText(p => p.Add(x => x.Weight, weight));

        // Assert
        var element = cut.Find(".sb-text");
        Assert.Contains($"sb-text--weight-{expectedSuffix}", element.ClassList);
    }

    [Theory]
    [InlineData(SbTextAlign.Center, "center")]
    [InlineData(SbTextAlign.End, "end")]
    [InlineData(SbTextAlign.Justify, "justify")]
    public void AppliesAlignClass(SbTextAlign align, string expectedSuffix)
    {
        // Arrange & Act
        var cut = RenderText(p => p.Add(x => x.Align, align));

        // Assert
        var element = cut.Find(".sb-text");
        Assert.Contains($"sb-text--align-{expectedSuffix}", element.ClassList);
    }

    [Fact]
    public void AppliesTruncateClass()
    {
        // Arrange & Act
        var cut = RenderText(p => p.Add(x => x.Truncate, true));

        // Assert
        var element = cut.Find(".sb-text");
        Assert.Contains("sb-text--truncate", element.ClassList);
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderText(p => p.Add(x => x.Class, "my-text"));

        // Assert
        var element = cut.Find(".sb-text");
        Assert.Contains("my-text", element.ClassList);
    }

    [Fact]
    public void AppliesStyleParameter()
    {
        // Arrange & Act
        var cut = RenderText(p => p.Add(x => x.Style, "line-height: 1.5"));

        // Assert
        var element = cut.Find(".sb-text");
        Assert.Contains("line-height: 1.5", element.GetAttribute("style") ?? "");
    }

    [Fact]
    public void PassesAdditionalAttributes()
    {
        // Arrange & Act
        var cut = RenderText(p => p
            .Add(x => x.AdditionalAttributes, new Dictionary<string, object> { ["data-test"] = "text" }));

        // Assert
        var element = cut.Find(".sb-text");
        Assert.Equal("text", element.GetAttribute("data-test"));
    }
}
