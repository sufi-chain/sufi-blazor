# SbMapPreview

Compact read-only map card for chat bubbles, CRM side panels, and location summaries.

Aligned with location payloads that expose `Latitude`, `Longitude`, `AccuracyMeters`, and `Label`.

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| Latitude | double | 0 | Latitude |
| Longitude | double | 0 | Longitude |
| AccuracyMeters | double? | null | Optional GPS accuracy ring |
| Label | string? | null | Caption text |
| Height | string | `"140px"` | Map height |
| Zoom | int | 15 | Preview zoom |
| OpenExternalOnClick | bool | true | Open OSM on click |
| ShowAttribution | bool | false | Show tile attribution |

## Example

```razor
<SbMapPreview Latitude="35.6892"
              Longitude="51.3890"
              AccuracyMeters="12"
              Label="Office"
              Height="160px" />
```

## CSS

- `sb-map-preview`
- `sb-map-preview__map`
- `sb-map-preview__footer`
- `sb-map-preview__label`
- `sb-map-preview__link`
- `sb-map-preview__overlay`
