namespace SufiChain.SufiBlazor.Contracts.Editors;

/// <summary>
/// Snapshot of the editor selection and document, pushed from JS on change.
/// </summary>
public sealed class EditorState
{
    public string EditorId { get; set; } = "";
    public bool CanUndo { get; set; }
    public bool CanRedo { get; set; }
    public bool IsEmpty { get; set; }
    public string? SelectionText { get; set; }
    public int SelectionFrom { get; set; }
    public int SelectionTo { get; set; }
    public string? ActiveNode { get; set; }
    public int HeadingLevel { get; set; }
    public string ContentFormat { get; set; } = nameof(SbContentFormat.Html);
    public int CharacterCount { get; set; }
    public int WordCount { get; set; }
    public List<string> ActiveMarks { get; set; } = new();
    public Dictionary<string, string?> ActiveMarkAttrs { get; set; } = new();
    public Dictionary<string, string?> ActiveNodeAttrs { get; set; } = new();

    public bool IsMarkActive(string mark) =>
        ActiveMarks.Contains(mark, StringComparer.OrdinalIgnoreCase);

    public bool IsNodeActive(string node) =>
        string.Equals(ActiveNode, node, StringComparison.OrdinalIgnoreCase);

    public bool IsAlignActive(string align) =>
        string.Equals(ActiveNodeAttrs.GetValueOrDefault("textAlign"), align, StringComparison.OrdinalIgnoreCase);
}

/// <summary>
/// Selection range plus surrounding node metadata.
/// </summary>
public sealed class SbEditorSelection
{
    public string Text { get; set; } = "";
    public int From { get; set; }
    public int To { get; set; }
    public string? NodeType { get; set; }
}

/// <summary>
/// In-document AI suggestion range.
/// </summary>
public sealed class SbEditorSuggestion
{
    public string Id { get; set; } = "";
    public string Kind { get; set; } = "replace";
    public int From { get; set; }
    public int To { get; set; }
    public string? InsertText { get; set; }
    public string? DeleteText { get; set; }
}

/// <summary>
/// Viewport-relative overlay box for an in-document suggestion.
/// </summary>
public sealed class SbEditorSuggestionRect
{
    public double Top { get; set; }
    public double Left { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
}
