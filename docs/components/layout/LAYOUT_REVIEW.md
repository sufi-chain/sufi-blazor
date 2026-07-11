# SufiBlazor Layout Components – Review vs SufiTheme

## Summary

The **SufiTheme uses its own layout components** (`SufiAppShell`, `SufiDualSidebar`, `SufiTopBar`, `SufiSidebar`, `SufiIconRail`, `SufiExpandPanel`) in `SufiChain.SufiTheme.Blazor/Components/Layout/`. The SufiBlazor design system shell components (`SbAppShell`, `SbDualSidebar`, `SbTopBar`, `SbSidebar`, `SbIconRail`, `SbExpandPanel`) are **not used** in any application or in the SufiTheme.

## Usage by Component

| Component | Used in SufiTheme? | Used elsewhere? | Verdict |
|-----------|---------------------|-----------------|---------|
| **SbContainer** | Yes – `TopMenuLayout.razor` | SufiBlazorDemo | **KEEP** |
| **SbContainerMaxWidth** | Yes – with SbContainer | SufiBlazorDemo | **KEEP** |
| **SbAppShell** | No | No (docs only) | **REMOVED** – redundant; Kom uses SufiAppShell |
| **SbAppShellVariant** | No | No (not even used by SbAppShell) | **REMOVED** – dead code |
| **SbDualSidebar** | No | No (docs only) | **REMOVED** – redundant; Kom uses SufiDualSidebar |
| **SbTopBar** | No | No (docs only) | **REMOVED** – redundant; Kom uses SufiTopBar |
| **SbSidebar** | No | No (docs only) | **REMOVED** – redundant; Kom uses SufiSidebar |
| **SbIconRail** | No | No (docs only) | **REMOVED** – redundant; Kom uses SufiIconRail |
| **SbExpandPanel** | No | No (docs only) | **REMOVED** – redundant; Kom uses SufiExpandPanel |
| **SbStack** | Yes (class names in layouts) | Identity, FileManager, Audit, Feature, Setting modules | **KEEP** |
| **SbAlign, SbJustify, SbStackDirection** | Yes | Many modules | **KEEP** |
| **SbGrid, SbGridItem** | No | SufiBlazorDemo only | **KEEP** – design system primitives |
| **SbSpacer** | No | SufiBlazorDemo only | **KEEP** – design system primitive |

## SufiTheme Layouts

- **DualSidebarLayout.razor** – Uses `SufiAppShell`, `SufiDualSidebar`, `SufiIconRail`, `SufiExpandPanel`, `SufiTopBar`.
- **SideMenuLayout.razor** – Uses `SufiAppShell`, `SufiSidebar`, `SufiTopBar`.
- **TopMenuLayout.razor** – Uses `SufiTopBar`, **SbContainer** (only Sb layout component used), `SbDrawer`, `SbNavMenu`, etc.

## Conclusion

- **Keep:** SbContainer, SbContainerMaxWidth, SbStack, SbAlign, SbJustify, SbStackDirection, SbGrid, SbGridItem, SbSpacer (used by apps or by design-system demo).
- **Removed:** SbAppShell, SbAppShellVariant, SbDualSidebar, SbTopBar, SbSidebar, SbIconRail, SbExpandPanel – unused and redundant with SufiTheme components; removing them avoids confusion and mistaken use during feature development.
