# SbMapPicker

Dialog for picking a location: searchable place, geolocate, click map, or drag pin.

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| Open | bool | false | Dialog open state (bindable) |
| Value | SbGeoPosition? | null | Selected position (updated on confirm) |
| Title | string? | localized | Dialog title |
| ConfirmText | string? | localized | Confirm button text |
| ShowSearch | bool | true | Show Nominatim search |
| ShowGeolocate | bool | true | Show geolocate button |
| InitialCenter | SbLatLng? | Tehran | Center when Value is null |
| MapHeight | string | `"360px"` | Map height |
| Language | string? | null | Nominatim language |
| CountryCodes | string? | null | Nominatim country filter |
| NominatimBaseUrl | string | public Nominatim | Override with a proxy in production |

## Events

| Event | Type | Description |
|-------|------|-------------|
| OpenChanged | EventCallback\<bool\> | Open state |
| ValueChanged | EventCallback\<SbGeoPosition?\> | On confirm |
| OnConfirm | EventCallback\<SbGeoPosition\> | On confirm |
| OnCancel | EventCallback | On cancel / dismiss |

## Example

```razor
<SbButton OnClick="@(() => _open = true)">Pick location</SbButton>

<SbMapPicker @bind-Open="_open"
             @bind-Value="_position"
             OnConfirm="HandleConfirm" />
```

## Nominatim policy

Use a short debounce (default 400ms), identify your app, and prefer a host-side proxy via `NominatimBaseUrl` for production traffic. Do not bulk-geocode.
