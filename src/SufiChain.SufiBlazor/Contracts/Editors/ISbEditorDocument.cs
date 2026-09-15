namespace SufiChain.SufiBlazor.Contracts.Editors;

/// <summary>
/// Engine-agnostic document operations for toolbar contributors and AI.
/// </summary>
public interface ISbEditorDocument
{
    Task<string> GetDocumentAsync(SbContentFormat format);
    Task SetDocumentAsync(string text, SbContentFormat format);
    Task<SbEditorSelection> GetSelectionAsync();
    Task ReplaceSelectionAsync(string text, SbContentFormat format);
    Task ReplaceDocumentAsync(string text, SbContentFormat format);
    Task InsertContentAsync(string text, SbContentFormat format);
    Task InsertLinkAsync(string url, string? text = null, string? target = null, string? rel = null);
    Task InsertImageAsync(string url, string? alt = null, string? width = null, string? height = null);
    Task InsertFileAsync(string url, string name, string? mime = null);
    Task InsertMentionAsync(string id, string label);
    Task InsertNodeAsync(string nodeName, IReadOnlyDictionary<string, object?>? attrs = null);
    Task ApplyMarkAsync(string mark, IReadOnlyDictionary<string, object?>? attrs = null);
    Task ApplyBlockAsync(string block, IReadOnlyDictionary<string, object?>? attrs = null);
    Task ExecuteCommandAsync(SbEditorCommand command);
    Task ShowSuggestionsAsync(IReadOnlyList<SbEditorSuggestion> suggestions);
    Task AcceptSuggestionAsync(string id);
    Task RejectSuggestionAsync(string id);
    Task AcceptAllSuggestionsAsync();
    Task RejectAllSuggestionsAsync();
    Task StreamInsertAsync(string chunk);
    Task FocusAsync();
}
