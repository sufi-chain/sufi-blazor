using Microsoft.AspNetCore.Components;
using SufiChain.SufiBlazor.Contracts.Editors;

namespace SufiChain.SufiBlazor.Components.Forms;

public partial class SbMarkEditor
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
    [Parameter] public SbMarkEditorMode Mode { get; set; } = SbMarkEditorMode.Markdown;
    [Parameter] public string? SourceLanguage { get; set; }
    [Parameter] public bool UseToolbarContributors { get; set; }
    [Parameter] public bool IncludeDefaultToolbarItems { get; set; } = true;
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
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private SbMarkdownEditorMode ResolveEditorMode()
    {
        return Mode switch
        {
            SbMarkEditorMode.Source => SbMarkdownEditorMode.Source,
            SbMarkEditorMode.Markup => SbMarkdownEditorMode.Source,
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
            _ => null
        };
    }
}
