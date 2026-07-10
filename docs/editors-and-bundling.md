# Editors and Bundling

`SbRichTextEditor` and `SbMarkdownEditor` depend on third-party JavaScript loaded at runtime.

## Vendor stack

| Editor | Libraries | SufiBlazor interop |
| --- | --- | --- |
| Rich text (Quill) | `quill.min.js`, `quill.snow.css` | `sufiblazor-editor.js`, `SbEditorInterop` |
| Markdown (EasyMDE) | `easymde.min.js`, `easymde.min.css`, marked.js, highlight.js, mermaid | `sufiblazor-markdown-editor.js`, `SbMarkdownEditorInterop` |

Vendor files ship under `_content/SufiChain.SufiBlazor/vendor/`.

## Sufi Platform (KomTheme)

KomTheme **does not** put editor vendors in the global bundle. They load **on demand** when a page uses an editor.

Registered in:

- `KomThemeBlazorServerModule` — `BlazorKomThemeBundles.SufiBlazor.Quill`, `BlazorKomThemeBundles.SufiBlazor.MarkdownEditor`
- `KomThemeBlazorWebAssemblyBundlingModule` — same bundle names for WASM

Global bundle (always loaded) includes only:

- `sufiblazor.css` / `sufiblazor.js`
- `kom-theme.css` / `kom-theme-viewport.js`

## Standalone Blazor apps

Without KomTheme, you must:

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

KomTheme adds `FontFamilyToolbarContributor` (Persian fonts, RTL cultures only) via the Server module.

## Related

- [Standalone adoption](standalone-adoption.md)
- [Installation](installation.md)
- KomTheme bundling: `sufi-abp/docs/kom-theme/configuration.md`
