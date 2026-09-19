namespace SufiChain.SufiBlazor.Contracts.Editors;

/// <summary>
/// Optional host-registered assistant for the in-document Ask AI menu.
/// SufiBlazor has no AI knowledge; modules supply the implementation.
/// </summary>
public interface ISbEditorAiAssistant
{
    Task<SbEditorAiResult> AssistAsync(SbEditorAiRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Request payload for <see cref="ISbEditorAiAssistant"/>.
/// </summary>
public sealed class SbEditorAiRequest
{
    public string Prompt { get; set; } = "";
    public string Document { get; set; } = "";
    public string? Selection { get; set; }
    public SbContentFormat ContentFormat { get; set; } = SbContentFormat.Markdown;
    public string? Culture { get; set; }
    public string? HooshvareKey { get; set; }
}

/// <summary>
/// Result payload for <see cref="ISbEditorAiAssistant"/>.
/// </summary>
public sealed class SbEditorAiResult
{
    public string? ReplacementText { get; set; }
    public bool ReplaceDocument { get; set; }
    public bool RequiresDiffReview { get; set; }
    public string? OriginalText { get; set; }
    public string? Message { get; set; }
    public List<SbEditorSuggestion> Suggestions { get; set; } = new();
}
