using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SufiChain.SufiBlazor.Contracts.Editors;
using SufiChain.SufiBlazor.Interop;

namespace SufiChain.SufiBlazor.Components.Common;

/// <summary>
/// Read-only document renderer (Markdown/HTML/JSON) via the Tiptap static renderer.
/// </summary>
public partial class SbDocumentViewer : ComponentBase, IAsyncDisposable
{
    private ElementReference _container;
    private SbDocumentViewerInterop? _interop;
    private bool _useFallback;
    private bool _disposed;
    private string? _lastContent;
    private string? _lastFormat;
    private string? _lastDirection;

    [Parameter] public string? Content { get; set; }
    [Parameter] public SbContentFormat Format { get; set; } = SbContentFormat.Markdown;
    [Parameter] public bool EnableMermaid { get; set; } = true;
    [Parameter] public bool EnableHighlight { get; set; } = true;
    [Parameter] public bool? RightToLeft { get; set; }

    /// <summary>
    /// Text direction for the document root and the renderer.
    /// <c>auto</c> follows the content. <c>ltr</c> and <c>rtl</c> force a direction.
    /// Null keeps the culture direction from <see cref="RightToLeft"/>.
    /// </summary>
    [Parameter] public string? TextDirection { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    protected bool EffectiveRightToLeft =>
        RightToLeft ?? CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;

    protected string ResolvedTextDirection =>
        NormalizeTextDirection(TextDirection) ?? (EffectiveRightToLeft ? "rtl" : "ltr");

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_disposed)
        {
            return;
        }

        if (firstRender)
        {
            _interop = new SbDocumentViewerInterop(JSRuntime);
        }

        var format = Format.ToString().ToLowerInvariant();
        var direction = ResolvedTextDirection;
        if (!firstRender && Content == _lastContent && format == _lastFormat && direction == _lastDirection)
        {
            return;
        }

        if (_interop == null)
        {
            return;
        }

        try
        {
            await _interop.RenderDocumentAsync(_container, Content ?? "", format, new SbDocumentViewerRenderOptions
            {
                EnableHighlight = EnableHighlight,
                EnableMermaid = EnableMermaid,
                Direction = direction
            });
            _lastContent = Content;
            _lastFormat = format;
            _lastDirection = direction;
            _useFallback = false;
        }
        catch (JSException)
        {
            _useFallback = true;
            await InvokeAsync(StateHasChanged);
        }
    }

    private string GetContainerClass() =>
        string.IsNullOrWhiteSpace(Class) ? "sb-document-viewer" : $"sb-document-viewer {Class}";

    private static string? NormalizeTextDirection(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim().ToLowerInvariant() switch
        {
            "auto" => "auto",
            "ltr" => "ltr",
            "rtl" => "rtl",
            _ => null
        };
    }

    public async ValueTask DisposeAsync()
    {
        _disposed = true;
        if (_interop != null)
        {
            try
            {
                await _interop.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }
    }
}
