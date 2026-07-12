using System.Globalization;

namespace SufiChain.SufiBlazor.Components.Maps;

/// <summary>
/// Geographic position with optional accuracy and label.
/// Shape matches common location payloads (e.g. chat location metadata).
/// </summary>
public sealed class SbGeoPosition
{
    /// <summary>
    /// Latitude in decimal degrees.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude in decimal degrees.
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Optional horizontal accuracy in meters.
    /// </summary>
    public double? AccuracyMeters { get; set; }

    /// <summary>
    /// Optional human-readable place label.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Converts to a <see cref="SbLatLng"/>.
    /// </summary>
    public SbLatLng ToLatLng() => new(Latitude, Longitude);

    /// <summary>
    /// Builds an OpenStreetMap URL centered on this position with a marker.
    /// </summary>
    public string ToOpenStreetMapUrl(int zoom = 16) => GetOpenStreetMapUrl(Latitude, Longitude, zoom);

    /// <summary>
    /// Builds an OpenStreetMap URL for the given coordinates.
    /// </summary>
    public static string GetOpenStreetMapUrl(double latitude, double longitude, int zoom = 16)
    {
        var lat = latitude.ToString(CultureInfo.InvariantCulture);
        var lng = longitude.ToString(CultureInfo.InvariantCulture);
        return $"https://www.openstreetmap.org/?mlat={lat}&mlon={lng}#map={zoom}/{lat}/{lng}";
    }
}
