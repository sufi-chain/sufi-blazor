# SbDocumentDiffEditor

Side-by-side document compare. The runtime is CodeMirror 6 MergeView.

Use it for article versions, copilot apply/discard, and other original-vs-suggested reviews.

Asset loading: [Editors and bundling](../../editors-and-bundling.md).

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| OriginalValue | string | `""` | Left / original text |
| SuggestedValue | string | `""` | Right / suggested text |
| SuggestedValueChanged | EventCallback\<string\> | — | Suggested text edited |
| Language | SbCodeLanguage | Markdown | Syntax mode |
| ReadOnlySuggested | bool | false | Lock the suggested pane |
| ShowApplyDiscard | bool | false | Show Apply / Discard actions |
| OnApply / OnDiscard | EventCallback | — | Action handlers |
| ApplyText / DiscardText | string | Apply / Discard | Action labels |
| RightToLeft | bool? | culture | Override direction |
| MinHeight | string? | `360px` | Size |
| AriaLabel | string | Document diff editor | Accessible name |

## Methods

- `GoToNextChunkAsync`
- `GoToPreviousChunkAsync`

## Example

```razor
<SbDocumentDiffEditor OriginalValue="@published"
                      SuggestedValue="@draft"
                      SuggestedValueChanged="OnDraftChanged"
                      Language="SbCodeLanguage.Markdown"
                      ShowApplyDiscard="true"
                      OnApply="ApplyDraftAsync"
                      OnDiscard="DiscardDraftAsync" />
```

## Related

- [Editors and bundling](../../editors-and-bundling.md)
- [SbRichTextEditor](./SbRichTextEditor.md)
- [SbCodeEditor](./SbCodeEditor.md)
