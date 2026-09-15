using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SufiChain.SufiBlazor.Interop;

/// <summary>
/// JavaScript interop for the CodeMirror 6 code editor.
/// </summary>
public sealed class SbCodeEditorInterop : IAsyncDisposable
{
    public const string ModulePath = "./_content/SufiChain.SufiBlazor/_sufi/editors/code/index.js";

    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _module;
    private bool _disposed;

    public SbCodeEditorInterop(IJSRuntime jsRuntime)
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
        SbCodeEditorInitOptions options) where T : class
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

    public async Task<string> GetValueAsync(string editorId)
    {
        var module = await EnsureModuleAsync();
        return await module.InvokeAsync<string>("getValue", editorId);
    }

    public async Task SetValueAsync(string editorId, string value)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("setValue", editorId, value);
    }

    public async Task InsertTextAsync(string editorId, string text)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("insertText", editorId, text);
    }

    public async Task<string> GetSelectionAsync(string editorId)
    {
        var module = await EnsureModuleAsync();
        return await module.InvokeAsync<string>("getSelection", editorId);
    }

    public async Task FocusAsync(string editorId)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("focusEditor", editorId);
    }

    public async Task<bool> FormatJsonAsync(string editorId)
    {
        var module = await EnsureModuleAsync();
        return await module.InvokeAsync<bool>("formatJson", editorId);
    }

    public async Task<bool> ValidateJsonAsync(string editorId)
    {
        var module = await EnsureModuleAsync();
        return await module.InvokeAsync<bool>("validateJson", editorId);
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

public sealed class SbCodeEditorInitOptions
{
    public string? Value { get; set; }
    public string? Language { get; set; }
    public bool ReadOnly { get; set; }
    public bool Disabled { get; set; }
    public string? Direction { get; set; }
    public bool LineNumbers { get; set; } = true;
    public bool WordWrap { get; set; }
    public string? Placeholder { get; set; }
    public bool ValidateJson { get; set; }
}
