# SbChart

A lightweight, dependency-free SVG chart component supporting Line, Bar, Donut, and Sparkline variants. All visuals are pure SVG + CSS and use the `--sb-*` design tokens, so charts adapt to the active theme and RTL/LTR direction automatically. No third-party JS is required.

## Variants

| `ChartType` | Description |
|-------------|-------------|
| `Line` | Line chart with area fill, axis, and point markers. |
| `Bar` | Vertical bars sized proportionally to the max value. |
| `Donut` | Stacked donut from per-slice values and colors. |
| `Sparkline` | Compact line chart (no axis, no markers) for inline trends. |

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChartType` | `SbChartType` | `Line` | Chart variant to render. |
| `Values` | `IReadOnlyList<double>?` | `null` | Numeric series. For `Donut`, each value is a slice. |
| `Labels` | `IReadOnlyList<string>?` | `null` | Labels aligned to `Values` (used for accessibility). |
| `Colors` | `IReadOnlyList<string>?` | `null` | Per-slice colors for `Donut`/`Bar`. CSS color or `--sb-*` token name. |
| `Height` | `int` | `160` | Chart height in CSS pixels. |
| `Width` | `int` | `480` | SVG viewBox width (chart scales to container width). |
| `Color` | `string?` | `null` | Main stroke/area color. CSS color or `--sb-*` token name (e.g. `"--sb-color-primary"`). |
| `ShowAxis` | `bool` | `true` | Draw the baseline axis (ignored for `Sparkline`/`Donut`). |
| `AriaLabel` | `string?` | `null` | Accessible label describing the chart. |
| `EmptyText` | `string` | `"No data"` | Text shown when `Values` is null or empty. |
| `Class` | `string?` | `null` | Additional CSS classes. |
| `Style` | `string?` | `null` | Additional inline styles. |

## Color tokens

Pass a token name (without `var()`) and `SbChart` resolves it to `var(--sb-color-*)`. Example: `Color="--sb-color-success"` renders as `var(--sb-color-success)`. Raw CSS colors (e.g. `"#0F766E"`) are also accepted.

## Examples

### Line chart

```razor
<SbChart ChartType="SbChartType.Line"
         Values="@(new double[] { 4, 8, 6, 12, 9, 15, 11, 18 })"
         Height="180" />
```

### Bar chart

```razor
<SbChart ChartType="SbChartType.Bar"
         Values="@(new double[] { 30, 45, 22, 60, 38 })"
         Height="180"
         Color="--sb-color-info" />
```

### Donut chart

```razor
<SbChart ChartType="SbChartType.Donut"
         Values="@(new double[] { 12, 8, 5, 3 })"
         Labels="@(new[] { "Open","In progress","Resolved","Closed" })"
         Height="180" />
```

### Sparkline

```razor
<SbChart ChartType="SbChartType.Sparkline"
         Values="@(new double[] { 2, 4, 3, 6, 5, 8, 7, 9, 11, 10, 13, 15 })"
         Height="48"
         Color="--sb-color-success" />
```

## Accessibility

The chart exposes `role="img"` and an `aria-label`. Provide a meaningful `AriaLabel` describing the data (e.g. "Daily views over the last 30 days") for screen-reader users.
