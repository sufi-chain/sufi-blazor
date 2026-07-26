# Architecture Decisions

Recorded decisions for SufiBlazor boundaries and asset loading.

## ADR-001: App shell lives in SufiTheme, not SufiBlazor

**Status:** Accepted

**Context:** SufiBlazor originally included layout shell components (`SbAppShell`, `SbDualSidebar`, `SbTopBar`, `SbSidebar`, `SbIconRail`, `SbExpandPanel`). SufiTheme ships its own shell (`SufiAppShell`, `SufiDualSidebar`, `SufiTopBar`, etc.) and never used the Sb variants.

**Decision:** Remove shell chrome from SufiBlazor. Keep only layout **primitives** (`SbStack`, `SbGrid`, `SbContainer`, `SbSpacer`) for page composition inside any host shell.

**Consequences:**

- Host applications use **SufiTheme** for sidebar, top bar, icon rail, and dual-sidebar layouts.
- SufiBlazor stays framework-neutral and smaller; no duplicate shell APIs.
- Standalone adopters build their own chrome or add SufiTheme separately.

### Component disposition (historical review)

| Component | Used in SufiTheme? | Verdict |
|-----------|-------------------|---------|
| **SbContainer**, **SbContainerMaxWidth** | Yes — `TopMenuLayout.razor` | **Keep** |
| **SbStack**, **SbAlign**, **SbJustify**, **SbStackDirection** | Yes — layout class names | **Keep** |
| **SbGrid**, **SbGridItem**, **SbSpacer** | Demo + design-system use | **Keep** |
| **SbAppShell**, **SbAppShellVariant** | No | **Removed** — use `SufiAppShell` |
| **SbDualSidebar** | No | **Removed** — use `SufiDualSidebar` |
| **SbTopBar** | No | **Removed** — use `SufiTopBar` |
| **SbSidebar** | No | **Removed** — use `SufiSidebar` |
| **SbIconRail** | No | **Removed** — use `SufiIconRail` |
| **SbExpandPanel** | No | **Removed** — use `SufiExpandPanel` |

### SufiTheme layout usage

- **DualSidebarLayout** — `SufiAppShell`, `SufiDualSidebar`, `SufiIconRail`, `SufiExpandPanel`, `SufiTopBar`
- **SideMenuLayout** — `SufiAppShell`, `SufiSidebar`, `SufiTopBar`
- **TopMenuLayout** — `SufiTopBar`, **SbContainer** (only Sb layout component), `SbDrawer`, `SbNavMenu`, etc.

## ADR-002: Editor and map vendors load on demand

**Status:** Accepted

**Context:** Rich text, markdown, and map components depend on third-party JavaScript (Quill, EasyMDE, Leaflet). Loading all vendors globally increases initial page weight for apps that never use editors or maps.

**Decision:**

- Ship vendor files under `_content/SufiChain.SufiBlazor/vendor/`.
- **SufiTheme** registers on-demand bundles (`BlazorSufiThemeBundles.SufiBlazor.Quill`, `BlazorSufiThemeBundles.SufiBlazor.MarkdownEditor`) in Server and WASM modules. The global bundle includes only `sufiblazor.css` / `sufiblazor.js` and theme assets.
- **Leaflet** loads when an `SbMap` (or map helper) is first used, with CDN fallback.
- **Standalone apps** must include `sufiblazor.js` globally and load Quill/EasyMDE/Leaflet on pages that need them (or replicate SufiTheme bundling).

See [Editors and bundling](../editors-and-bundling.md) and [SufiTheme configuration](../../sufi-theme/docs/configuration.md).

## Related

- [Package map](package-map.md)
- [Standalone adoption](../standalone-adoption.md)
- [Demo host integration](../demo-host-integration.md)
