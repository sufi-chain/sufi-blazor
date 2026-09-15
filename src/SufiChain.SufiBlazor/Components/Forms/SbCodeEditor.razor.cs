using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using SufiChain.SufiBlazor.Contracts.Editors;
using SufiChain.SufiBlazor.Interop;

namespace SufiChain.SufiBlazor.Components.Forms;

/// <summary>
/// CodeMirror 6 source editor for HTML, JSON, Scriban, and other languages.
/// </summary>
public partial class SbCodeEditor : ComponentBase, IAsyncDisposable, ISbEditorDocument
{
    private ElementReference _editorContainer;
    private SbCodeEditorInterop? _interop;
    private DotNetObjectReference<SbCodeEditor>? _dotNetRef;
    private string? _editorId;
    private bool _useFallback;
    private bool _disposed;
    private string? _lastValue;
    private List<EditorToolbarItem> _toolbarItems = [];
    private readonly EditorState _state = new();
    private bool _jsonValid = true;

    [Parameter] public string Value { get; set; } = "";
    [Parameter] public EventCallback<string> ValueChanged { get; set; }
    [Parameter] public SbCodeLanguage Language { get; set; } = SbCodeLanguage.PlainText;
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool? RightToLeft { get; set; }
    [Parameter] public bool LineNumbers { get; set; } = true;
    [Parameter] public bool WordWrap { get; set; }
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public string? MinHeight { get; set; } = "200px";
    [Parameter] public string? MaxHeight { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter] public bool HideToolbar { get; set; } = true;
    [Parameter] public bool UseToolbarContributors { get; set; }
    [Parameter] public string? ToolbarScope { get; set; }
    [Parameter] public bool ValidateJson { get; set; }
    [Parameter] public EventCallback<bool> JsonValidChanged { get; set; }
    [Parameter] public EventCallback<string> OnShortcut { get; set; }
    [Parameter] public string ToolbarAriaLabel { get; set; } = "Code editor toolbar";
    [Parameter] public string EditorAriaLabel { get; set; } = "Code editor";

    public bool IsJsonValid => _jsonValid;

    protected bool EffectiveRightToLeft =>
        RightToLeft ?? CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;

    private SbContentFormat ContentFormat => Language == SbCodeLanguage.Markdown
        ? SbContentFormat.Markdown
        : SbContentFormat.Html;

    protected override async Task OnParametersSetAsync()
    {
        if (_editorId != null && _interop != null && Value != _lastValue)
        {
            await _interop.SetValueAsync(_editorId, Value ?? "");
            _lastValue = Value;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_disposed || !firstRender)
        {
            return;
        }

        if (UseToolbarContributors)
        {
            var service = ServiceProvider.GetService<IEditorToolbarService>();
            if (service != null)
            {
                _toolbarItems = await service.GetToolbarItemsAsync(
                    _editorId,
                    includeDefaults: false,
                    ToolbarScope,
                    SbEditorSurface.Code,
                    ContentFormat,
                    this);
            }
        }

        _interop = new SbCodeEditorInterop(JSRuntime);
        _dotNetRef = DotNetObjectReference.Create(this);
        try
        {
            _editorId = await _interop.InitializeAsync(_editorContainer, _dotNetRef, new SbCodeEditorInitOptions
            {
                Value = Value,
                Language = Language.ToString().ToLowerInvariant(),
                ReadOnly = ReadOnly,
                Disabled = Disabled,
                Direction = EffectiveRightToLeft ? "rtl" : "ltr",
                LineNumbers = LineNumbers,
                WordWrap = WordWrap,
                Placeholder = Placeholder,
                ValidateJson = ValidateJson && Language == SbCodeLanguage.Json
            });
            _lastValue = Value;
        }
        catch (JSException)
        {
            _useFallback = true;
        }

        await InvokeAsync(StateHasChanged);
    }

    private string GetContainerClass()
    {
        var classes = new List<string> { "sb-code-editor" };
        if (Disabled) classes.Add("sb-code-editor--disabled");
        if (ReadOnly) classes.Add("sb-code-editor--readonly");
        if (!string.IsNullOrWhiteSpace(Class)) classes.Add(Class);
        return string.Join(' ', classes);
    }

