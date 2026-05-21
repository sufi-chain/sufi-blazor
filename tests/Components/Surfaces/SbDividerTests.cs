using Microsoft.AspNetCore.Components;
using Bunit;
using SufiChain.SufiBlazor.Components;
using SufiChain.SufiBlazor.Components.Layout;
using SufiChain.SufiBlazor.Components.Surfaces;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Surfaces;

public class SbDividerTests : BunitContext
{
    private IRenderedComponent<SbDivider> RenderDivider(
        Action<ComponentParameterCollectionBuilder<SbDivider>>? configure = null)
    {
        return Render<SbDivider>(p => configure?.Invoke(p));
    }

    [Fact]
    public void RendersAsHrWhenNoContent()
    {
        // Arrange & Act
        var cut = RenderDivider();

        // Assert
        var hr = cut.Find("hr.sb-divider");
        Assert.NotNull(hr);
    }

    [Fact]
    public void RendersAsDivWithContentWhenChildContentProvided()
    {
        // Arrange & Act
        var cut = RenderDivider(p => p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "OR"))));

        // Assert
        Assert.Empty(cut.FindAll("hr"));
        var div = cut.Find("div.sb-divider");
        Assert.NotNull(div);
        Assert.Contains("OR", cut.Markup);
    }

    [Fact]
    public void HasSeparatorRole()
    {
        // Arrange & Act
        var cut = RenderDivider();

        // Assert
        var hr = cut.Find("hr.sb-divider");
        Assert.Equal("separator", hr.GetAttribute("role"));
    }

    [Fact]
    public void HasWithLabelRoleWhenContentProvided()
    {
        var cut = RenderDivider(p => p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "Label"))));
        var div = cut.Find("div.sb-divider");
        Assert.Equal("separator", div.GetAttribute("role"));
    }

    [Theory]
    [InlineData(SbOrientation.Horizontal, "horizontal")]
    [InlineData(SbOrientation.Vertical, "vertical")]
    public void AppliesOrientationClass(SbOrientation orientation, string expectedClass)
    {
        // Arrange & Act
        var cut = RenderDivider(p => p.Add(x => x.Orientation, orientation));

        // Assert
        var element = cut.Find(".sb-divider");
        Assert.Contains($"sb-divider--{expectedClass}", element.ClassList);
    }

    [Fact]
    public void AppliesWithLabelAndLabelCenterWhenContentProvided()
    {
        // Arrange & Act
        var cut = RenderDivider(p => p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "OR"))));

        // Assert
        var div = cut.Find("div.sb-divider");
        Assert.Contains("sb-divider--with-label", div.ClassList);
        Assert.Contains("sb-divider--label-center", div.ClassList);
    }

    [Theory]
    [InlineData(SbAlign.Start, "start")]
    [InlineData(SbAlign.Center, "center")]
    [InlineData(SbAlign.End, "end")]
    public void AppliesLabelAlignClass(SbAlign align, string expectedClass)
    {
        // Arrange & Act
        var cut = RenderDivider(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "Label")))
            .Add(x => x.LabelAlign, align));

        // Assert
        var div = cut.Find("div.sb-divider");
        Assert.Contains($"sb-divider--label-{expectedClass}", div.ClassList);
    }

    [Fact]
    public void RendersLabelInContentSpan()
    {
        // Arrange & Act
        var cut = RenderDivider(p => p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "Custom Label"))));

        // Assert
        var content = cut.Find(".sb-divider__content");
        Assert.NotNull(content);
        Assert.Equal("Custom Label", content.TextContent.Trim());
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderDivider(p => p.Add(x => x.Class, "my-divider"));

        // Assert
        var element = cut.Find(".sb-divider");
        Assert.Contains("my-divider", element.ClassList);
    }

    [Fact]
    public void AppliesStyleParameter()
    {
        // Arrange & Act
        var cut = RenderDivider(p => p.Add(x => x.Style, "margin: 16px"));

        // Assert
        var element = cut.Find(".sb-divider");
        Assert.Contains("margin: 16px", element.GetAttribute("style") ?? "");
    }

    [Fact]
    public void PassesAdditionalAttributes()
    {
        // Arrange & Act
        var cut = RenderDivider(p => p
            .Add(x => x.AdditionalAttributes, new Dictionary<string, object> { ["data-test"] = "divider" }));

        // Assert
        var element = cut.Find(".sb-divider");
        Assert.Equal("divider", element.GetAttribute("data-test"));
    }
}
