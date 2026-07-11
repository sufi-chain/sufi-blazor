# Standalone SufiBlazor Adoption

Use this checklist when adding SufiBlazor to a **plain ASP.NET Core Blazor app** without Sufi Platform, ABP, or SufiTheme.

The core library (`SufiChain.SufiBlazor`) has no dependency on those stacks.

## Checklist

1. **Package** — `dotnet add package SufiChain.SufiBlazor`
2. **CSS** — `<link href="_content/SufiChain.SufiBlazor/sufiblazor.css" rel="stylesheet" />`
3. **JS** — `<script src="_content/SufiChain.SufiBlazor/sufiblazor.js"></script>` (dialogs, grid, popover, editors interop)
4. **DI** — `builder.Services.AddSufiBlazor();` in `Program.cs`
5. **Theme** — wrap app in `SbThemeProvider` (light/dark + LTR/RTL)
6. **Namespaces** — `@using SufiChain.SufiBlazor.Components` (and sub-namespaces as needed)

See [Installation](installation.md) for full steps.

## What you get without SufiTheme

| Included | Not included |
| --- | --- |
| All `Sb*` components | App shell (sidebar, top bar, icon rail) |
| Design tokens + RTL | Platform menu/toolbar system |
| `SbDataGrid`, forms, overlays | SufiTheme bundling for editor vendors |
| Localization via `SufiBlazorResource` | Account layout chrome |

**App chrome** lives in **SufiTheme** (`SufiAppShell`, `SufiSidebar`, etc.). SufiBlazor only provides layout **primitives** (`SbStack`, `SbGrid`, `SbContainer`, `SbSpacer`). See [LAYOUT_REVIEW](components/layout/LAYOUT_REVIEW.md).

## Rich text and markdown editors

`SbRichTextEditor` and `SbMarkdownEditor` need vendor JavaScript (Quill, EasyMDE, marked.js, etc.).

- **Sufi Platform hosts:** SufiTheme Server/WASM modules register on-demand bundles — see [Editors and bundling](editors-and-bundling.md).
- **Standalone apps:** load vendor assets from `_content/SufiChain.SufiBlazor/vendor/` yourself or copy the bundling pattern from SufiTheme.

## Optional toolbar contributors

```csharp
builder.Services.AddSufiBlazor();
builder.Services.AddRteToolbarContributor<MyRteContributor>();
builder.Services.AddMdToolbarContributor<MyMdContributor>();
```

## Related

- [Installation](installation.md)
- [Theming](theming.md)
- [Localization](localization.md)
- [Demo host integration](demo-host-integration.md) — how the platform demo differs from standalone use
