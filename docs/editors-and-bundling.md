# Editors and Bundling

`SbRichTextEditor` and `SbMarkdownEditor` depend on third-party JavaScript loaded at runtime.

## Vendor stack

| Feature | Libraries | SufiBlazor interop |
| --- | --- | --- |
| Rich text (Quill) | `quill.min.js`, `quill.snow.css` | `sufiblazor-editor.js`, `SbEditorInterop` |
| Markdown (EasyMDE) | `easymde.min.js`, `easymde.min.css`, marked.js, highlight.js, mermaid | `sufiblazor-markdown-editor.js`, `SbMarkdownEditorInterop` |
| Maps (Leaflet) | `vendor/leaflet/leaflet.js`, `leaflet.css`, marker images | `sufiblazor-map.js`, `SbMapInterop` |

Vendor files ship under `_content/SufiChain.SufiBlazor/vendor/`. Leaflet loads on demand when an `SbMap` (or map helper) is used, with CDN fallback.

## Sufi Platform (SufiTheme)

SufiTheme **does not** put editor vendors in the global bundle. They load **on demand** when a page uses an editor.

Registered in:

- `SufiThemeBlazorServerModule` — `BlazorSufiThemeBundles.SufiBlazor.Quill`, `BlazorSufiThemeBundles.SufiBlazor.MarkdownEditor`
- `SufiThemeBlazorWebAssemblyBundlingModule` — same bundle names for WASM

Global bundle (always loaded) includes only:

- `sufiblazor.css` / `sufiblazor.js`
- `sufi-theme.css` / `sufi-theme-viewport.js`

## Standalone Blazor apps

Without SufiTheme, you must:

1. Call `AddSufiBlazor()` (registers `IRteToolbarService`, `IMdToolbarService`, localization).
2. Include `sufiblazor.js` globally.
3. Load Quill/EasyMDE assets on pages that use editors (or add your own bundling).

Example (page-level, simplified):

```html
<link href="_content/SufiChain.SufiBlazor/vendor/quill.snow.css" rel="stylesheet" />
<script src="_content/SufiChain.SufiBlazor/vendor/quill.min.js"></script>
```

For markdown, also load EasyMDE, marked, and optional highlight/mermaid per [SbMarkdownEditor](components/forms/SbMarkdownEditor.md).

## Toolbar services

Editors support contributor pipelines:

- `AddRteToolbarContributor<T>()` — rich text
- `AddMdToolbarContributor<T>()` — markdown

SufiTheme adds `FontFamilyToolbarContributor` (Persian fonts, RTL cultures only) via the Server module.

## Related

- [Standalone adoption](standalone-adoption.md)
- [Installation](installation.md)
- SufiTheme bundling: [SufiTheme configuration](../../sufi-theme/docs/configuration.md)
