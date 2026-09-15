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

## ADR-002: Editor and map runtimes load on demand

**Status:** Accepted

**Context:** Document editors and maps ship large JavaScript. Loading them on every page increases initial weight for apps that never open an editor or map.

**Decision:**

- Editor TypeScript lives in repo-root `frontend/`. Vite emits ES modules into `src/SufiChain.SufiBlazor/wwwroot/_sufi/editors/{rich-text,code,diff,viewer}/`.
- C# interop imports those modules on first use (`./_content/SufiChain.SufiBlazor/_sufi/editors/*/index.js`).
- `sufiblazor.css` imports `sufiblazor-editors.css`. SufiTheme also registers that stylesheet on the global style bundle and on named bundles `SufiBlazor.RichText`, `SufiBlazor.Code`, `SufiBlazor.Diff`, and `SufiBlazor.Viewer`.
- Host `dotnet build` does not run Node. Commit the `wwwroot` emit. Leaflet still loads when an `SbMap` (or map helper) is first used, with CDN fallback.
- Standalone apps include `sufiblazor.css` and `sufiblazor.js`. Editor ES modules load from static web assets when a component mounts.

See [Editors and bundling](../editors-and-bundling.md).

## Related

- [Package map](package-map.md)
- [Standalone adoption](../standalone-adoption.md)
- [Demo host integration](../demo-host-integration.md)
