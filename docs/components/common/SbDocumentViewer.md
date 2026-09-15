# SbDocumentViewer

Read-only renderer for Markdown, HTML, or JSON. The runtime is the Tiptap static renderer. Optional highlight.js and mermaid load from the viewer bundle.

Use this component in chat timelines, previews, and documentation panes. Published HTML on the server still goes through `SufiChain.SufiPlatform.Content.Rendering`.

Asset loading: [Editors and bundling](../../editors-and-bundling.md).

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| Content | string? | null | Document text |
| Format | SbContentFormat | Markdown | `Markdown`, `Html`, or `Json` |
| EnableMermaid | bool | true | Render mermaid fences |
| EnableHighlight | bool | true | Syntax-highlight fenced code |
| RightToLeft | bool? | culture | Override direction |
| Class / Style | string? | — | Host chrome |

## CSS classes

- `sb-document-viewer`

## Example

```razor
<SbDocumentViewer Content="@message.Body"
                  Format="SbContentFormat.Markdown"
                  EnableMermaid="true"
                  EnableHighlight="true" />
```

## Related

- [Editors and bundling](../../editors-and-bundling.md)
- [SbRichTextEditor](../forms/SbRichTextEditor.md)
