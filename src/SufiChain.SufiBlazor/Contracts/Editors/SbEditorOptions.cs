namespace SufiChain.SufiBlazor.Contracts.Editors;

/// <summary>
/// DI options for the unified editor toolbar and default feature set.
/// </summary>
public sealed class SbEditorOptions
{
    public List<Type> Contributors { get; } = new();

    public bool IncludeDefaultItems { get; set; } = true;

    public List<string> GroupOrder { get; set; } =
    [
        "history",
        "heading",
        "formatting",
        "list",
        "alignment",
        "insert",
        "blocks",
        "custom",
        "actions"
    ];

    public SbEditorFeatures Features { get; set; } = SbEditorFeatures.Default;

    /// <summary>
    /// Named HTML allow-list policy for <see cref="SbContentFormat.Html"/> parse.
    /// </summary>
    public string? AllowedHtml { get; set; }
}
