using Microsoft.AspNetCore.Components;
using Bunit;
using SufiChain.SufiBlazor.Components.Feedback;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Feedback;

public class SbAlertTests : BunitContext
{
    private IRenderedComponent<SbAlert> RenderAlert(
        Action<ComponentParameterCollectionBuilder<SbAlert>>? configure = null)
    {
        return Render<SbAlert>(p =>
        {
            p.AddChildContent("<span>Alert message</span>");
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersAlertStructure()
    {
        // Arrange & Act
        var cut = RenderAlert();

        // Assert
        var alert = cut.Find(".sb-alert");
        Assert.NotNull(alert);
        Assert.NotNull(cut.Find(".sb-alert__icon"));
        Assert.NotNull(cut.Find(".sb-alert__content"));
        Assert.NotNull(cut.Find(".sb-alert__message"));
    }

    [Fact]
    public void RendersChildContent()
    {
        // Arrange & Act
        var cut = RenderAlert();

        // Assert
        Assert.Contains("Alert message", cut.Markup);
    }

    [Fact]
    public void RendersTitleWhenProvided()
    {
        // Arrange & Act
        var cut = RenderAlert(p => p.Add(x => x.Title, "Heads up"));

        // Assert
        var title = cut.Find(".sb-alert__title");
        Assert.NotNull(title);
        Assert.Contains("Heads up", title.TextContent);
    }

    [Fact]
    public void DoesNotRenderTitleWhenEmpty()
    {
        // Arrange & Act
        var cut = RenderAlert();

        // Assert
        Assert.Empty(cut.FindAll(".sb-alert__title"));
    }

    [Fact]
    public void AppliesInfoSeverityByDefault()
    {
        // Arrange & Act
        var cut = RenderAlert();

        // Assert
        var alert = cut.Find(".sb-alert");
        Assert.Contains("sb-alert--info", alert.ClassList);
        Assert.Contains("ℹ", cut.Markup);
    }

    [Fact]
    public void AppliesSuccessSeverity()
    {
        // Arrange & Act
        var cut = RenderAlert(p => p.Add(x => x.Severity, SbAlertSeverity.Success));

        // Assert
        var alert = cut.Find(".sb-alert");
        Assert.Contains("sb-alert--success", alert.ClassList);
        Assert.Contains("✓", cut.Markup);
    }

    [Fact]
    public void AppliesWarningSeverity()
    {
        // Arrange & Act
        var cut = RenderAlert(p => p.Add(x => x.Severity, SbAlertSeverity.Warning));

        // Assert
        var alert = cut.Find(".sb-alert");
        Assert.Contains("sb-alert--warning", alert.ClassList);
        Assert.Contains("⚠", cut.Markup);
    }

    [Fact]
    public void AppliesDangerSeverity()
    {
        // Arrange & Act
        var cut = RenderAlert(p => p.Add(x => x.Severity, SbAlertSeverity.Danger));

        // Assert
        var alert = cut.Find(".sb-alert");
        Assert.Contains("sb-alert--danger", alert.ClassList);
        Assert.Contains("✕", cut.Markup);
    }

    [Fact]
    public void HasRoleAlertWhenDanger()
    {
        // Arrange & Act
        var cut = RenderAlert(p => p.Add(x => x.Severity, SbAlertSeverity.Danger));

        // Assert
        var alert = cut.Find(".sb-alert");
        Assert.Equal("alert", alert.GetAttribute("role"));
    }

    [Fact]
    public void HasRoleStatusWhenNotDanger()
    {
        // Arrange & Act
        var cut = RenderAlert(p => p.Add(x => x.Severity, SbAlertSeverity.Info));

        // Assert
        var alert = cut.Find(".sb-alert");
        Assert.Equal("status", alert.GetAttribute("role"));
    }

    [Fact]
    public void RendersDismissButtonWhenDismissible()
    {
        // Arrange & Act
        var cut = RenderAlert(p => p.Add(x => x.Dismissible, true));

        // Assert
        var dismissBtn = cut.Find(".sb-alert__dismiss");
        Assert.NotNull(dismissBtn);
        Assert.Equal("button", dismissBtn.TagName.ToLowerInvariant());
        Assert.Equal("Dismiss", dismissBtn.GetAttribute("aria-label"));
        Assert.Contains("×", dismissBtn.TextContent);
    }

    [Fact]
    public void DoesNotRenderDismissButtonWhenNotDismissible()
    {
        // Arrange & Act
        var cut = RenderAlert();

        // Assert
        Assert.Empty(cut.FindAll(".sb-alert__dismiss"));
    }

    [Fact]
    public async Task InvokesOnDismissWhenDismissButtonClicked()
    {
        // Arrange
        var dismissed = false;

        var cut = RenderAlert(p => p
            .Add(x => x.Dismissible, true)
            .Add(x => x.OnDismiss, EventCallback.Factory.Create(this, () => dismissed = true)));

        var dismissBtn = cut.Find(".sb-alert__dismiss");

        // Act
        await cut.InvokeAsync(() => dismissBtn!.Click());

        cut.WaitForState(() => dismissed);

        // Assert
        Assert.True(dismissed);
    }

    [Fact]
    public void AppliesDenseClass()
    {
        // Arrange & Act
        var cut = RenderAlert(p => p.Add(x => x.Dense, true));

        // Assert
        var alert = cut.Find(".sb-alert");
        Assert.Contains("sb-alert--dense", alert.ClassList);
    }

    [Fact]
    public void AppliesCustomClass()
    {
        // Arrange & Act
        var cut = RenderAlert(p => p.Add(x => x.Class, "my-alert"));

        // Assert
        var alert = cut.Find(".sb-alert");
        Assert.Contains("my-alert", alert.ClassList);
    }

    [Fact]
    public void AppliesInlineStyle()
    {
        // Arrange & Act
        var cut = RenderAlert(p => p.Add(x => x.Style, "margin-top: 10px;"));

        // Assert
        var alert = cut.Find(".sb-alert");
        Assert.Contains("margin-top: 10px", alert.GetAttribute("style"));
    }
}
