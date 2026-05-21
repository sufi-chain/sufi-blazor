using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SufiChain.SufiBlazor.Contracts.Editors;

namespace SufiChain.SufiBlazor.Interop;

/// <summary>
/// JavaScript interop service for the rich text editor.
/// </summary>
public class SbEditorInterop : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _module;
    private bool _disposed;
    
    public SbEditorInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    /// <summary>
    /// Ensures the editor module is loaded.
    /// </summary>
    private async Task<IJSObjectReference> EnsureModuleAsync()
    {
        if (_module == null)
        {
            _module = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/SufiChain.SufiBlazor/sufiblazor-editor.js");
        }
        return _module;
    }
    
    /// <summary>
    /// Initializes the editor on the specified element.
    /// </summary>
    public async Task<string> InitializeEditorAsync<T>(
        ElementReference container,
        DotNetObjectReference<T> dotNetRef,
        SbEditorOptions options) where T : class
    {
        var module = await EnsureModuleAsync();
        return await module.InvokeAsync<string>("initEditor", container, dotNetRef, options);
    }
    
    /// <summary>
    /// Destroys the editor instance.
    /// </summary>
    public async Task DestroyEditorAsync(string editorId)
    {
        if (_module != null)
        {
            await _module.InvokeVoidAsync("destroyEditor", editorId);
        }
    }
    
    /// <summary>
    /// Gets the editor content.
    /// </summary>
    public async Task<string> GetContentAsync(string editorId, SbEditorMode mode)
    {
        var module = await EnsureModuleAsync();
        return await module.InvokeAsync<string>("getContent", editorId, mode.ToString().ToLower());
    }
    
    /// <summary>
    /// Sets the editor content.
    /// </summary>
    public async Task SetContentAsync(string editorId, string content, SbEditorMode mode)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("setContent", editorId, content, mode.ToString().ToLower());
    }
    
    /// <summary>
    /// Focuses the editor.
    /// </summary>
    public async Task FocusAsync(string editorId)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("focus", editorId);
    }
    
    /// <summary>
    /// Gets the current selection range.
    /// </summary>
    public async Task<SbEditorRange?> GetSelectionAsync(string editorId)
    {
        var module = await EnsureModuleAsync();
        return await module.InvokeAsync<SbEditorRange?>("getSelection", editorId);
    }
    
    /// <summary>
    /// Sets the selection range.
    /// </summary>
    public async Task SetSelectionAsync(string editorId, int index, int length)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("setSelection", editorId, index, length);
    }
    
    /// <summary>
    /// Applies a format to the current selection.
    /// </summary>
    public async Task FormatAsync(string editorId, string format, object? value = null)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("format", editorId, format, value);
    }
    
    /// <summary>
    /// Inserts content at the current cursor position.
    /// </summary>
    public async Task InsertAsync(string editorId, string type, object? value = null)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("insert", editorId, type, value);
    }
    
    /// <summary>
    /// Inserts an image at the current cursor position.
    /// </summary>
    /// <param name="editorId">The editor instance ID.</param>
    /// <param name="url">The image URL.</param>
    /// <param name="alt">Optional alt text.</param>
    /// <param name="width">Optional width (e.g. "200" for pixels).</param>
    /// <param name="height">Optional height (e.g. "150" for pixels).</param>
    public async Task InsertImageAsync(string editorId, string url, string? alt = null, string? width = null, string? height = null)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("insertImage", editorId, url, alt ?? "", width ?? "", height ?? "");
    }
    
    /// <summary>
    /// Inserts HTML content at the current cursor position.
    /// </summary>
    public async Task InsertHtmlAsync(string editorId, string html)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("insertHtml", editorId, html);
    }
    
    /// <summary>
    /// Gets the currently selected text.
    /// </summary>
    public async Task<string?> GetSelectionTextAsync(string editorId)
    {
        var module = await EnsureModuleAsync();
        return await module.InvokeAsync<string?>("getSelectionText", editorId);
    }
    
    /// <summary>
    /// Gets the format at the current selection (e.g. link URL when cursor is in a link).
    /// </summary>
    public async Task<Dictionary<string, object>> GetFormatAsync(string editorId)
    {
        var module = await EnsureModuleAsync();
        var result = await module.InvokeAsync<JsonElement?>("getFormat", editorId);
        if (!result.HasValue || result.Value.ValueKind != JsonValueKind.Object)
        {
            return new Dictionary<string, object>();
        }
        var dict = new Dictionary<string, object>();
        foreach (var prop in result.Value.EnumerateObject())
        {
            dict[prop.Name] = prop.Value.ValueKind switch
            {
                JsonValueKind.String => prop.Value.GetString() ?? "",
                JsonValueKind.Number => prop.Value.TryGetInt32(out var i) ? i : prop.Value.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => prop.Value.ToString()
            };
        }
        return dict;
    }
    
    /// <summary>
    /// Inserts a link at the current selection.
    /// </summary>
    /// <param name="editorId">The editor instance ID.</param>
    /// <param name="url">The link URL.</param>
    /// <param name="text">Optional link text when inserting without a selection.</param>
    /// <param name="target">Optional target (e.g. "_blank", "_self").</param>
    /// <param name="rel">Optional rel (e.g. "noopener noreferrer").</param>
    public async Task InsertLinkAsync(string editorId, string url, string? text = null, string? target = null, string? rel = null)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("insertLink", editorId, url, text, target ?? "", rel ?? "");
    }
    
    /// <summary>
    /// Performs undo.
    /// </summary>
    public async Task UndoAsync(string editorId)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("undo", editorId);
    }
    
    /// <summary>
    /// Performs redo.
    /// </summary>
    public async Task RedoAsync(string editorId)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("redo", editorId);
    }
    
    /// <summary>
    /// Clears formatting from the current selection.
    /// </summary>
    public async Task ClearFormattingAsync(string editorId)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("clearFormatting", editorId);
    }
    
    /// <summary>
    /// Enables or disables the editor.
    /// </summary>
    public async Task SetEnabledAsync(string editorId, bool enabled)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("setEnabled", editorId, enabled);
    }
    
    /// <summary>
    /// Sets the text direction.
    /// </summary>
    public async Task SetDirectionAsync(string editorId, string direction)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("setDirection", editorId, direction);
    }
    
    /// <summary>
    /// Updates paste cleanup options for the editor.
    /// </summary>
    public async Task SetPasteCleanupOptionsAsync(string editorId, SbPasteCleanupOptions? options)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("setPasteCleanupOptions", editorId, options);
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
                // Circuit disconnected, ignore
            }
            catch (ObjectDisposedException)
            {
                // Already disposed, ignore
            }
            _disposed = true;
        }
    }
}

/// <summary>
/// Options for initializing the editor.
/// </summary>
public class SbEditorOptions
{
    public string? Placeholder { get; set; }
    public bool ReadOnly { get; set; }
    public string Theme { get; set; } = "snow";
    public string? Direction { get; set; }
    public object[]? Toolbar { get; set; }
    public SbPasteCleanupOptions? PasteCleanup { get; set; }
}

/// <summary>
/// Represents a selection range in the editor.
/// </summary>
public class SbEditorRange
{
    public int Index { get; set; }
    public int Length { get; set; }
}
