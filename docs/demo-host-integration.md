# Demo Host Integration

`SufiChain.SufiBlazor.Demo` is a **platform-hosted component gallery**, not a minimal standalone sample app.

## Dependency split

| Project | Sufi Platform UI dependency | Role |
| --- | --- | --- |
| `SufiChain.SufiBlazor` | **None** | Core `Sb*` library |
| `SufiChain.SufiBlazor.Demo` | **Yes** — Sufi Platform UI Blazor (`SufiChain.SufiAbp.UI.Blazor` package id until rename lands) | Demo pages + menu contributor |
| `SufiChain.SufiBlazor.Demo.Localization` | ABP localization module | Demo menu strings (en/fa/ar) |

The Demo depends on **Sufi Platform UI** for menus (`IMenuContributor`, `ApplicationMenuItem`, `AbpModule`). The `.csproj` may still reference package id `SufiChain.SufiAbp.UI.Blazor` as a transitional name until the package rename lands. That dependency does not change the standalone nature of the core `SufiChain.SufiBlazor` package.

## How the Sufi Platform host wires it

In `.dev/hosts/SufiChane.SufiPlatform/`:

1. Project-reference or package-reference to `SufiChain.SufiBlazor.Demo`
2. `[DependsOn(typeof(SufiBlazorDemoModule))]` on the host module
3. Demo routes under `/demo/sufi-blazor/*`
4. Menu entries from `SufiBlazorDemoMenuContributor`

## Docs mirror in Demo

On build, MSBuild copies `docs/` → `wwwroot/docs/` in the Demo project (`CopyDocsToWwwroot` target in the Demo `.csproj`). The committed `wwwroot/docs/` copy is build output — prefer editing files under `docs/` at the repo root of sufi-blazor.

## If you need a standalone demo

Today there is no separate zero-dependency demo host. Options:

- Use [Standalone adoption](standalone-adoption.md) and build a small test page in your own Blazor app
- Use the component reference under `docs/components/`
- Run the gallery via `.dev/hosts/SufiChane.SufiPlatform/` (full platform stack)

Removing Sufi Platform UI from the Demo would require replacing menu contribution, localization, and module bootstrap — out of scope for the core library.

## Related

- [Overview](overview.md)
- [Package map](architecture/package-map.md)
- [Standalone adoption](standalone-adoption.md)
