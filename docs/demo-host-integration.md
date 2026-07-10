# Demo Host Integration

`SufiChain.SufiBlazor.Demo` is a **platform-hosted component gallery**, not a minimal standalone sample app.

## Dependency split

| Project | SufiAbp dependency | Role |
| --- | --- | --- |
| `SufiChain.SufiBlazor` | **None** | Core `Sb*` library |
| `SufiChain.SufiBlazor.Demo` | **Yes** — `SufiChain.SufiAbp.UI.Blazor` | Demo pages + menu contributor |
| `SufiChain.SufiBlazor.Demo.Localization` | ABP localization module | Demo menu strings (en/fa/ar) |

The Demo references SufiAbp **only for menu integration** (`IMenuContributor`, `ApplicationMenuItem`, `AbpModule`). It does not change the standalone nature of the core package.

## How the Console host wires it

In `.dev/hosts/SufiChane.Console`:

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
- Run the gallery via SufiChane.Console (full platform stack)

Removing SufiAbp from the Demo would require replacing menu contribution, localization, and module bootstrap — out of scope for the core library.

## Related

- [Overview](overview.md)
- [Standalone adoption](standalone-adoption.md)
