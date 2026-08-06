using Bunit;
using SufiChain.SufiBlazor.Theming;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Theming;

public class SbThemeProviderTests : BunitContext
{
    [Fact]
    public void RendersWithDefaultLightTheme()
    {
        // Arrange & Act
        var cut = Render<SbThemeProvider>(parameters => parameters
            .AddChildContent("<p>Test Content</p>"));

        // Assert
        var rootElement = cut.Find(".sb-theme-root");
        Assert.NotNull(rootElement);
        Assert.Contains("sb-theme-light", rootElement.ClassList);
        Assert.DoesNotContain("sb-theme-dark", rootElement.ClassList);
    }

    [Fact]
    public void RendersWithDarkThemeWhenIsDarkIsTrue()
    {
        // Arrange & Act
        var cut = Render<SbThemeProvider>(parameters => parameters
            .Add(p => p.IsDark, true)
            .AddChildContent("<p>Test Content</p>"));

        // Assert
        var rootElement = cut.Find(".sb-theme-root");
        Assert.Contains("sb-theme-dark", rootElement.ClassList);
        Assert.DoesNotContain("sb-theme-light", rootElement.ClassList);
    }

    [Fact]
    public void RendersWithLtrDirectionByDefault()
    {
        // Arrange & Act
        var cut = Render<SbThemeProvider>(parameters => parameters
            .AddChildContent("<p>Test Content</p>"));

        // Assert
        var rootElement = cut.Find(".sb-theme-root");
        Assert.Equal("ltr", rootElement.GetAttribute("dir"));
        Assert.DoesNotContain("sb-rtl", rootElement.ClassList);
    }

    [Fact]
    public void RendersWithRtlDirectionWhenDirectionIsRtl()
    {
        // Arrange & Act
        var cut = Render<SbThemeProvider>(parameters => parameters
            .Add(p => p.Direction, SbDirection.Rtl)
            .AddChildContent("<p>Test Content</p>"));

        // Assert
        var rootElement = cut.Find(".sb-theme-root");
        Assert.Equal("rtl", rootElement.GetAttribute("dir"));
        Assert.Contains("sb-rtl", rootElement.ClassList);
    }

    [Fact]
    public void RendersChildContent()
    {
        // Arrange
        const string expectedContent = "Hello SufiBlazor";

        // Act
        var cut = Render<SbThemeProvider>(parameters => parameters
            .AddChildContent($"<p>{expectedContent}</p>"));

        // Assert
        cut.MarkupMatches($"""<div class="sb-theme-root sb-theme-light" dir="ltr"><p>{expectedContent}</p></div>""");
    }

    [Fact]
    public void AppliesCustomClass()
    {
        // Arrange & Act
        var cut = Render<SbThemeProvider>(parameters => parameters
            .Add(p => p.Class, "my-custom-class")
            .AddChildContent("<p>Test</p>"));

        // Assert
        var rootElement = cut.Find(".sb-theme-root");
        Assert.Contains("my-custom-class", rootElement.ClassList);
    }

    [Fact]
    public void CascadesThemeContext()
    {
        // Arrange
        SbThemeContext? capturedContext = null;
        Action<SbThemeContext> onCaptured = ctx => capturedContext = ctx;

        // Act
        var cut = Render<SbThemeProvider>(parameters => parameters
            .Add(p => p.IsDark, true)
            .Add(p => p.Direction, SbDirection.Rtl)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<CascadeContextCapture>(0);
                builder.AddAttribute(1, "OnContextCaptured", onCaptured);
                builder.CloseComponent();
            }));

        // Assert
        Assert.NotNull(capturedContext);
        Assert.True(capturedContext!.IsDark);
        Assert.Equal(SbDirection.Rtl, capturedContext.Direction);
    }

    /// <summary>
    /// Helper component to capture cascaded theme context.
    /// </summary>
    private sealed class CascadeContextCapture : Microsoft.AspNetCore.Components.ComponentBase
    {
        [Microsoft.AspNetCore.Components.CascadingParameter]
        public SbThemeContext? ThemeContext { get; set; }

        [Microsoft.AspNetCore.Components.Parameter]
        public Action<SbThemeContext>? OnContextCaptured { get; set; }

        protected override void OnParametersSet()
        {
            if (ThemeContext != null)
            {
                OnContextCaptured?.Invoke(ThemeContext);
            }
        }
    }
}
