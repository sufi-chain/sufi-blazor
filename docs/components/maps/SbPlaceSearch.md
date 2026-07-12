# SbPlaceSearch

Nominatim-backed place autocomplete (OpenStreetMap geocoding).

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| Value | SbPlaceResult? | null | Selected place (bindable) |
| Placeholder | string? | localized | Placeholder |
| Label | string? | null | Field label |
| Language | string? | null | Accept-Language |
| CountryCodes | string? | null | ISO country filter (e.g. `"ir"`) |
| NominatimBaseUrl | string | public Nominatim | Override with a proxy |
| DebounceMs | int | 400 | Debounce |
| MinLength | int | 2 | Min chars before search |
| MaxResults | int | 8 | Result limit |
| Disabled | bool | false | Disabled |

## Events

| Event | Type | Description |
|-------|------|-------------|
| ValueChanged | EventCallback\<SbPlaceResult?\> | Selection changed |
| OnPlaceSelected | EventCallback\<SbPlaceResult\> | Place chosen |

## Example

```razor
<SbPlaceSearch CountryCodes="ir"
               Language="fa"
               OnPlaceSelected="place => MoveMap(place)" />
```

## Nominatim usage

- Debounce searches; avoid bulk requests.
- For production, set `NominatimBaseUrl` to your own proxy that adds a proper identifying User-Agent.
- See [Nominatim usage policy](https://operations.osmfoundation.org/policies/nominatim/).
