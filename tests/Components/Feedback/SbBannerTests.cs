using Microsoft.AspNetCore.Components;
using Bunit;
using SufiChain.SufiBlazor.Components.Feedback;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Feedback;

public class SbBannerTests : BunitContext
{
    private IRenderedComponent<SbBanner> RenderBanner(
        Action<ComponentParameterCollectionBuilder<SbBanner>>? configure = null)
    {
        return Render<SbBanner>(p =>
        {
            p.Add(x => x.Message, "Banner message");
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersBannerStructure()
    {
        // Arrange & Act
        var cut = RenderBanner();

        // Assert
        var banner = cut.Find(".sb-banner");
        Assert.NotNull(banner);
        Assert.Equal("alert", banner.GetAttribute("role"));
        Assert.NotNull(cut.Find(".sb-banner__header"));
        Assert.NotNull(cut.Find(".sb-banner__content"));
        Assert.NotNull(cut.Find(".sb-banner__icon"));
    }

    [Fact]
    public void RendersMessageWhenNoChildContent()
    {
        // Arrange & Act
        var cut = RenderBanner();

        // Assert
        Assert.Contains("Banner message", cut.Markup);
        var message = cut.Find(".sb-banner__message");
        Assert.NotNull(message);
    }

    [Fact]
    public void RendersChildContentOverMessage()
    {
        // Arrange & Act
        var cut = Render<SbBanner>(p => p
            .Add(x => x.Message, "Fallback")
            .AddChildContent("<span class=\"custom-content\">Custom content</span>"));

        // Assert - ChildContent takes precedence
        Assert.Contains("Custom content", cut.Markup);
        Assert.Empty(cut.FindAll(".sb-banner__message"));
    }

    [Fact]
    public void RendersTitleWhenProvided()
    {
        // Arrange & Act
        var cut = RenderBanner(p => p.Add(x => x.Title, "Important Notice"));

        // Assert
        var title = cut.Find(".sb-banner__title");
        Assert.NotNull(title);
        Assert.Contains("Important Notice", title.TextContent);
    }

    [Fact]
    public void DoesNotRenderTitleWhenEmpty()
    {
        // Arrange & Act
        var cut = RenderBanner();

        // Assert
        Assert.Empty(cut.FindAll(".sb-banner__title"));
    }

    [Fact]
    public void AppliesInfoSeverityByDefault()
    {
        // Arrange & Act
        var cut = RenderBanner();

        // Assert
        var banner = cut.Find(".sb-banner");
        Assert.Contains("sb-banner--info", banner.ClassList);
    }

    [Fact]
    public void AppliesSuccessSeverity()
    {
        // Arrange & Act
        var cut = RenderBanner(p => p.Add(x => x.Severity, SbBannerSeverity.Success));

        // Assert
        var banner = cut.Find(".sb-banner");
        Assert.Contains("sb-banner--success", banner.ClassList);
    }

    [Fact]
    public void AppliesWarningSeverity()
    {
        // Arrange & Act
        var cut = RenderBanner(p => p.Add(x => x.Severity, SbBannerSeverity.Warning));

        // Assert
        var banner = cut.Find(".sb-banner");
        Assert.Contains("sb-banner--warning", banner.ClassList);
    }

    [Fact]
    public void AppliesErrorSeverity()
    {
        // Arrange & Act
        var cut = RenderBanner(p => p.Add(x => x.Severity, SbBannerSeverity.Error));

        // Assert
        var banner = cut.Find(".sb-banner");
        Assert.Contains("sb-banner--error", banner.ClassList);
    }

    [Fact]
    public void RendersCloseButtonWhenDismissible()
    {
        // Arrange & Act
        var cut = RenderBanner(p => p.Add(x => x.Dismissible, true));

        // Assert
        var closeBtn = cut.Find(".sb-banner__close");
        Assert.NotNull(closeBtn);
        Assert.Equal("Dismiss", closeBtn.GetAttribute("aria-label"));
    }

    [Fact]
    public void DoesNotRenderCloseButtonWhenNotDismissible()
    {
        // Arrange & Act
        var cut = RenderBanner(p => p.Add(x => x.Dismissible, false));

        // Assert
        Assert.Empty(cut.FindAll(".sb-banner__close"));
    }

    [Fact]
    public async Task HidesBannerWhenDismissed()
    {
        // Arrange
        var cut = RenderBanner();

        Assert.NotNull(cut.Find(".sb-banner"));

        // Act
        var closeBtn = cut.Find(".sb-banner__close");
        await cut.InvokeAsync(() => closeBtn!.Click());

        cut.WaitForState(() => cut.FindAll(".sb-banner").Count == 0);

        // Assert
        Assert.Empty(cut.FindAll(".sb-banner"));
    }

    [Fact]
    public async Task InvokesOnDismissWhenDismissed()
    {
        // Arrange
        var dismissed = false;
        var cut = RenderBanner(p => p
            .Add(x => x.Dismissible, true)
            .Add(x => x.OnDismiss, EventCallback.Factory.Create(this, () => dismissed = true)));

        var closeBtn = cut.Find(".sb-banner__close");

        // Act
        await cut.InvokeAsync(() => closeBtn!.Click());

        cut.WaitForState(() => dismissed);

        // Assert
        Assert.True(dismissed);
    }

    [Fact]
    public async Task ShowRestoresVisibilityAfterDismiss()
    {
        // Arrange
        var cut = RenderBanner();
        var closeBtn = cut.Find(".sb-banner__close");
        await cut.InvokeAsync(() => closeBtn!.Click());
        cut.WaitForState(() => cut.FindAll(".sb-banner").Count == 0);

        // Act - Show() calls StateHasChanged, must run on Dispatcher
        await cut.InvokeAsync(() => cut.Instance.Show());

        cut.WaitForState(() => cut.FindAll(".sb-banner").Count > 0);

        // Assert
        Assert.NotNull(cut.Find(".sb-banner"));
    }

    [Fact]
    public void RendersActionsWhenProvided()
    {
        // Arrange & Act
        var cut = RenderBanner(p => p
            .Add(x => x.Actions, a => a.AddMarkupContent(0, "<button class=\"banner-action\">Learn more</button>")));

        // Assert
        var actions = cut.Find(".sb-banner__actions");
        Assert.NotNull(actions);
        var actionBtn = cut.Find(".banner-action");
        Assert.NotNull(actionBtn);
        Assert.Contains("Learn more", actionBtn.TextContent);
    }

    [Fact]
    public void DoesNotRenderActionsWhenNull()
    {
        // Arrange & Act
        var cut = RenderBanner();

        // Assert
        Assert.Empty(cut.FindAll(".sb-banner__actions"));
    }

    [Fact]
    public void AppliesCustomClass()
    {
        // Arrange & Act
        var cut = RenderBanner(p => p.Add(x => x.Class, "my-banner"));

        // Assert
        var banner = cut.Find(".sb-banner");
        Assert.Contains("my-banner", banner.ClassList);
    }
}
