using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SufiChain.SufiBlazor.Contracts.Editors;
using SufiChain.SufiBlazor.Interop;

namespace SufiChain.SufiBlazor.Components.Forms;

/// <summary>
/// Side-by-side document diff editor backed by CodeMirror 6 MergeView.
/// </summary>
public partial class SbDocumentDiffEditor : ComponentBase, IAsyncDisposable
{
    private ElementReference _editorContainer;
    private SbDiffEditorInterop? _interop;
    private DotNetObjectReference<SbDocumentDiffEditor>? _dotNetRef;
    private string? _editorId;
    private bool _useFallback;
    private bool _disposed;
    private string? _lastOriginal;
    private string? _lastSuggested;

    [Parameter] public string OriginalValue { get; set; } = "";
    [Parameter] public string SuggestedValue { get; set; } = "";
    [Parameter] public EventCallback<string> SuggestedValueChanged { get; set; }
    [Parameter] public SbCodeLanguage Language { get; set; } = SbCodeLanguage.Markdown;
    [Parameter] public bool ReadOnlySuggested { get; set; }
    [Parameter] public bool? RightToLeft { get; set; }
    [Parameter] public bool ShowApplyDiscard { get; set; }
    [Parameter] public EventCallback OnApply { get; set; }
    [Parameter] public EventCallback OnDiscard { get; set; }
    [Parameter] public string ApplyText { get; set; } = "Apply";
    [Parameter] public string DiscardText { get; set; } = "Discard";
    [Parameter] public string? MinHeight { get; set; } = "360px";
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter] public string AriaLabel { get; set; } = "Document diff editor";

    protected bool EffectiveRightToLeft =>
        RightToLeft ?? CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;

    protected override async Task OnParametersSetAsync()
    {
        if (_editorId != null && _interop != null &&
            (OriginalValue != _lastOriginal || SuggestedValue != _lastSuggested))
        {
            await _interop.SetValuesAsync(_editorId, OriginalValue ?? "", SuggestedValue ?? "");
            _lastOriginal = OriginalValue;
            _lastSuggested = SuggestedValue;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_disposed || !firstRender)
        {
            return;
        }

        _interop = new SbDiffEditorInterop(JSRuntime);
        _dotNetRef = DotNetObjectReference.Create(this);
        try
        {
            _editorId = await _interop.InitializeAsync(_editorContainer, _dotNetRef, new SbDiffEditorInitOptions
            {
                Original = OriginalValue,
                Suggested = SuggestedValue,
                Language = Language.ToString().ToLowerInvariant(),
                ReadOnlySuggested = ReadOnlySuggested,
                Direction = EffectiveRightToLeft ? "rtl" : "ltr"
            });
            _lastOriginal = OriginalValue;
            _lastSuggested = SuggestedValue;
        }
        catch (JSException)
        {
            _useFallback = true;
        }

        await InvokeAsync(StateHasChanged);
    }

    private string GetContainerClass()
    {
        var classes = new List<string> { "sb-diff-editor" };
        if (!string.IsNullOrWhiteSpace(Class)) classes.Add(Class);
        return string.Join(' ', classes);
    }

    private string GetContainerStyle()
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(MinHeight)) parts.Add($"min-height: {MinHeight}");
        if (!string.IsNullOrWhiteSpace(Style)) parts.Add(Style);
        return string.Join("; ", parts);
    }

    private Task OnFallbackSuggestedChangedAsync(string? value)
    {
        SuggestedValue = value ?? "";
        return SuggestedValueChanged.InvokeAsync(SuggestedValue);
    }

    [JSInvokable]
    public Task OnSuggestedChanged(string value)
    {
        SuggestedValue = value;
        _lastSuggested = value;
        return SuggestedValueChanged.InvokeAsync(value);
    }

    public async Task GoToNextChunkAsync()
    {
        if (_editorId != null && _interop != null)
        {
            await _interop.GoToChunkAsync(_editorId, "next");
        }
    }

    public async Task GoToPreviousChunkAsync()
    {
        if (_editorId != null && _interop != null)
        {
            await _interop.GoToChunkAsync(_editorId, "prev");
        }
    }

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
