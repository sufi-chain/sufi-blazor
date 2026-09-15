# SbCodeEditor

CodeMirror 6 source editor. Use it for HTML, JSON, Scriban, CSS, JavaScript, TypeScript, XML, Markdown, or plain text.

Use [SbRichTextEditor](./SbRichTextEditor.md) when the user should edit a formatted document. Use [SbDocumentDiffEditor](./SbDocumentDiffEditor.md) for side-by-side review.

Asset loading: [Editors and bundling](../../editors-and-bundling.md).

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| Value | string | `""` | Source text |
| ValueChanged | EventCallback\<string\> | — | Raised when the text changes |
| Language | SbCodeLanguage | PlainText | Syntax mode |
| ReadOnly | bool | false | Read-only |
| Disabled | bool | false | Disabled |
| RightToLeft | bool? | culture | Override direction |
| LineNumbers | bool | true | Gutter line numbers |
| WordWrap | bool | false | Soft wrap |
| Placeholder | string? | null | Empty-state text |
| MinHeight / MaxHeight | string? | `200px` / null | Size |
| HideToolbar | bool | true | Hide contributor toolbar |
| UseToolbarContributors | bool | false | Load items for `SbEditorSurface.Code` |
| ToolbarScope | string? | null | Contributor scope |
| ValidateJson | bool | false | JSON validity when `Language=Json` |
| JsonValidChanged | EventCallback\<bool\> | — | Validity changed |
| OnShortcut | EventCallback\<string\> | — | Named shortcut |

## Methods

- `GetValueAsync` / `SetValueAsync`
- `InsertTextAsync`
- `FormatAsync` (JSON)
- `RefreshJsonValidityAsync`
- `FocusAsync`

The component also implements `ISbEditorDocument` so toolbar contributors can insert text through the same contract as rich text.

## Example

```razor
<SbCodeEditor @ref="_code"
              @bind-Value="html"
              Language="SbCodeLanguage.Html"
              UseToolbarContributors="true"
              ToolbarScope="communication-template"
              HideToolbar="false"
              MinHeight="280px" />
```

```csharp
await _code.InsertTextAsync("{{ UserName }}");
```

## Related

- [Editors and bundling](../../editors-and-bundling.md)
- [SbRichTextEditor](./SbRichTextEditor.md)
- [SbDocumentDiffEditor](./SbDocumentDiffEditor.md)
