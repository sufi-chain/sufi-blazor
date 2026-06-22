using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SufiChain.SufiBlazor.Interop;

/// <summary>
/// JavaScript interop for the markdown editor.
/// </summary>
public class SbMarkdownEditorInterop : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _module;
    private bool _disposed;

    public SbMarkdownEditorInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    private async Task<IJSObjectReference> EnsureModuleAsync()
    {
        _module ??= await _jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/SufiChain.SufiBlazor/sufiblazor-markdown-editor.js");
        return _module;
    }

    public async Task EnsureAssetsAsync(SbMarkdownAssetOptions options)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("ensureAssets", options);
    }

    public async Task<string?> InitEditorAsync<T>(
        ElementReference textarea,
        DotNetObjectReference<T> dotNetRef,
        SbMarkdownEditorInitOptions options) where T : class
    {
        var module = await EnsureModuleAsync();
        return await module.InvokeAsync<string?>("initEditor", textarea, dotNetRef, options);
    }

    public async Task<string?> InitDiffReviewAsync<T>(
        ElementReference container,
        DotNetObjectReference<T> dotNetRef,
        SbMarkdownDiffInitOptions options) where T : class
    {
        var module = await EnsureModuleAsync();
        return await module.InvokeAsync<string?>("initDiffReview", container, dotNetRef, options);
    }

    public async Task<string> GetValueAsync(string editorId)
    {
        var module = await EnsureModuleAsync();
        return await module.InvokeAsync<string>("getValue", editorId);
    }

    public async Task SetValueAsync(string editorId, string value, string? originalValue = null)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("setValue", editorId, value, originalValue);
    }

    public async Task InsertTextAtCursorAsync(string editorId, string text)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("insertTextAtCursor", editorId, text);
    }

    public async Task ExecActionAsync(string editorId, string action, object? value = null)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("execAction", editorId, action, value);
    }

    public async Task TogglePreviewAsync(string editorId)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("togglePreview", editorId);
    }

    public async Task SetPreviewAsync(string editorId, bool show)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("setPreview", editorId, show);
    }

    public async Task DestroyEditorAsync(string editorId)
    {
        if (_module != null)
        {
            try
            {
                await _module.InvokeVoidAsync("destroyEditor", editorId);
            }
            catch (JSDisconnectedException)
            {
            }
            catch (JSException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }
    }

    public async Task<string> RenderMarkdownAsync(string content, SbMarkdownAssetOptions? options = null)
    {
        var module = await EnsureModuleAsync();
        return await module.InvokeAsync<string>("renderMarkdown", content, options ?? new SbMarkdownAssetOptions());
    }

    public async Task EnhanceRenderedMarkdownAsync(ElementReference element, SbMarkdownAssetOptions? options = null)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("enhanceRenderedMarkdown", element, options ?? new SbMarkdownAssetOptions());
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed && _module != null)
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

            _disposed = true;
        }
    }
}

/// <summary>
/// Options for initializing the markdown editor.
/// </summary>
public class SbMarkdownEditorInitOptions
{
    public string? EditorId { get; set; }
    public string? Value { get; set; }
    public string? Placeholder { get; set; }
    public bool ReadOnly { get; set; }
    public string? Direction { get; set; }
    public string EditorMode { get; set; } = "markdown";
    public string? SourceLanguage { get; set; }
    public bool EnablePreview { get; set; } = true;
    public bool EnableMermaid { get; set; } = true;
    public bool EnableHighlight { get; set; } = true;
    public string HighlightTheme { get; set; } = "github";
    public string? MinHeight { get; set; }
    public string? MaxHeight { get; set; }
    public bool LineNumbers { get; set; } = true;
    public bool LineWrapping { get; set; } = true;
}

/// <summary>
/// Options for initializing diff review mode.
/// </summary>
public class SbMarkdownDiffInitOptions
{
    public string? EditorId { get; set; }
    public string? Original { get; set; }
    public string? Modified { get; set; }
    public bool ReadOnly { get; set; }
    public string EditorMode { get; set; } = "markdown";
}

/// <summary>
/// Options for loading markdown rendering assets.
/// </summary>
public class SbMarkdownAssetOptions
{
    public bool EnableMermaid { get; set; } = true;
    public bool EnableHighlight { get; set; } = true;
    public string HighlightTheme { get; set; } = "github";
}
