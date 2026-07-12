# SbGeolocateButton

Icon button that captures the browser geolocation position.

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| HighAccuracy | bool | true | Request high accuracy |
| TimeoutMs | int | 10000 | Timeout |
| MaximumAgeMs | int | 0 | Max cached age |
| Disabled | bool | false | Disabled state |
| Size | SbSize | Md | Button size |
| Color | SbColor | Default | Button color |
| Variant | SbButtonVariant | Ghost | Button variant |
| AriaLabel | string? | localized | Accessible label |
| Title | string? | localized | Tooltip |

## Events

| Event | Type | Description |
|-------|------|-------------|
| OnLocated | EventCallback\<SbGeoPosition\> | Success |
| OnError | EventCallback\<SbGeolocationError\> | Failure |

## Example

```razor
<SbGeolocateButton OnLocated="pos => _position = pos"
                   OnError="err => _error = err.Message" />
```
