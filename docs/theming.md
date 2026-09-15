# SufiBlazor Theming

## SbThemeProvider

Wrap app content. Set **Theme** (`Light` / `Dark`) and **Direction** (`Ltr` / `Rtl`).  
Toggle theme (and optionally persist in localStorage) to switch at runtime.

## Tokens

CSS variables under `:root`, `.sb-theme-root`, and `.sb-theme-dark` control:

- **Colors** — Primary, secondary, semantic (success, warning, danger), surface, background, text, border.  
- **Typography** — Font family, sizes (xs–3xl), weights, line heights.  
- **Spacing** — `--sb-space-0` through `--sb-space-16`.  
- **Radius** — sm, md, lg, xl, full.  
- **Shadows** — sm, md, lg, xl.  
- **Z-index** — Dropdown, modal, popover, tooltip, toast.

Override in your own CSS to customize. Use semantic tokens (e.g. `--sb-color-primary`) rather than raw values.

## Persian palette defaults

`wwwroot/persian-palette.css` contains the 25 supplied brand primitives.
`sufiblazor.css` imports this file and assigns semantic roles for light and dark modes.
SufiTheme consumes these defaults and owns only its shell-specific tokens.
The Server and WebAssembly global bundles register the primitives before the SB stylesheet.

| Role | Light default | Dark default | Usage |
|------|---------------|--------------|-------|
| Primary | Strong blue `#0067A5` | Pale blue `#BCD4E6` | Main actions, selection, navigation |
| Secondary | Purplish blue `#32127A` | Moderate pink `#D597AE` | Secondary actions and accents |
| Success | Moderate teal `#317873` | Brilliant teal `#00A693` | Completed and successful states |
| Warning | Vivid orange `#F38400` | Moderate orange `#D99058` | Attention and pending review |
| Danger | Vivid red `#CC3333` | Red mixed with white | Errors and destructive actions |
| Info | Vivid blue `#1C39BB` | Vivid blue mixed with white | Informational states |
| Neutral | White, pale-blue tints, mineral-green muted text | Ink `#101F2C`, surfaces `#192C38` / `#243B45`, pale-blue text | Canvas, panels, text hierarchy |

Use `--sb-color-{role}` for fills and `--sb-color-on-{role}` for text on those fills.
Use `--sb-color-{role}-text` for links, status labels, outlined controls and text on tinted backgrounds.
Use `--sb-color-{role}-light` for a tinted surface.
Warning text uses reddish brown in light mode because vivid orange is too light for small text on white.
Hover and active fills are derived from the same role. Their foreground stays paired.
Control boundaries use `--sb-color-control-border`; decorative dividers use `--sb-color-border`.

The chart defaults `--sb-color-chart-1` through `--sb-color-chart-8` are categorical.
They use blue, terracotta, purple, teal, rose and mineral neutrals without assigning success/error meanings to data categories.
Keep labels and legends when using these colors. Do not rely on color alone to distinguish data.
Additional orange, pink, red and purple primitives remain available for illustrations and explicit brand variants.
They are not all suitable for body text or status fills.
Media backdrops, masks, vendor syntax colors and user-selected document colors keep their separate roles.

The supplied list declared `--persian-red-vivid-purplish` twice.
`#CE4676` retains that name; `#FE28A2` is available as `--persian-red-fluorescent-purplish`.

## Style extraction

Run `scripts/color-audit.py PROJECT_ROOT OUTPUT_DIRECTORY` with native Python 3 and `rg` on PATH.
It extracts CSS files and embedded `<style>` blocks with source paths and line numbers.
The output contains `styles.css.txt`, `color-usage.json` and `color-index.json`.
The color index groups each token or literal by its source locations.
Vendor styles and generated editor styles are inventoried separately from authored styles.
Runtime tenant-branding and CMS style expressions are identified separately.
Explicit tenant colors and published CMS content retain their own palettes.
Build, package, tool-cache and runtime directories are excluded.
The combined extraction is for review; do not load it as an application stylesheet.

## RTL

Set `Direction="Rtl"` on `SbThemeProvider`. Components use logical properties so layout flips automatically.

## Custom Themes

Define a new set of variable overrides (e.g. `brand-theme.css`), load after `sufiblazor.css`. No extra C# theme types required unless you add custom theme selection logic.
