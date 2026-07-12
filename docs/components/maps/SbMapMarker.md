# SbMapMarker

Marker (optional accuracy circle) that registers with a parent `SbMap`.

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| Position | SbLatLng | (0, 0) | Marker position (bindable via PositionChanged) |
| Draggable | bool | false | Allow drag |
| Label | string? | null | Tooltip / label |
| Title | string? | null | Alias for Label when Label is null |
| AccuracyMeters | double? | null | Draws an accuracy circle |
| Color | string? | null | Circle accent color |

## Events

| Event | Type | Description |
|-------|------|-------------|
| PositionChanged | EventCallback\<SbLatLng\> | After drag end |
| OnClick | EventCallback | Marker click |

## Example

```razor
<SbMap Center="_center" Zoom="14">
    <SbMapMarker @bind-Position="_pin"
                 Draggable="true"
                 AccuracyMeters="25"
                 Label="Pin" />
</SbMap>
```
