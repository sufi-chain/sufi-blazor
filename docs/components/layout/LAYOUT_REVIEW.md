# SufiBlazor Layout Components – Review vs Wish Theme

## Summary

The **Wish theme uses its own layout components** (`WishAppShell`, `WishDualSidebar`, `WishTopBar`, `WishSidebar`, `WishIconRail`, `WishExpandPanel`) in `SufiChain.SufiAbp.WishTheme.Blazor/Components/Layout/`. The SufiBlazor design system shell components (`SbAppShell`, `SbDualSidebar`, `SbTopBar`, `SbSidebar`, `SbIconRail`, `SbExpandPanel`) are **not used** in any application or in the Wish theme.

## Usage by Component

| Component | Used in Wish theme? | Used elsewhere? | Verdict |
|-----------|---------------------|-----------------|---------|
| **SbContainer** | Yes – `TopMenuLayout.razor` | SufiBlazorDemo | **KEEP** |
| **SbContainerMaxWidth** | Yes – with SbContainer | SufiBlazorDemo | **KEEP** |
| **SbAppShell** | No | No (docs only) | **REMOVED** – redundant; Wish uses WishAppShell |
| **SbAppShellVariant** | No | No (not even used by SbAppShell) | **REMOVED** – dead code |
| **SbDualSidebar** | No | No (docs only) | **REMOVED** – redundant; Wish uses WishDualSidebar |
| **SbTopBar** | No | No (docs only) | **REMOVED** – redundant; Wish uses WishTopBar |
| **SbSidebar** | No | No (docs only) | **REMOVED** – redundant; Wish uses WishSidebar |
| **SbIconRail** | No | No (docs only) | **REMOVED** – redundant; Wish uses WishIconRail |
| **SbExpandPanel** | No | No (docs only) | **REMOVED** – redundant; Wish uses WishExpandPanel |
| **SbStack** | Yes (class names in layouts) | Identity, FileManager, Audit, Feature, Setting modules | **KEEP** |
| **SbAlign, SbJustify, SbStackDirection** | Yes | Many modules | **KEEP** |
| **SbGrid, SbGridItem** | No | SufiBlazorDemo only | **KEEP** – design system primitives |
| **SbSpacer** | No | SufiBlazorDemo only | **KEEP** – design system primitive |

## Wish Theme Layouts

- **DualSidebarLayout.razor** – Uses `WishAppShell`, `WishDualSidebar`, `WishIconRail`, `WishExpandPanel`, `WishTopBar`.
- **SideMenuLayout.razor** – Uses `WishAppShell`, `WishSidebar`, `WishTopBar`.
- **TopMenuLayout.razor** – Uses `WishTopBar`, **SbContainer** (only Sb layout component used), `SbDrawer`, `SbNavMenu`, etc.

## Conclusion

- **Keep:** SbContainer, SbContainerMaxWidth, SbStack, SbAlign, SbJustify, SbStackDirection, SbGrid, SbGridItem, SbSpacer (used by apps or by design-system demo).
- **Removed:** SbAppShell, SbAppShellVariant, SbDualSidebar, SbTopBar, SbSidebar, SbIconRail, SbExpandPanel – unused and redundant with Wish theme components; removing them avoids confusion and mistaken use during feature development.
