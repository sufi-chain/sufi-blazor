# Standalone SufiBlazor Adoption

Use this checklist when adding SufiBlazor to a **plain ASP.NET Core Blazor app** without Sufi Platform, ABP, or SufiTheme.

The core library (`SufiChain.SufiBlazor`) has no dependency on those stacks.

## Checklist

1. **Package** — `dotnet add package SufiChain.SufiBlazor`
2. **CSS** — `<link href="_content/SufiChain.SufiBlazor/sufiblazor.css" rel="stylesheet" />`
3. **JS** — `<script src="_content/SufiChain.SufiBlazor/sufiblazor.js"></script>` (dialogs, grid, popover). Document editors import `_sufi/editors/*/index.js` on demand.
4. **DI** — `builder.Services.AddSufiBlazor();` in `Program.cs`
5. **Theme** — wrap app in `SbThemeProvider` (light/dark + LTR/RTL)
6. **Namespaces** — `@using SufiChain.SufiBlazor.Components` (and sub-namespaces as needed)

See [Installation](installation.md) for full steps.

## What you get without SufiTheme

| Included | Not included |
| --- | --- |
| All `Sb*` components | App shell (sidebar, top bar, icon rail) |
| Design tokens + RTL | Platform menu/toolbar system |
| `SbDataGrid`, forms, overlays, document editors | SufiTheme named editor style bundles (optional) |
| Localization via `SufiBlazorResource` | Account layout chrome |

**App chrome** lives in **SufiTheme** (`SufiAppShell`, `SufiSidebar`, etc.). SufiBlazor only provides layout **primitives** (`SbStack`, `SbGrid`, `SbContainer`, `SbSpacer`). See [Architecture decisions](architecture/decisions.md).

## Document editors

`SbRichTextEditor`, `SbCodeEditor`, `SbDocumentDiffEditor`, and `SbDocumentViewer` import ES modules from `_content/SufiChain.SufiBlazor/_sufi/editors/` when they mount. `sufiblazor.css` already imports `sufiblazor-editors.css`.

See [Editors and bundling](editors-and-bundling.md).

## Optional toolbar contributors

```csharp
builder.Services.AddSufiBlazor();
builder.Services.AddEditorToolbarContributor<MyEditorContributor>();
```

## Related

- [Installation](installation.md)
- [Theming](theming.md)
- [Localization](localization.md)
- [Demo host integration](demo-host-integration.md) — how the platform demo differs from standalone use
