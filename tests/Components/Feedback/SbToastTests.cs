using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Bunit;
using SufiChain.SufiBlazor.Components.Feedback;
using SufiChain.SufiBlazor.Localization;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Feedback;

/// <summary>
/// Stub localizer that returns the key as the value (for testing).
/// </summary>
file class StubStringLocalizer : IStringLocalizer<SufiBlazorResource>
{
    public LocalizedString this[string name] => new(name, name);
    public LocalizedString this[string name, params object[] arguments] => new(name, string.Format(CultureInfo.InvariantCulture, name, arguments));
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => Array.Empty<LocalizedString>();
}

public class SbToastTests : BunitContext
{
    public SbToastTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new StubStringLocalizer());
    }

    private IRenderedComponent<SbToast> RenderToast(
        Action<ComponentParameterCollectionBuilder<SbToast>>? configure = null)
    {
        return Render<SbToast>(p =>
        {
            p.Add(x => x.Message, "Toast message");
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersToastStructure()
    {
        // Arrange & Act
        var cut = RenderToast();

        // Assert
        var toast = cut.Find(".sb-toast");
        Assert.NotNull(toast);
        Assert.Equal("alert", toast.GetAttribute("role"));
        Assert.Equal("polite", toast.GetAttribute("aria-live"));
        Assert.NotNull(cut.Find(".sb-toast__icon"));
        Assert.NotNull(cut.Find(".sb-toast__content"));
        Assert.NotNull(cut.Find(".sb-toast__message"));
    }

    [Fact]
    public void RendersMessage()
    {
        // Arrange & Act
        var cut = RenderToast();

        // Assert
        Assert.Contains("Toast message", cut.Markup);
    }

    [Fact]
    public void RendersChildContentOverMessage()
    {
        // Arrange & Act
        var cut = Render<SbToast>(p => p
            .Add(x => x.Message, "Fallback")
            .AddChildContent("<span class=\"custom-msg\">Custom content</span>"));

        // Assert - ChildContent takes precedence
        Assert.Contains("Custom content", cut.Markup);
    }

    [Fact]
    public void RendersTitleWhenProvided()
    {
        // Arrange & Act
        var cut = RenderToast(p => p.Add(x => x.Title, "Success"));

        // Assert
        var title = cut.Find(".sb-toast__title");
        Assert.NotNull(title);
        Assert.Contains("Success", title.TextContent);
    }

    [Fact]
    public void DoesNotRenderTitleWhenEmpty()
    {
        // Arrange & Act
        var cut = RenderToast();

        // Assert
        Assert.Empty(cut.FindAll(".sb-toast__title"));
    }

    [Fact]
    public void AppliesInfoSeverityByDefault()
    {
        // Arrange & Act
        var cut = RenderToast();

        // Assert
        var toast = cut.Find(".sb-toast");
        Assert.Contains("sb-toast--info", toast.ClassList);
    }

    [Fact]
    public void AppliesSuccessSeverity()
    {
        // Arrange & Act
        var cut = RenderToast(p => p.Add(x => x.Severity, SbToastSeverity.Success));

        // Assert
        var toast = cut.Find(".sb-toast");
        Assert.Contains("sb-toast--success", toast.ClassList);
    }

    [Fact]
    public void AppliesWarningSeverity()
    {
        // Arrange & Act
        var cut = RenderToast(p => p.Add(x => x.Severity, SbToastSeverity.Warning));

        // Assert
        var toast = cut.Find(".sb-toast");
        Assert.Contains("sb-toast--warning", toast.ClassList);
    }

    [Fact]
    public void AppliesErrorSeverity()
    {
        // Arrange & Act
        var cut = RenderToast(p => p.Add(x => x.Severity, SbToastSeverity.Error));

        // Assert
        var toast = cut.Find(".sb-toast");
        Assert.Contains("sb-toast--error", toast.ClassList);
    }

    [Fact]
    public void RendersCloseButtonByDefault()
    {
        // Arrange & Act
        var cut = RenderToast();

        // Assert
        var closeBtn = cut.Find(".sb-toast__close");
        Assert.NotNull(closeBtn);
        Assert.Equal("Close", closeBtn.GetAttribute("aria-label"));
    }

    [Fact]
    public void DoesNotRenderCloseButtonWhenShowCloseButtonFalse()
    {
        // Arrange & Act
        var cut = RenderToast(p => p.Add(x => x.ShowCloseButton, false));

        // Assert
        Assert.Empty(cut.FindAll(".sb-toast__close"));
    }

    [Fact]
    public async Task InvokesOnCloseWhenCloseButtonClicked()
    {
        // Arrange
        var closed = false;
        var cut = RenderToast(p => p.Add(x => x.OnClose, EventCallback.Factory.Create(this, () => closed = true)));

        var closeBtn = cut.Find(".sb-toast__close");

        // Act
        await cut.InvokeAsync(() => closeBtn!.Click());

        cut.WaitForState(() => closed);

        // Assert
        Assert.True(closed);
    }

    [Fact]
    public void AppliesCustomClass()
    {
        // Arrange & Act
        var cut = RenderToast(p => p.Add(x => x.Class, "my-toast"));

        // Assert
        var toast = cut.Find(".sb-toast");
        Assert.Contains("my-toast", toast.ClassList);
    }
}
