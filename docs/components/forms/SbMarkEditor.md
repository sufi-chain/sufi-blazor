# SbMarkEditor

Unified markdown, markup/source, and JSON editor over `SbMarkdownEditor`. Switches toolbar, preview, and validation behavior by `Mode`.

## Modes

| `SbMarkEditorMode` | Behavior |
|--------------------|----------|
| `Markdown` | WYSIWYG markdown with optional preview (default) |
| `Markup` / `Source` | Source editor; default language `html` |
| `Json` | Source editor with JSON validation and optional format action |

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| Value | string | `""` | Editor content (two-way) |
| ValueHtml | string? | null | Rendered HTML output (markdown mode) |
| Mode | SbMarkEditorMode | Markdown | Editor mode |
| SourceLanguage | string? | null | Syntax language override for source modes |
| ReadOnly / Disabled | bool | false | Editor state |
| Placeholder | string? | null | Empty-state hint |
| RightToLeft | bool | false | RTL editing |
| EnablePreview | bool | true | Markdown preview pane |
| EnableMermaid / EnableHighlight | bool | true | Markdown extras |
| HighlightTheme | string | `"github"` | Code highlight theme |
| HideToolbar | bool | false | Hide toolbar entirely |
| UseToolbarContributors | bool | false | Use registered `IMdToolbarContributor` pipeline |
| IncludeDefaultToolbarItems | bool | true | Default markdown toolbar buttons |
| ToolbarScope | string? | null | Filter contributor scope |
| ToolbarItems | IReadOnlyList\<SbMarkdownToolbarItem\>? | null | Explicit toolbar items |
| ValidateJson | bool | true | Parse-check JSON in JSON mode |
| JsonValidChanged | EventCallback\<bool\> | — | Fired when JSON validity changes |
| ShowJsonToolbar | bool | true | Built-in format-json button in JSON mode |
| IsDiffReview | bool | false | Side-by-side review UI |
| OriginalValue / SuggestedValue | string | `""` | Diff-review content |
| OnApplyChanges / OnDiscardChanges | EventCallback | — | Diff-review actions |
| MinHeight / MaxHeight | string? | `"200px"` / null | Editor sizing |
| FallbackRows | int | 12 | Textarea fallback rows |
| OnShortcut | EventCallback\<string\> | — | Keyboard shortcut hook |
| Class / Style | string? | null | Root styling |

## Public methods

- `IsJsonValid` — current JSON validity (always true outside JSON mode)
- `FormatJsonAsync()` — pretty-print JSON in JSON mode
- `GetValueAsync()` / `SetValueAsync(string)` — programmatic access

## Example

```razor
<SbMarkEditor @bind-Value="configJson"
              Mode="SbMarkEditorMode.Json"
              ValidateJson="true"
              JsonValidChanged="OnJsonValidChanged"
              MinHeight="320px" />
```

## See also

- [SbMarkdownEditor](./SbMarkdownEditor.md) — underlying editor
- [Editors and bundling](../../editors-and-bundling.md) — vendor asset loading