    private string GetContainerStyle()
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(MinHeight)) parts.Add($"min-height: {MinHeight}");
        if (!string.IsNullOrWhiteSpace(MaxHeight)) parts.Add($"max-height: {MaxHeight}");
        if (!string.IsNullOrWhiteSpace(Style)) parts.Add(Style);
        return string.Join("; ", parts);
    }

    private async Task OnFallbackValueChangedAsync(string? value)
    {
        _lastValue = value;
        await ValueChanged.InvokeAsync(value ?? "");
    }

    private async Task OnToolbarItemClickAsync(EditorToolbarItem item)
    {
        if (item.OnClickAsync != null)
        {
            await item.OnClickAsync(new EditorActionContext(_editorId ?? "", ServiceProvider, this)
            {
                Surface = SbEditorSurface.Code,
                ContentFormat = ContentFormat,
                State = _state
            });
        }
    }

    [JSInvokable]
    public async Task OnEditorContentChanged(string value)
    {
        if (value == _lastValue)
        {
            return;
        }

        _lastValue = value;
        await ValueChanged.InvokeAsync(value);
        if (ValidateJson && Language == SbCodeLanguage.Json)
        {
            await RefreshJsonValidityAsync();
        }
    }

    [JSInvokable]
    public Task OnEditorShortcut(string name) => OnShortcut.InvokeAsync(name);

    public async Task<string> GetValueAsync()
    {
        if (_editorId == null || _interop == null)
        {
            return Value;
        }

        return await _interop.GetValueAsync(_editorId);
    }

    public async Task SetValueAsync(string value)
    {
        Value = value;
        _lastValue = value;
        if (_editorId != null && _interop != null)
        {
            await _interop.SetValueAsync(_editorId, value);
        }

        await ValueChanged.InvokeAsync(value);
    }

    public async Task InsertTextAsync(string text)
    {
        if (_editorId != null && _interop != null)
        {
            await _interop.InsertTextAsync(_editorId, text);
            return;
        }

        Value += text;
        await ValueChanged.InvokeAsync(Value);
    }

    public async Task FormatAsync()
    {
        if (_editorId == null || _interop == null || Language != SbCodeLanguage.Json)
        {
            return;
        }

        await _interop.FormatJsonAsync(_editorId);
    }

    public async Task RefreshJsonValidityAsync()
    {
        var valid = true;
        if (_editorId != null && _interop != null)
        {
            valid = await _interop.ValidateJsonAsync(_editorId);
        }

        if (valid == _jsonValid)
        {
            return;
        }

        _jsonValid = valid;
        await JsonValidChanged.InvokeAsync(valid);
    }

    public Task<string> GetDocumentAsync(SbContentFormat format) => GetValueAsync();
    public Task SetDocumentAsync(string text, SbContentFormat format) => SetValueAsync(text);
    public async Task<SbEditorSelection> GetSelectionAsync()
    {
        var text = _editorId == null || _interop == null ? "" : await _interop.GetSelectionAsync(_editorId);
        return new SbEditorSelection { Text = text };
    }

    public Task ReplaceSelectionAsync(string text, SbContentFormat format) => InsertTextAsync(text);
    public Task ReplaceDocumentAsync(string text, SbContentFormat format) => SetValueAsync(text);
    public Task InsertContentAsync(string text, SbContentFormat format) => InsertTextAsync(text);
    public Task InsertLinkAsync(string url, string? text = null, string? target = null, string? rel = null) =>
        InsertTextAsync(text ?? url);
    public Task InsertImageAsync(string url, string? alt = null, string? width = null, string? height = null) =>
        InsertTextAsync($"![{alt}]({url})");
    public Task InsertFileAsync(string url, string name, string? mime = null) =>
        InsertTextAsync($"[{name}]({url})");
    public Task InsertMentionAsync(string id, string label) => InsertTextAsync($"@{label}");
    public Task InsertNodeAsync(string nodeName, IReadOnlyDictionary<string, object?>? attrs = null) => Task.CompletedTask;
    public Task ApplyMarkAsync(string mark, IReadOnlyDictionary<string, object?>? attrs = null) => Task.CompletedTask;
    public Task ApplyBlockAsync(string block, IReadOnlyDictionary<string, object?>? attrs = null) => Task.CompletedTask;
    public Task ExecuteCommandAsync(SbEditorCommand command) => Task.CompletedTask;
    public Task ShowSuggestionsAsync(IReadOnlyList<SbEditorSuggestion> suggestions) => Task.CompletedTask;
    public Task AcceptSuggestionAsync(string id) => Task.CompletedTask;
    public Task RejectSuggestionAsync(string id) => Task.CompletedTask;
    public Task AcceptAllSuggestionsAsync() => Task.CompletedTask;
    public Task RejectAllSuggestionsAsync() => Task.CompletedTask;
    public Task StreamInsertAsync(string chunk) => InsertTextAsync(chunk);
    public Task FocusAsync() =>
        _editorId == null || _interop == null ? Task.CompletedTask : _interop.FocusAsync(_editorId);

    public async ValueTask DisposeAsync()
    {
        _disposed = true;
        if (_editorId != null && _interop != null)
        {
            try
            {
                await _interop.DestroyAsync(_editorId);
            }
            catch (JSDisconnectedException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }

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

        _dotNetRef?.Dispose();
    }
}
