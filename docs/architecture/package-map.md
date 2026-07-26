# Package Map

Projects in the SufiBlazor repository and their dependencies.

## Projects

| Project | Package ID | Role |
|---------|------------|------|
| `SufiChain.SufiBlazor` | `SufiChain.SufiBlazor` | Core `Sb*` component library |
| `SufiChain.SufiBlazor.Demo` | `SufiChain.SufiBlazor.Demo` | Platform-hosted component gallery |
| `SufiChain.SufiBlazor.Demo.Localization` | `SufiChain.SufiBlazor.Demo.Localization` | Demo menu strings (en/fa/ar) |
| `SufiChain.SufiBlazor.Tests` | — (not published) | bUnit component tests |

## Dependency graph

```
SufiChain.SufiBlazor
  ├── Microsoft.AspNetCore.Components.Web
  └── Microsoft.Extensions.Localization

SufiChain.SufiBlazor.Demo
  ├── SufiChain.SufiBlazor (project)
  ├── SufiChain.SufiBlazor.Demo.Localization (project)
  └── SufiChain.SufiAbp.UI.Blazor (package — transitional; Sufi Platform UI Blazor)

SufiChain.SufiBlazor.Demo.Localization
  ├── Volo.Abp.Localization
  ├── Volo.Abp.VirtualFileSystem
  └── SufiChain.SufiAbp.UI.Domain.Shared

SufiChain.SufiBlazor.Tests
  └── SufiChain.SufiBlazor (project)
```

## Core library boundary

`SufiChain.SufiBlazor` depends on **Web + Localization only**. It has no ABP, SufiTheme, or Sufi Platform references. Demo and test projects sit outside that boundary.

## Related

- [Architecture decisions](decisions.md)
- [Demo host integration](../demo-host-integration.md)
- [Standalone adoption](../standalone-adoption.md)
