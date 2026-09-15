using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SufiChain.SufiBlazor.Interop;

/// <summary>
/// JavaScript interop for the CodeMirror 6 merge/diff editor.
/// </summary>
public sealed class SbDiffEditorInterop : IAsyncDisposable
{
    public const string ModulePath = "./_content/SufiChain.SufiBlazor/_sufi/editors/diff/index.js";

    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _module;
    private bool _disposed;

    public SbDiffEditorInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    private async Task<IJSObjectReference> EnsureModuleAsync()
    {
        _module ??= await _jsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath);
        return _module;
    }

    public async Task<string> InitializeAsync<T>(
        ElementReference container,
        DotNetObjectReference<T> dotNetRef,
        SbDiffEditorInitOptions options) where T : class
    {
        var module = await EnsureModuleAsync();
        return await module.InvokeAsync<string>("initEditor", container, dotNetRef, options);
    }

    public async Task DestroyAsync(string editorId)
    {
        if (_module == null)
        {
            return;
        }

        await _module.InvokeVoidAsync("destroyEditor", editorId);
    }

    public async Task<string> GetSuggestedAsync(string editorId)
    {
        var module = await EnsureModuleAsync();
        return await module.InvokeAsync<string>("getSuggested", editorId);
    }

    public async Task SetValuesAsync(string editorId, string original, string suggested)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("setValues", editorId, original, suggested);
    }

    public async Task GoToChunkAsync(string editorId, string direction)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("goToChunk", editorId, direction);
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

public sealed class SbDiffEditorInitOptions
{
    public string? Original { get; set; }
    public string? Suggested { get; set; }
    public string? Language { get; set; }
    public bool ReadOnlySuggested { get; set; }
    public string? Direction { get; set; }
}
