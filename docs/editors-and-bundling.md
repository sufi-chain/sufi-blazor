# Editors and Bundling

SufiBlazor ships four document surfaces. Choose the surface that matches the job. Do not load extra vendor scripts in the host.

| Component | Job | Runtime |
| --- | --- | --- |
| `SbRichTextEditor` | Author Markdown or HTML in a WYSIWYG document | Tiptap 3 (`_sufi/editors/rich-text/index.js`) |
| `SbCodeEditor` | Edit source (HTML, JSON, Scriban, CSS, JavaScript, Markdown) | CodeMirror 6 (`_sufi/editors/code/index.js`) |
| `SbDocumentDiffEditor` | Review original vs suggested text | CodeMirror 6 MergeView (`_sufi/editors/diff/index.js`) |
| `SbDocumentViewer` | Render Markdown, HTML, or JSON read-only | Tiptap static renderer (`_sufi/editors/viewer/index.js`) |

## Choose a surface

| Need | Use |
| --- | --- |
| Article, CMS leaf, form rich text, markdown body | `SbRichTextEditor` with `Mode="SbEditorMode.Wysiwyg"` and `ContentFormat` `Markdown` or `Html` |
| Compact inspector field | `SbRichTextEditor` with `Layout="SbEditorLayout.Compact"` |
| Email HTML / Scriban / JSON source | `SbCodeEditor` with `Language` |
| Version compare, AI apply/discard | `SbDocumentDiffEditor` |
| Chat, preview, documentation pane | `SbDocumentViewer` |

Public HTML for Knowledge Base, CMS, chat, and email uses `SufiChain.SufiPlatform.Content.Rendering` on the server. Do not treat the Blazor viewer as the sanitizer for published content.

## Platform usage

| Product | Surface |
| --- | --- |
| HelpDesk Knowledge Base article editor | `SbRichTextEditor` (`ContentFormat=Markdown`, `ToolbarScope=kb-article`) with Edit/Preview via `SbDocumentViewer`; `SbDocumentDiffEditor` on versions |
| SufiCom message templates | `SbCodeEditor` for HTML/Scriban email; `SbRichTextEditor` Markdown for other channels |
| SufiCom Chat timeline | `SbDocumentViewer` for message bodies |
| SufiForms dynamic fields | Compact `SbRichTextEditor` for RichText/Html/Markdown; `SbCodeEditor` for Code |
| SufiCMS page-builder leaf `html` / `source` | Compact `SbRichTextEditor` |
| File Manager demos | `SbRichTextEditor` |
| SufiAI hooshvare diff review | `SbDocumentDiffEditor` (`HooshvareTimelineDiffReviewMessage`) |

## Register services

```csharp
builder.Services.AddSufiBlazor();
builder.Services.AddEditorToolbarContributor<MyEditorToolbarContributor>();
builder.Services.AddScoped<ISbEditorAiAssistant, MyEditorAiAssistant>();
```

`AddSufiBlazor()` registers `IEditorToolbarService`. Contributors implement `IEditorToolbarContributor`. Clicks receive `EditorActionContext` with `ISbEditorDocument`. Do not call Tiptap or CodeMirror APIs from C#.

Set `UseToolbarContributors="true"` and a `ToolbarScope` on the editor when the contributor is scoped (for example `kb-article` or `communication-template`).

Platform hooshvares register `IHooshvareEditorAssistant` in the owning Blazor module. `HooshvareSbEditorAiBridge` implements `ISbEditorAiAssistant`. Pass `AiHooshvareKey` on `SbRichTextEditor` so Ask AI routes to the matching hooshvare.

## Static assets

Committed Razor class library output:

- `src/SufiChain.SufiBlazor/wwwroot/_sufi/editors/{rich-text,code,diff,viewer}/index.js`
- `src/SufiChain.SufiBlazor/wwwroot/_sufi/editors/chunks/`
- `src/SufiChain.SufiBlazor/wwwroot/_sufi/editors/styles/`
- `src/SufiChain.SufiBlazor/wwwroot/sufiblazor-editors.css`

`sufiblazor.css` imports `sufiblazor-editors.css`. Interop loads each editor ES module on demand from `_content/SufiChain.SufiBlazor/_sufi/editors/*/index.js`. Hosts do not add those scripts to the global bundle.

SufiTheme registers `sufiblazor-editors.css` on the global style bundle and on named style bundles `SufiBlazor.RichText`, `SufiBlazor.Code`, `SufiBlazor.Diff`, and `SufiBlazor.Viewer`. Leaflet maps stay separate (`sufiblazor-map.js`).

## Standalone Blazor apps

1. Call `AddSufiBlazor()`.
2. Include `sufiblazor.css` and `sufiblazor.js`.
3. Place an editor on the page. The component imports its ES module.

Do not copy Node packages into the host `wwwroot`.

## Develop the editor runtimes

TypeScript lives at repo-root `frontend/` (outside the `.csproj` folder). Vite writes into the Razor class library `wwwroot`.

```bash
cd independent-projects/sufi-blazor/frontend
npm ci
npm test
npm run check
```

`npm run check` builds the four entries and concatenates chrome plus Vite CSS into `wwwroot/sufiblazor-editors.css`. Commit those `wwwroot` files. Do not commit `frontend/node_modules`.

The platform host (`hosts/SufiChane.SufiPlatform`) project-references `SufiChain.SufiBlazor`. `dotnet build` / publish copies the committed static web assets as `_content/SufiChain.SufiBlazor/...`. The host build does not run Node.

CI: `.github/workflows/editors.yml` runs `npm ci`, `npm test`, and `npm run check` when `frontend/` or the committed editor assets change.

## Related

- [SbRichTextEditor](components/forms/SbRichTextEditor.md)
- [SbCodeEditor](components/forms/SbCodeEditor.md)
- [SbDocumentDiffEditor](components/forms/SbDocumentDiffEditor.md)
- [SbDocumentViewer](components/common/SbDocumentViewer.md)
- [Standalone adoption](standalone-adoption.md)
- [Installation](installation.md)
