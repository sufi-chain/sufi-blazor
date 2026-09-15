using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SufiChain.SufiBlazor.Contracts.Editors;

namespace SufiChain.SufiBlazor.Interop;

/// <summary>
/// JavaScript interop for the Tiptap-backed rich text editor.
/// </summary>
public sealed class SbRichTextEditorInterop : IAsyncDisposable
{
    public const string ModulePath = "./_content/SufiChain.SufiBlazor/_sufi/editors/rich-text/index.js";

    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _module;
    private bool _disposed;

    public SbRichTextEditorInterop(IJSRuntime jsRuntime)
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
        SbRichTextEditorInitOptions options,
        object? bubbleMenu = null) where T : class
    {
        var module = await EnsureModuleAsync();
        return await module.InvokeAsync<string>("initEditor", container, dotNetRef, options, bubbleMenu);
    }

    public async Task DestroyAsync(string editorId)
    {
        if (_module == null)
        {
            return;
        }

        await _module.InvokeVoidAsync("destroyEditor", editorId);
    }

    public async Task<string> GetContentAsync(string editorId, SbContentFormat format)
    {
        var module = await EnsureModuleAsync();
        return await module.InvokeAsync<string>("getContent", editorId, format.ToString().ToLowerInvariant());
    }

    public async Task SetContentAsync(string editorId, string content, SbContentFormat format)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("setContent", editorId, content, format.ToString().ToLowerInvariant());
    }

    public async Task FocusAsync(string editorId)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("focusEditor", editorId);
    }

    public async Task SetEditableAsync(string editorId, bool editable)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("setEditable", editorId, editable);
    }

    public async Task SetDirectionAsync(string editorId, string direction)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("setDirection", editorId, direction);
    }

    public async Task ExecCommandAsync(string editorId, SbEditorCommand command, object? value = null)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("execCommand", editorId, command.ToString(), value);
    }

    public async Task InsertContentAsync(string editorId, string text, SbContentFormat format)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("insertContent", editorId, text, format.ToString().ToLowerInvariant());
    }

    public async Task InsertLinkAsync(string editorId, string url, string? text = null, string? target = null, string? rel = null)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("insertLink", editorId, url, text, target ?? "", rel ?? "");
    }

    public async Task InsertImageAsync(string editorId, string url, string? alt = null, string? width = null, string? height = null)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("insertImage", editorId, url, alt, width, height);
    }

    public async Task InsertFileAsync(string editorId, string url, string name, string? mime = null)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("insertFile", editorId, url, name);
    }

    public async Task ApplyMarkAsync(string editorId, string mark, object? attrs = null)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("applyMark", editorId, mark, attrs);
    }

    public async Task ApplyBlockAsync(string editorId, string block, object? attrs = null)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("applyBlock", editorId, block, attrs);
    }

    public async Task<SbEditorSelection> GetSelectionAsync(string editorId)
    {
        var module = await EnsureModuleAsync();
        return await module.InvokeAsync<SbEditorSelection>("getSelection", editorId);
    }

    public async Task ReplaceSelectionAsync(string editorId, string text, SbContentFormat format)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("replaceSelection", editorId, text, format.ToString().ToLowerInvariant());
    }

    public async Task ShowSuggestionsAsync(string editorId, IReadOnlyList<SbEditorSuggestion> suggestions)
    {
        var module = await EnsureModuleAsync();
        var json = JsonSerializer.Serialize(suggestions, SuggestionJsonOptions);
        await module.InvokeVoidAsync("showSuggestions", editorId, json);
    }

    public async Task AcceptSuggestionAsync(string editorId, string id)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("acceptSuggestion", editorId, id);
    }

    public async Task RejectSuggestionAsync(string editorId, string id)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("rejectSuggestion", editorId, id);
    }

    public async Task ClearSuggestionsAsync(string editorId)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("clearEditorSuggestions", editorId);
    }

    public async Task<SbEditorSuggestionRect?> GetSuggestionRectAsync(string editorId, string? id = null)
    {
        var module = await EnsureModuleAsync();
        return await module.InvokeAsync<SbEditorSuggestionRect?>("getSuggestionRect", editorId, id);
    }

    private static readonly JsonSerializerOptions SuggestionJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task StreamInsertAsync(string editorId, string chunk)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("streamInsert", editorId, chunk);
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

public sealed class SbRichTextEditorInitOptions
{
    public string? Placeholder { get; set; }
    public bool ReadOnly { get; set; }
    public bool Disabled { get; set; }
    public string? Direction { get; set; }
    public string? Content { get; set; }
    public string ContentFormat { get; set; } = "html";
    public SbPasteCleanupOptions? PasteCleanup { get; set; }
    public int Features { get; set; } = (int)SbEditorFeatures.Default;
}
