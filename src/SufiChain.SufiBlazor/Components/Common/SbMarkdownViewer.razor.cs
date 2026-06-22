using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SufiChain.SufiBlazor.Interop;

namespace SufiChain.SufiBlazor.Components.Common;

/// <summary>
/// Renders markdown content using client-side marked.js with optional mermaid and syntax highlighting.
/// </summary>
public partial class SbMarkdownViewer : ComponentBase, IAsyncDisposable
{
    [Parameter] public string? Content { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter] public bool EnableMermaid { get; set; } = true;
    [Parameter] public bool EnableHighlight { get; set; } = true;
    [Parameter] public string HighlightTheme { get; set; } = "github";
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private ElementReference _containerRef;
    private SbMarkdownEditorInterop? _interop;
    private string? _lastRenderedContent;
    private bool _isClientRendered;

    private string CssClass => string.IsNullOrWhiteSpace(Class)
        ? "sb-markdown-viewer"
        : $"sb-markdown-viewer {Class}";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (Content == _lastRenderedContent && !firstRender)
        {
            return;
        }

        _lastRenderedContent = Content;
        _interop ??= new SbMarkdownEditorInterop(JSRuntime);

        try
        {
            var html = await _interop.RenderMarkdownAsync(Content ?? string.Empty, new SbMarkdownAssetOptions
            {
                EnableMermaid = EnableMermaid,
                EnableHighlight = EnableHighlight,
                HighlightTheme = HighlightTheme
            });

            await JSRuntime.InvokeVoidAsync("SufiBlazor.markdown.setInnerHtml", _containerRef, html);
            await _interop.EnhanceRenderedMarkdownAsync(_containerRef, new SbMarkdownAssetOptions
            {
                EnableMermaid = EnableMermaid,
                EnableHighlight = EnableHighlight,
                HighlightTheme = HighlightTheme
            });
            _isClientRendered = true;
        }
        catch (JSException)
        {
            _isClientRendered = false;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_interop != null)
        {
            await _interop.DisposeAsync();
        }
    }
}
