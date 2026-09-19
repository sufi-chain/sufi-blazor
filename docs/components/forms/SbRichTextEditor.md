# SbRichTextEditor

Canonical WYSIWYG document editor. The runtime is Tiptap 3. Serialized content is Markdown or HTML.

Use this component for articles, CMS leaves, form rich-text fields, and other authored documents. Use [SbCodeEditor](./SbCodeEditor.md) for raw source. Use [SbDocumentViewer](../common/SbDocumentViewer.md) for read-only rendering.

Asset loading and toolbar registration: [Editors and bundling](../../editors-and-bundling.md).

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| Value | string? | null | Document text (Markdown or HTML per `ContentFormat`) |
| ValueChanged | EventCallback\<string?\> | — | Raised when the document changes |
| Mode | SbEditorMode | Wysiwyg | `Wysiwyg` or `ReadOnly`. `Markdown` forces Markdown format |
| ContentFormat | SbContentFormat | Html | `Markdown` or `Html` for get/set/insert |
| Layout | SbEditorLayout | Document | `Compact` for inspectors and form fields |
| Features | SbEditorFeatures | Default | Bubble menu, slash menu, tables, task lists, mermaid, Ask AI |
| ToolbarItems | IReadOnlyList\<SbEditorToolbarItem\>? | null | Explicit toolbar; otherwise defaults |
| UseToolbarContributors | bool | false | Merge `IEditorToolbarContributor` items |
| IncludeDefaultToolbarItems | bool | true | Keep built-in items when contributors run |
| ToolbarScope | string? | null | Filter contributors (example: `kb-article`) |
| HideToolbar | bool | false | Hide the top toolbar |
| SourceEditorEnabled | bool | false | Reserved source-toggle flag |
| PasteCleanupOptions | SbPasteCleanupOptions? | null | Paste sanitizer options |
| OnImageUpload | Func\<IBrowserFile, Task\<SbImageUploadResult\>\>? | null | Upload handler for images |
| Placeholder | string? | null | Empty-state text |
| ReadOnly | bool | false | Read-only chrome |
| Disabled | bool | false | Disabled chrome |
| RightToLeft | bool? | culture | Override direction |
| Height / MinHeight / MaxHeight | string? | — | Size |
| ShowCharacterCount / ShowWordCount | bool | false | Footer counts |
| AiHooshvareKey | string? | null | Hooshvare key for Ask AI (`ISbEditorAiAssistant`) |

Dialog and ARIA strings (`LinkDialogTitle`, `EditorAriaLabel`, `CancelText`, `ApplyText`) fall back to localized defaults when unset.

## Events

| Event | Type | Description |
|-------|------|-------------|
| ValueChanged | EventCallback\<string?\> | Document text changed |
| OnChange | EventCallback | Raised after a content change |
| OnFocus / OnBlur | EventCallback | Focus |
| OnShortcut | EventCallback\<string\> | Named shortcut from the runtime |

## Document API (`ISbEditorDocument`)

Hold an `@ref` and call:

- `GetDocumentAsync` / `SetDocumentAsync`
- `InsertContentAsync` / `ReplaceSelectionAsync` / `ReplaceDocumentAsync`
- `InsertLinkAsync` / `InsertImageAsync` / `InsertFileAsync`
- `ExecuteCommandAsync`
- `ShowSuggestionsAsync` / `AcceptSuggestionAsync` / `RejectSuggestionAsync`
- `FocusAsync`

Inline suggestions use ProseMirror decorations (strikethrough original, green proposed text) plus `SbEditorAiReviewBar` Accept / Reject / Dismiss. The runtime is SufiBlazor's own Tiptap plugin, not Tiptap AI Toolkit.

## CSS classes

- `sb-editor` / `sb-editor--tiptap`
- `sb-editor--compact`, `sb-editor--rtl`, `sb-editor--readonly`, `sb-editor--disabled`

## Examples

### HTML document

```razor
<SbRichTextEditor @bind-Value="content" ContentFormat="SbContentFormat.Html" />
```

### Markdown article

```razor
<SbRichTextEditor @ref="_editor"
                  @bind-Value="body"
                  Mode="SbEditorMode.Wysiwyg"
                  ContentFormat="SbContentFormat.Markdown"
                  UseToolbarContributors="true"
                  ToolbarScope="kb-article"
                  AiHooshvareKey="HelpDeskKbArticleEditor"
                  MinHeight="360px" />
```

### Compact field

```razor
<SbRichTextEditor @bind-Value="html"
                  Layout="SbEditorLayout.Compact"
                  ContentFormat="SbContentFormat.Html"
                  HideToolbar="false" />
```

### Insert from code

```csharp
await _editor.InsertContentAsync("{{ Name }}", SbContentFormat.Markdown);
```

## Related

- [Editors and bundling](../../editors-and-bundling.md)
- [SbCodeEditor](./SbCodeEditor.md)
- [SbDocumentDiffEditor](./SbDocumentDiffEditor.md)
- [SbDocumentViewer](../common/SbDocumentViewer.md)
