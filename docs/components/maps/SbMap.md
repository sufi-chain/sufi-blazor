# SbMap

Interactive OpenStreetMap map canvas powered by Leaflet.

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| Center | SbLatLng | (0, 0) | Map center (bindable) |
| Zoom | int | 2 | Zoom level (bindable) |
| MinZoom | int? | null | Minimum zoom |
| MaxZoom | int? | null | Maximum zoom |
| Height | string | `"280px"` | Map height CSS value |
| Interactive | bool | true | Allow pan/zoom |
| ShowAttribution | bool | true | Show OSM attribution |
| TileUrl | string | OSM tiles | Tile URL template |
| TileAttribution | string | OSM copyright | Attribution HTML |
| Class | string? | null | Extra CSS classes |
| Style | string? | null | Inline styles |

## Events

| Event | Type | Description |
|-------|------|-------------|
| CenterChanged | EventCallback\<SbLatLng\> | After pan/zoom end |
| ZoomChanged | EventCallback\<int\> | After pan/zoom end |
| OnClick | EventCallback\<SbMapClickEventArgs\> | Map click |

## Child content

Nest `SbMapMarker` (and future overlays) inside `SbMap`.

## Example

```razor
<SbMap Center="new SbLatLng(35.6892, 51.3890)" Zoom="12" Height="320px"
       OnClick="HandleClick">
    <SbMapMarker Position="new SbLatLng(35.6892, 51.3890)" Label="Tehran" />
</SbMap>
```

## CSS

- `sb-map`
- `sb-map--readonly`

## Notes

- Leaflet is loaded on demand from bundled `wwwroot/vendor/leaflet/` with CDN fallback.
- Call `InvalidateSizeAsync()` after showing the map inside a dialog.
