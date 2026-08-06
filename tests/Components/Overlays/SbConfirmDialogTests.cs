using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Bunit;
using SufiChain.SufiBlazor.Components.Overlays;
using SufiChain.SufiBlazor.Localization;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Overlays;

file class StubStringLocalizer : IStringLocalizer<SufiBlazorResource>
{
    public LocalizedString this[string name] => new(name, name);
    public LocalizedString this[string name, params object[] arguments] => new(name, string.Format(CultureInfo.InvariantCulture, name, arguments));
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => Array.Empty<LocalizedString>();
}

public class SbConfirmDialogTests : BunitContext
{
    public SbConfirmDialogTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new StubStringLocalizer());
    }

    private IRenderedComponent<SbConfirmDialog> RenderConfirmDialog(
        Action<ComponentParameterCollectionBuilder<SbConfirmDialog>>? configure = null)
    {
        return Render<SbConfirmDialog>(p => configure?.Invoke(p));
    }

    [Fact]
    public void DoesNotRenderWhenClosed()
    {
        // Arrange & Act
        var cut = RenderConfirmDialog(p => p
            .Add(x => x.Title, "Confirm")
            .Add(x => x.Message, "Are you sure?"));

        // Assert
        Assert.Empty(cut.FindAll(".sb-confirm-dialog-overlay"));
    }

    [Fact]
    public async Task RendersWhenShowCalled()
    {
        // Arrange & Act
        var cut = RenderConfirmDialog(p => p
            .Add(x => x.Title, "Confirm")
            .Add(x => x.Message, "Proceed?"));
        await cut.InvokeAsync(() => cut.Instance.Show());
        cut.Render();

        // Assert
        var overlay = cut.Find(".sb-confirm-dialog-overlay");
        Assert.NotNull(overlay);
        var dialog = cut.Find(".sb-confirm-dialog");
        Assert.NotNull(dialog);
    }

    [Fact]
    public async Task RendersTitleAndMessage()
    {
        // Arrange & Act
        var cut = RenderConfirmDialog(p => p
            .Add(x => x.Title, "Delete Item")
            .Add(x => x.Message, "This cannot be undone."));
        await cut.InvokeAsync(() => cut.Instance.Show());
        cut.Render();

        // Assert
        Assert.Contains("Delete Item", cut.Markup);
        Assert.Contains("This cannot be undone.", cut.Markup);
        var title = cut.Find(".sb-confirm-dialog__title");
        Assert.NotNull(title);
        var message = cut.Find(".sb-confirm-dialog__message");
        Assert.NotNull(message);
    }

    [Fact]
    public async Task RendersChildContent()
    {
        // Arrange & Act
        var cut = RenderConfirmDialog(p => p
            .Add(x => x.Title, "Confirm")
            .AddChildContent(b => b.AddMarkupContent(0, "<p>Custom content</p>")));
        await cut.InvokeAsync(() => cut.Instance.Show());
        cut.Render();

        // Assert
        Assert.Contains("Custom content", cut.Markup);
        var body = cut.Find(".sb-confirm-dialog__body");
        Assert.NotNull(body);
    }

    [Fact]
    public async Task DoesNotRenderMessageWhenNull()
    {
        // Arrange & Act
        var cut = RenderConfirmDialog(p => p
            .Add(x => x.Title, "Title only"));
        await cut.InvokeAsync(() => cut.Instance.Show());
        cut.Render();

        // Assert
        Assert.Empty(cut.FindAll(".sb-confirm-dialog__message"));
    }

    [Fact]
    public async Task AppliesVariantClass()
    {
        // Arrange & Act
        var cut = RenderConfirmDialog(p => p
            .Add(x => x.Title, "Confirm")
            .Add(x => x.Variant, SbConfirmDialogVariant.Danger));
        await cut.InvokeAsync(() => cut.Instance.Show());
        cut.Render();

        // Assert
        var dialog = cut.Find(".sb-confirm-dialog");
        Assert.Contains("sb-confirm-dialog--danger", dialog.ClassList);
    }

    [Theory]
    [InlineData(SbConfirmDialogVariant.Default, "default")]
    [InlineData(SbConfirmDialogVariant.Danger, "danger")]
    [InlineData(SbConfirmDialogVariant.Warning, "warning")]
    [InlineData(SbConfirmDialogVariant.Info, "info")]
    public async Task AppliesCorrectVariantClass(SbConfirmDialogVariant variant, string expectedClass)
    {
        // Arrange & Act
        var cut = RenderConfirmDialog(p => p
            .Add(x => x.Title, "Confirm")
            .Add(x => x.Variant, variant));
        await cut.InvokeAsync(() => cut.Instance.Show());
        cut.Render();

        // Assert
        var dialog = cut.Find(".sb-confirm-dialog");
        Assert.Contains($"sb-confirm-dialog--{expectedClass}", dialog.ClassList);
    }

    [Fact]
    public async Task ShowsCancelButtonByDefault()
    {
        // Arrange & Act
        var cut = RenderConfirmDialog(p => p.Add(x => x.Title, "Confirm"));
        await cut.InvokeAsync(() => cut.Instance.Show());
        cut.Render();

        // Assert - two buttons: Cancel and Confirm
        var buttons = cut.FindAll("button");
        Assert.True(buttons.Count >= 2);
    }

    [Fact]
    public async Task HidesCancelButtonWhenShowCancelButtonFalse()
    {
        // Arrange & Act
        var cut = RenderConfirmDialog(p => p
            .Add(x => x.Title, "Confirm")
            .Add(x => x.ShowCancelButton, false));
        await cut.InvokeAsync(() => cut.Instance.Show());
        cut.Render();

        // Assert - only Confirm button (SbButton with Ghost variant is Cancel)
        var ghostButtons = cut.FindAll("button.sb-button--ghost");
        Assert.Empty(ghostButtons);
    }

    [Fact]
    public async Task AppliesCustomButtonText()
    {
        // Arrange & Act
        var cut = RenderConfirmDialog(p => p
            .Add(x => x.Title, "Confirm")
            .Add(x => x.ConfirmText, "Yes, delete")
            .Add(x => x.CancelText, "Keep"));
        await cut.InvokeAsync(() => cut.Instance.Show());
        cut.Render();

        // Assert
        Assert.Contains("Yes, delete", cut.Markup);
        Assert.Contains("Keep", cut.Markup);
    }

    [Fact]
    public async Task AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderConfirmDialog(p => p
            .Add(x => x.Title, "Confirm")
            .Add(x => x.Class, "my-dialog"));
        await cut.InvokeAsync(() => cut.Instance.Show());
        cut.Render();

        // Assert
        var dialog = cut.Find(".sb-confirm-dialog");
        Assert.Contains("my-dialog", dialog.ClassList);
    }

    [Fact]
    public async Task HasAlertDialogRole()
    {
        // Arrange & Act
        var cut = RenderConfirmDialog(p => p.Add(x => x.Title, "Confirm"));
        await cut.InvokeAsync(() => cut.Instance.Show());
        cut.Render();

        // Assert
        var dialog = cut.Find(".sb-confirm-dialog");
        Assert.Equal("alertdialog", dialog.GetAttribute("role"));
        Assert.Equal("true", dialog.GetAttribute("aria-modal"));
    }

    [Fact]
    public async Task ConfirmButtonInvokesOnConfirm()
    {
        // Arrange
        var confirmed = false;
        var cut = RenderConfirmDialog(p => p
            .Add(x => x.Title, "Confirm")
            .Add(x => x.OnConfirm, EventCallback.Factory.Create(this, () => confirmed = true)));
        await cut.InvokeAsync(() => cut.Instance.Show());
        cut.Render();

        // Act - click Confirm (Solid button, not Ghost)
        var confirmBtn = cut.FindAll("button.sb-button--solid").Last();
        await cut.InvokeAsync(() => confirmBtn.Click());

        // Assert
        Assert.True(confirmed);
    }

    [Fact]
    public async Task CancelButtonInvokesOnCancel()
    {
        // Arrange
        var cancelled = false;
        var cut = RenderConfirmDialog(p => p
            .Add(x => x.Title, "Confirm")
            .Add(x => x.OnCancel, EventCallback.Factory.Create(this, () => cancelled = true)));
        await cut.InvokeAsync(() => cut.Instance.Show());
        cut.Render();

        // Act - click Cancel (Ghost button)
        var cancelBtn = cut.Find("button.sb-button--ghost");
        await cut.InvokeAsync(() => cancelBtn!.Click());

        // Assert
        Assert.True(cancelled);
    }

    [Fact]
    public async Task ShowAsyncReturnsTrueWhenConfirmed()
    {
        // Arrange
        var cut = RenderConfirmDialog(p => p
            .Add(x => x.Title, "Confirm")
            .Add(x => x.ShowCancelButton, false));
        Task<bool> showTask = null!;
        await cut.InvokeAsync(() => { showTask = cut.Instance.ShowAsync(); });
        cut.Render();

        // Act
        var confirmBtn = cut.Find("button.sb-button--solid");
        await cut.InvokeAsync(() => confirmBtn!.Click());

        // Assert
        var result = await showTask;
        Assert.True(result);
    }

    [Fact]
    public async Task ShowAsyncReturnsFalseWhenCancelled()
    {
        // Arrange
        var cut = RenderConfirmDialog(p => p.Add(x => x.Title, "Confirm"));
        Task<bool> showTask = null!;
        await cut.InvokeAsync(() => { showTask = cut.Instance.ShowAsync(); });
        cut.Render();

        // Act - click Cancel
        var cancelBtn = cut.Find("button.sb-button--ghost");
        await cut.InvokeAsync(() => cancelBtn!.Click());

        // Assert
        var result = await showTask;
        Assert.False(result);
    }

    [Fact]
    public async Task CloseHidesDialog()
    {
        // Arrange & Act
        var cut = RenderConfirmDialog(p => p.Add(x => x.Title, "Confirm"));
        await cut.InvokeAsync(() => cut.Instance.Show());
        cut.Render();
        Assert.NotNull(cut.Find(".sb-confirm-dialog-overlay"));

        await cut.InvokeAsync(() => cut.Instance.Close());
        cut.Render();

        // Assert
        Assert.Empty(cut.FindAll(".sb-confirm-dialog-overlay"));
    }

    [Fact]
    public async Task RendersIconSection()
    {
        // Arrange & Act
        var cut = RenderConfirmDialog(p => p.Add(x => x.Title, "Confirm"));
        await cut.InvokeAsync(() => cut.Instance.Show());
        cut.Render();

        // Assert
        var icon = cut.Find(".sb-confirm-dialog__icon");
        Assert.NotNull(icon);
    }
}
