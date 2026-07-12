using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SufiChain.SufiBlazor.Components.Maps;
using SufiChain.SufiBlazor.Contracts.Maps;

namespace SufiChain.SufiBlazor.Interop;

/// <summary>
/// JavaScript interop for Leaflet / OpenStreetMap map components.
/// </summary>
public sealed class SbMapInterop : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _module;
    private bool _disposed;

    /// <summary>
    /// Creates a new map interop helper.
    /// </summary>
    public SbMapInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    private async Task<IJSObjectReference> EnsureModuleAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_module == null)
        {
            _module = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/SufiChain.SufiBlazor/sufiblazor-map.js");
        }

        return _module;
    }

    /// <summary>
    /// Initializes a Leaflet map on the given element.
    /// </summary>
    public async Task<string> InitializeMapAsync<T>(
        ElementReference element,
        DotNetObjectReference<T> dotNetRef,
        SbMapInitOptions options) where T : class
    {
        var module = await EnsureModuleAsync();
        return await module.InvokeAsync<string>("initMap", element, dotNetRef, options);
    }

    /// <summary>
    /// Destroys a map instance.
    /// </summary>
    public async Task DestroyMapAsync(string mapId)
    {
        if (_module == null || string.IsNullOrEmpty(mapId))
        {
            return;
        }

        await _module.InvokeVoidAsync("destroyMap", mapId);
    }

    /// <summary>
    /// Sets the map center and optional zoom.
    /// </summary>
    public async Task SetViewAsync(string mapId, double latitude, double longitude, int? zoom = null)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("setView", mapId, latitude, longitude, zoom);
    }

    /// <summary>
    /// Enables or disables map interaction.
    /// </summary>
    public async Task SetInteractiveAsync(string mapId, bool interactive)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("setInteractive", mapId, interactive);
    }

    /// <summary>
    /// Invalidates Leaflet size (call after dialog open / layout change).
    /// </summary>
    public async Task InvalidateSizeAsync(string mapId)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("invalidateSize", mapId);
    }

    /// <summary>
    /// Adds a marker to the map.
    /// </summary>
    public async Task AddMarkerAsync<T>(
        string mapId,
        string markerId,
        SbMarkerOptions options,
        DotNetObjectReference<T> dotNetRef) where T : class
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("addMarker", mapId, markerId, options, dotNetRef);
    }

    /// <summary>
    /// Updates an existing marker.
    /// </summary>
    public async Task UpdateMarkerAsync(string mapId, string markerId, SbMarkerOptions options)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("updateMarker", mapId, markerId, options);
    }

    /// <summary>
    /// Removes a marker from the map.
    /// </summary>
    public async Task RemoveMarkerAsync(string mapId, string markerId)
    {
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("removeMarker", mapId, markerId);
    }

    /// <summary>
    /// Gets the current browser geolocation position.
    /// </summary>
    public async Task<SbGeoPosition?> GetCurrentPositionAsync(SbGeolocationOptions? options = null)
    {
        var module = await EnsureModuleAsync();
        var result = await module.InvokeAsync<JsonElement>("getCurrentPosition", options ?? new SbGeolocationOptions());

        if (result.TryGetProperty("error", out var errorProp) && errorProp.GetBoolean())
        {
            return null;
        }

        return new SbGeoPosition
        {
            Latitude = result.GetProperty("latitude").GetDouble(),
            Longitude = result.GetProperty("longitude").GetDouble(),
            AccuracyMeters = result.TryGetProperty("accuracyMeters", out var acc) && acc.ValueKind == JsonValueKind.Number
                ? acc.GetDouble()
                : null
        };
    }

    /// <summary>
    /// Gets the current browser geolocation position or an error payload.
    /// </summary>
    public async Task<(SbGeoPosition? Position, SbGeolocationError? Error)> TryGetCurrentPositionAsync(
        SbGeolocationOptions? options = null)
    {
        var module = await EnsureModuleAsync();
        var result = await module.InvokeAsync<JsonElement>("getCurrentPosition", options ?? new SbGeolocationOptions());

        if (result.TryGetProperty("error", out var errorProp) && errorProp.GetBoolean())
        {
            return (null, new SbGeolocationError
            {
                Code = result.TryGetProperty("code", out var code) ? code.GetInt32() : 0,
                Message = result.TryGetProperty("message", out var msg) ? msg.GetString() ?? "Geolocation failed." : "Geolocation failed."
            });
        }

        var position = new SbGeoPosition
        {
            Latitude = result.GetProperty("latitude").GetDouble(),
            Longitude = result.GetProperty("longitude").GetDouble(),
            AccuracyMeters = result.TryGetProperty("accuracyMeters", out var acc) && acc.ValueKind == JsonValueKind.Number
                ? acc.GetDouble()
                : null
        };

        return (position, null);
    }

    /// <summary>
    /// Searches Nominatim for places matching the query.
    /// </summary>
    public async Task<IReadOnlyList<SbPlaceResult>> SearchNominatimAsync(
        string query,
        SbNominatimSearchOptions? options = null)
    {
        var module = await EnsureModuleAsync();
        var json = await module.InvokeAsync<JsonElement>("searchNominatim", query, options ?? new SbNominatimSearchOptions());

        if (json.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<SbPlaceResult>();
        }

        var list = new List<SbPlaceResult>();
        foreach (var item in json.EnumerateArray())
        {
            list.Add(new SbPlaceResult
            {
                DisplayName = item.TryGetProperty("displayName", out var name) ? name.GetString() ?? "" : "",
                Latitude = item.TryGetProperty("latitude", out var lat) ? lat.GetDouble() : 0,
                Longitude = item.TryGetProperty("longitude", out var lng) ? lng.GetDouble() : 0,
                BoundingBox = item.TryGetProperty("boundingBox", out var bbox) && bbox.ValueKind == JsonValueKind.Array
                    ? bbox.EnumerateArray().Select(x => x.GetDouble()).ToArray()
                    : null,
                OsmType = item.TryGetProperty("osmType", out var osmType) ? osmType.GetString() : null,
                OsmId = item.TryGetProperty("osmId", out var osmId) && osmId.ValueKind == JsonValueKind.Number
                    ? osmId.GetInt64()
                    : null
            });
        }

        return list;
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (_module != null)
        {
            try
            {
                await _module.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // circuit gone
            }

            _module = null;
        }
    }
}
