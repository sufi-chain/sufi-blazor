using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SufiChain.SufiBlazor.Contracts.Editors;
using SufiChain.SufiBlazor.Interop;

namespace SufiChain.SufiBlazor.Components.Forms;

/// <summary>
/// Markdown editor with live preview, mermaid/highlight.js support, diff review, and toolbar contributors.
/// </summary>
public partial class SbMarkdownEditor : ComponentBase, IAsyncDisposable
{
    [Parameter] public string Value { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> ValueChanged { get; set; }
    [Parameter] public string? ValueHtml { get; set; }
    [Parameter] public EventCallback<string?> ValueHtmlChanged { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter] public bool RightToLeft { get; set; }
    [Parameter] public bool EnablePreview { get; set; } = true;
    [Parameter] public bool EnableMermaid { get; set; } = true;
    [Parameter] public bool EnableHighlight { get; set; } = true;
    [Parameter] public string HighlightTheme { get; set; } = "github";
    [Parameter] public SbMarkdownEditorMode EditorMode { get; set; } = SbMarkdownEditorMode.Markdown;
    [Parameter] public string? SourceLanguage { get; set; }
    [Parameter] public bool UseToolbarContributors { get; set; }
    [Parameter] public bool IncludeDefaultToolbarItems { get; set; } = true;
    /// <summary>
    /// Optional toolbar scope that filters which <see cref="IMdToolbarContributor"/>
    /// instances run for this editor. Contributors whose <c>Scope</c> is non-null
    /// only execute when it matches this value. Contributors with a null scope
    /// always run. This prevents page-specific toolbar items from leaking across
    /// navigations within the same Blazor circuit.
    /// </summary>
    [Parameter] public string? ToolbarScope { get; set; }
    [Parameter] public bool HideToolbar { get; set; }
    [Parameter] public IReadOnlyList<SbMarkdownToolbarItem>? ToolbarItems { get; set; }
    [Parameter] public EventCallback<string> OnShortcut { get; set; }
    [Parameter] public string? MinHeight { get; set; } = "200px";
    [Parameter] public string? MaxHeight { get; set; }
    [Parameter] public int FallbackRows { get; set; } = 12;
    [Parameter] public bool IsDiffReview { get; set; }
    [Parameter] public string OriginalValue { get; set; } = string.Empty;
    [Parameter] public string SuggestedValue { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> SuggestedValueChanged { get; set; }
    [Parameter] public EventCallback OnApplyChanges { get; set; }
    [Parameter] public EventCallback OnDiscardChanges { get; set; }
    [Parameter] public EventCallback<string> OnToolbarCustomAction { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private ElementReference _textarea;
    private ElementReference _diffContainer;
    private SbMarkdownEditorInterop? _interop;
    private DotNetObjectReference<SbMarkdownEditor>? _dotNetRef;
    private string? _editorId;
    private string _elementId = $"sb-md-{Guid.NewGuid():N}";
    private string? _lastRenderedValue;
    private string? _lastRenderedOriginal;
    private string? _lastRenderedSuggested;
    private bool _lastRenderedDiffMode;
    private SbMarkdownEditorMode _lastEditorMode;
    private string? _lastSourceLanguage;
    private bool _lastIncludeDefaultToolbarItems;
    private string? _lastToolbarScope;
    private List<SbMarkdownToolbarItem> _toolbarItems = new();
    private bool _useFallback;
    private bool _disposed;

    protected bool IsReady { get; private set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_disposed)
        {
            return;
        }

        if (firstRender)
        {
            _interop = new SbMarkdownEditorInterop(JSRuntime);

            await LoadToolbarItemsAsync();

            await InitializeEditorAsync();
            if (_disposed)
            {
                return;
            }

            _lastRenderedValue = Value;
            _lastRenderedOriginal = OriginalValue;
            _lastRenderedSuggested = SuggestedValue;
            _lastRenderedDiffMode = IsDiffReview;
            _lastEditorMode = EditorMode;
            _lastSourceLanguage = SourceLanguage;
            _lastIncludeDefaultToolbarItems = IncludeDefaultToolbarItems;
            _lastToolbarScope = ToolbarScope;
            await InvokeAsync(StateHasChanged);
            return;
        }

        if (_editorId == null || _interop == null)
        {
            return;
        }

        if (IsDiffReview != _lastRenderedDiffMode)
        {
            await ReinitializeEditorAsync();
            return;
        }

        if (IsDiffReview)
        {
            if (OriginalValue != _lastRenderedOriginal || SuggestedValue != _lastRenderedSuggested)
            {
                await _interop.SetValueAsync(_editorId, SuggestedValue, OriginalValue);
                _lastRenderedOriginal = OriginalValue;
                _lastRenderedSuggested = SuggestedValue;
            }
        }
        else if (Value != _lastRenderedValue ||
                 EditorMode != _lastEditorMode ||
                 SourceLanguage != _lastSourceLanguage ||
                 IncludeDefaultToolbarItems != _lastIncludeDefaultToolbarItems ||
                 ToolbarScope != _lastToolbarScope)
        {
            if (EditorMode != _lastEditorMode ||
                SourceLanguage != _lastSourceLanguage ||
                IncludeDefaultToolbarItems != _lastIncludeDefaultToolbarItems ||
                ToolbarScope != _lastToolbarScope)
            {
                await ReinitializeEditorAsync();
            }
            else
            {
                await _interop.SetValueAsync(_editorId, Value);
                _lastRenderedValue = Value;
            }
        }
    }

    private async Task LoadToolbarItemsAsync()
    {
        if (IsDiffReview)
        {
            return;
        }

        var editorId = GetEditorId();

        if (UseToolbarContributors)
        {
            _toolbarItems = await ToolbarService.GetToolbarItemsAsync(
                editorId,
                includeDefaults: IncludeDefaultToolbarItems,
                includeContributors: true,
                scope: ToolbarScope);
        }
        else if (ToolbarItems != null)
        {
            _toolbarItems = ToolbarItems.ToList();
        }
        else
        {
            _toolbarItems = await ToolbarService.GetToolbarItemsAsync(editorId, includeDefaults: true, includeContributors: false);
        }
    }

    private async Task InitializeEditorAsync()
    {
        if (_disposed || _interop == null)
        {
            return;
        }

        try
        {
            _dotNetRef ??= DotNetObjectReference.Create(this);

            if (IsDiffReview)
            {
                _editorId = await _interop.InitDiffReviewAsync(_diffContainer, _dotNetRef, new SbMarkdownDiffInitOptions
                {
                    EditorId = _elementId,
                    Original = OriginalValue,
                    Modified = SuggestedValue,
                    ReadOnly = ReadOnly,
                    EditorMode = EditorMode == SbMarkdownEditorMode.Source ? "source" : "markdown",
                    SourceLanguage = SourceLanguage
                });
            }
            else
            {
                await _interop.EnsureAssetsAsync(new SbMarkdownAssetOptions
                {
                    EnableMermaid = EnableMermaid,
                    EnableHighlight = EnableHighlight,
                    HighlightTheme = HighlightTheme
                });

                if (_disposed)
                {
                    return;
                }

                _editorId = await _interop.InitEditorAsync(_textarea, _dotNetRef, new SbMarkdownEditorInitOptions
                {
                    EditorId = GetEditorId(),
                    Value = Value,
                    Placeholder = Placeholder,
                    ReadOnly = ReadOnly,
                    Direction = RightToLeft ? "rtl" : "ltr",
                    EditorMode = EditorMode == SbMarkdownEditorMode.Source ? "source" : "markdown",
                    SourceLanguage = SourceLanguage,
                    EnablePreview = EnablePreview && EditorMode != SbMarkdownEditorMode.Source,
                    EnableMermaid = EnableMermaid,
                    EnableHighlight = EnableHighlight,
                    HighlightTheme = HighlightTheme,
                    MinHeight = MinHeight,
                    MaxHeight = MaxHeight
                });
            }

            if (_disposed)
            {
                return;
            }

            IsReady = !string.IsNullOrEmpty(_editorId);
            _useFallback = !IsReady;
        }
        catch (JSException)
        {
            IsReady = false;
            _useFallback = true;
        }
    }

    private async Task ReinitializeEditorAsync()
    {
        if (_disposed)
        {
            return;
        }

        if (_editorId != null && _interop != null)
        {
            await _interop.DestroyEditorAsync(_editorId);
            _editorId = null;
        }

        IsReady = false;
        _useFallback = false;
        await InvokeAsync(StateHasChanged);
        if (_disposed)
        {
            return;
        }

        await LoadToolbarItemsAsync();
        await InitializeEditorAsync();
        if (_disposed)
        {
            return;
        }

        _lastRenderedDiffMode = IsDiffReview;
        _lastRenderedValue = Value;
        _lastRenderedOriginal = OriginalValue;
        _lastRenderedSuggested = SuggestedValue;
        _lastEditorMode = EditorMode;
        _lastSourceLanguage = SourceLanguage;
        _lastIncludeDefaultToolbarItems = IncludeDefaultToolbarItems;
        _lastToolbarScope = ToolbarScope;
        await InvokeAsync(StateHasChanged);
    }

    private string GetEditorId()
    {
        if (string.Equals(SourceLanguage, "html", StringComparison.OrdinalIgnoreCase))
        {
            return $"{_elementId}-html";
        }

        if (string.Equals(SourceLanguage, "json", StringComparison.OrdinalIgnoreCase))
        {
            return $"{_elementId}-json";
        }

        return _elementId;
    }

    [JSInvokable]
    public async Task OnEditorChangeAsync(string value, string html)
    {
        _lastRenderedValue = value;
        await ValueChanged.InvokeAsync(value);
        if (ValueHtmlChanged.HasDelegate)
        {
            await ValueHtmlChanged.InvokeAsync(html);
        }
    }

    [JSInvokable]
    public Task OnDiffModifiedChangedAsync(string value)
    {
        _lastRenderedSuggested = value;
        return SuggestedValueChanged.InvokeAsync(value);
    }

    [JSInvokable]
    public Task NotifyShortcutAsync(string shortcut)
    {
        return OnShortcut.InvokeAsync(shortcut);
    }

    public async Task<string> GetValueAsync()
    {
        if (_editorId != null && _interop != null && IsReady)
        {
            return await _interop.GetValueAsync(_editorId);
        }

        return IsDiffReview ? SuggestedValue : Value;
    }

    public async Task SetValueAsync(string value)
    {
        if (_editorId != null && _interop != null && IsReady)
        {
            await _interop.SetValueAsync(_editorId, value);
        }

        _lastRenderedValue = value;
        await ValueChanged.InvokeAsync(value);
    }

    public async Task InsertTextAsync(string text)
    {
        if (_editorId != null && _interop != null && IsReady && !IsDiffReview)
        {
            await _interop.InsertTextAtCursorAsync(_editorId, text);
        }
    }

    public async Task TogglePreviewAsync()
    {
        if (_editorId != null && _interop != null && IsReady && !IsDiffReview)
        {
            await _interop.TogglePreviewAsync(_editorId);
        }
    }

    public async Task SetPreviewAsync(bool show)
    {
        if (_editorId != null && _interop != null && IsReady && !IsDiffReview)
        {
            await _interop.SetPreviewAsync(_editorId, show);
        }
    }

    private string GetBoundValue() => IsDiffReview ? SuggestedValue : Value;

    private Task OnFallbackValueChangedAsync(string? value)
    {
        var text = value ?? string.Empty;
        if (IsDiffReview)
        {
            return SuggestedValueChanged.InvokeAsync(text);
        }

        return ValueChanged.InvokeAsync(text);
    }

    private string GetContainerClass()
    {
        var classes = new List<string> { "sb-markdown-editor" };
        if (IsDiffReview)
        {
            classes.Add("sb-markdown-editor--diff");
        }

        if (EditorMode == SbMarkdownEditorMode.Source)
        {
            classes.Add("sb-markdown-editor--source");
        }

        if (string.Equals(SourceLanguage, "json", StringComparison.OrdinalIgnoreCase))
        {
            classes.Add("sb-markdown-editor--json");
        }

        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes.Add(Class);
        }

        return string.Join(' ', classes);
    }

    private static bool IsCustomToolbarAction(string action) =>
        action is "format-json" or "minify-json";

    private static bool IsToolbarItemDisabled(SbMarkdownToolbarItem item)
    {
        if (item is MdToolbarContributedItem contributed)
        {
            return contributed.IsEnabled?.Invoke() == false;
        }

        return false;
    }

    private async Task OnToolbarClickAsync(SbMarkdownToolbarItem item)
    {
        if (item is MdToolbarContributedItem contributedItem && contributedItem.OnClickAsync != null)
        {
            await ToolbarService.ExecuteItemActionAsync(contributedItem, CreateActionContext());
            return;
        }

        if (!string.IsNullOrEmpty(item.Action) &&
            IsCustomToolbarAction(item.Action) &&
            OnToolbarCustomAction.HasDelegate)
        {
            await OnToolbarCustomAction.InvokeAsync(item.Action);
            return;
        }

        if (_editorId == null || _interop == null || string.IsNullOrEmpty(item.Action))
        {
            return;
        }

        await _interop.ExecActionAsync(_editorId, item.Action, item.ActionValue);
    }

    private MdToolbarActionContext CreateActionContext()
    {
        return new MdToolbarActionContext(_editorId ?? _elementId, ServiceProvider)
        {
            InsertTextAsync = InsertTextAsync,
            InsertImageMarkdownAsync = async (url, alt) =>
                await InsertTextAsync($"![{alt ?? "image"}]({url})"),
            InsertLinkMarkdownAsync = async (url, text) =>
                await InsertTextAsync($"[{text ?? url}]({url})"),
            GetSelectionAsync = async () =>
            {
                if (_editorId == null || _interop == null)
                {
                    return null;
                }

                return await _interop.GetValueAsync(_editorId);
            },
            GetValueAsync = async () =>
            {
                if (_editorId == null || _interop == null)
                {
                    return Value;
                }

                return await _interop.GetValueAsync(_editorId);
            }
        };
    }

    public async ValueTask DisposeAsync()
    {
        _disposed = true;

        if (_editorId != null && _interop != null)
        {
            try
            {
                await _interop.DestroyEditorAsync(_editorId);
            }
            catch (JSDisconnectedException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SbMarkdownEditor] DestroyEditorAsync JSDisconnected: {ex.Message}");
            }
            catch (ObjectDisposedException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SbMarkdownEditor] DestroyEditorAsync ObjectDisposed: {ex.Message}");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SbMarkdownEditor] DestroyEditorAsync error: {ex}");
            }
        }

        if (_interop != null)
        {
            try
            {
                await _interop.DisposeAsync();
            }
            catch (JSDisconnectedException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SbMarkdownEditor] Interop DisposeAsync JSDisconnected: {ex.Message}");
            }
            catch (ObjectDisposedException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SbMarkdownEditor] Interop DisposeAsync ObjectDisposed: {ex.Message}");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SbMarkdownEditor] Interop DisposeAsync error: {ex}");
            }
        }

        _dotNetRef?.Dispose();
    }
}
