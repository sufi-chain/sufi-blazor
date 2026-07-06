using System.Text.Json;
using Microsoft.AspNetCore.Components;
using SufiChain.SufiBlazor.Contracts.Editors;

namespace SufiChain.SufiBlazor.Components.Forms;

/// <summary>
/// Unified markdown, markup/source, and JSON editor wrapper over <see cref="SbMarkdownEditor"/>.
/// </summary>
public partial class SbMarkEditor
{
    private SbMarkdownEditor? _editor;
    private bool _isJsonValid = true;
    private IReadOnlyList<SbMarkdownToolbarItem>? _resolvedToolbarItems;

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
    [Parameter] public SbMarkEditorMode Mode { get; set; } = SbMarkEditorMode.Markdown;
    [Parameter] public string? SourceLanguage { get; set; }
    [Parameter] public bool UseToolbarContributors { get; set; }
    [Parameter] public bool IncludeDefaultToolbarItems { get; set; } = true;
    /// <summary>
    /// Optional toolbar scope forwarded to <see cref="SbMarkdownEditor"/> to
    /// filter which registered <see cref="IMdToolbarContributor"/> instances run.
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
    /// <summary>
    /// When <see cref="Mode"/> is <see cref="SbMarkEditorMode.Json"/>, validates JSON on change.
    /// </summary>
    [Parameter] public bool ValidateJson { get; set; } = true;
    /// <summary>
    /// Fired when JSON validity changes (only when <see cref="ValidateJson"/> is enabled in JSON mode).
    /// </summary>
    [Parameter] public EventCallback<bool> JsonValidChanged { get; set; }
    /// <summary>
    /// Shows the built-in JSON toolbar (format action) when in JSON mode.
    /// </summary>
    [Parameter] public bool ShowJsonToolbar { get; set; } = true;
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Whether the current JSON value is valid. Always true outside JSON mode.
    /// </summary>
    public bool IsJsonValid => !IsJsonMode() || _isJsonValid;

    protected override void OnParametersSet()
    {
        _resolvedToolbarItems = ResolveToolbarItems();
        if (IsJsonMode())
        {
            UpdateJsonValidity(Value, notify: false);
        }
    }

    private bool IsJsonMode()
    {
        if (Mode == SbMarkEditorMode.Json)
        {
            return true;
        }

        return string.Equals(ResolveSourceLanguage(), "json", StringComparison.OrdinalIgnoreCase);
    }

    private SbMarkdownEditorMode ResolveEditorMode()
    {
        return Mode switch
        {
            SbMarkEditorMode.Source => SbMarkdownEditorMode.Source,
            SbMarkEditorMode.Markup => SbMarkdownEditorMode.Source,
            SbMarkEditorMode.Json => SbMarkdownEditorMode.Source,
            _ => SbMarkdownEditorMode.Markdown
        };
    }

    private string? ResolveSourceLanguage()
    {
        if (!string.IsNullOrWhiteSpace(SourceLanguage))
        {
            return SourceLanguage;
        }

        return Mode switch
        {
            SbMarkEditorMode.Markup => "html",
            SbMarkEditorMode.Source => "html",
            SbMarkEditorMode.Json => "json",
            _ => null
        };
    }

    private bool ResolveEnablePreview() => IsJsonMode() ? false : EnablePreview;

    private bool ResolveHideToolbar()
    {
        if (HideToolbar)
        {
            return true;
        }

        if (IsJsonMode())
        {
            return !ShowJsonToolbar || ToolbarItems != null || UseToolbarContributors;
        }

        return false;
    }

    private bool ResolveIncludeDefaultToolbarItems() =>
        IsJsonMode() ? false : IncludeDefaultToolbarItems;

    private string? ResolveClass()
    {
        var classes = new List<string>();
        if (IsJsonMode())
        {
            classes.Add("sb-markdown-editor--json");
            if (ValidateJson && !_isJsonValid && !string.IsNullOrWhiteSpace(Value))
            {
                classes.Add("sb-markdown-editor--json-invalid");
            }
        }

        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes.Add(Class);
        }

        return classes.Count == 0 ? null : string.Join(' ', classes);
    }

    private IReadOnlyList<SbMarkdownToolbarItem>? ResolveToolbarItems()
    {
        if (ToolbarItems != null)
        {
            return ToolbarItems;
        }

        if (!IsJsonMode() || !ShowJsonToolbar || HideToolbar || ReadOnly)
        {
            return null;
        }

        return
        [
            new SbMarkdownToolbarItem
            {
                Id = "format-json",
                IconName = "json",
                Tooltip = "Format JSON",
                Action = "format-json"
            }
        ];
    }

    private async Task OnValueChangedAsync(string value)
    {
        if (IsJsonMode())
        {
            await UpdateJsonValidityAsync(value);
        }

        await ValueChanged.InvokeAsync(value);
    }

    private async Task OnSuggestedValueChangedAsync(string value)
    {
        if (IsJsonMode())
        {
            await UpdateJsonValidityAsync(value);
        }

        await SuggestedValueChanged.InvokeAsync(value);
    }

    private async Task UpdateJsonValidityAsync(string value)
    {
        var wasValid = _isJsonValid;
        UpdateJsonValidity(value, notify: false);
        if (wasValid != _isJsonValid)
        {
            if (JsonValidChanged.HasDelegate)
            {
                await JsonValidChanged.InvokeAsync(_isJsonValid);
            }

            await InvokeAsync(StateHasChanged);
        }
    }

    private void UpdateJsonValidity(string value, bool notify)
    {
        if (!IsJsonMode() || !ValidateJson || string.IsNullOrWhiteSpace(value))
        {
            _isJsonValid = true;
            return;
        }

        try
        {
            using var _ = JsonDocument.Parse(value);
            _isJsonValid = true;
        }
        catch (JsonException)
        {
            _isJsonValid = false;
        }
    }

    private async Task OnToolbarCustomActionAsync(string action)
    {
        if (!IsJsonMode())
        {
            return;
        }

        if (action == "format-json")
        {
            await FormatJsonAsync();
        }
    }

    /// <summary>
    /// Pretty-prints the current JSON value when in JSON mode.
    /// </summary>
    public async Task FormatJsonAsync()
    {
        if (!IsJsonMode() || _editor == null)
        {
            return;
        }

        var current = IsDiffReview
            ? SuggestedValue
            : await _editor.GetValueAsync();

        if (string.IsNullOrWhiteSpace(current))
        {
            return;
        }

        try
        {
            using var document = JsonDocument.Parse(current);
            var formatted = JsonSerializer.Serialize(document, new JsonSerializerOptions { WriteIndented = true });
            await _editor.SetValueAsync(formatted);
            if (IsDiffReview)
            {
                await OnSuggestedValueChangedAsync(formatted);
            }
        }
        catch (JsonException)
        {
            // Keep invalid JSON as-is so the user can fix it manually.
        }
    }

    /// <summary>
    /// Gets the current editor value.
    /// </summary>
    public Task<string> GetValueAsync() =>
        _editor?.GetValueAsync() ?? Task.FromResult(IsDiffReview ? SuggestedValue : Value);

    /// <summary>
    /// Sets the current editor value.
    /// </summary>
    public async Task SetValueAsync(string value)
    {
        if (_editor != null)
        {
            await _editor.SetValueAsync(value);
        }

        if (IsDiffReview)
        {
            await OnSuggestedValueChangedAsync(value);
        }
        else
        {
            await OnValueChangedAsync(value);
        }
    }
}
