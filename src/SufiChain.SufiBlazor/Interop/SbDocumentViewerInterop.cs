using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SufiChain.SufiBlazor.Interop;

/// <summary>
/// JavaScript interop for the static document viewer.
/// </summary>
public sealed class SbDocumentViewerInterop : IAsyncDisposable
{
    public const string ModulePath = "./_content/SufiChain.SufiBlazor/_sufi/editors/viewer/index.js";

    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _module;
    private bool _disposed;

    public SbDocumentViewerInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    private async Task<IJSObjectReference> EnsureModuleAsync()
    {
        _module ??= await _jsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath);
        return _module;
    }

    public async Task RenderDocumentAsync(
        ElementReference element,
        string content,
        string format,
        SbDocumentViewerRenderOptions? options = null)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("renderDocument", element, content ?? "", format, options);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        if (_module != null)
        {
            try
            {
                await _module.DisposeAsync();
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

public sealed class SbDocumentViewerRenderOptions
{
    public bool EnableHighlight { get; set; } = true;
    public bool EnableMermaid { get; set; } = true;
    public string? Direction { get; set; }
}
