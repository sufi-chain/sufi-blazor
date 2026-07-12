namespace SufiChain.SufiBlazor.Components.Maps;

/// <summary>
/// A place result from Nominatim (or a compatible geocoder).
/// </summary>
public sealed class SbPlaceResult
{
    /// <summary>
    /// Display name from the geocoder.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Latitude in decimal degrees.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude in decimal degrees.
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Optional bounding box: south, north, west, east.
    /// </summary>
    public double[]? BoundingBox { get; set; }

    /// <summary>
    /// Optional OSM type (node, way, relation).
    /// </summary>
    public string? OsmType { get; set; }

    /// <summary>
    /// Optional OSM id.
    /// </summary>
    public long? OsmId { get; set; }

    /// <summary>
    /// Converts to a <see cref="SbGeoPosition"/> using the display name as label.
    /// </summary>
    public SbGeoPosition ToGeoPosition() => new()
    {
        Latitude = Latitude,
        Longitude = Longitude,
        Label = DisplayName
    };
}
