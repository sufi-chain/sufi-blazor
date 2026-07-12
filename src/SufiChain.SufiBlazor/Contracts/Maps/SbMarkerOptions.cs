namespace SufiChain.SufiBlazor.Contracts.Maps;

/// <summary>
/// Options for a Leaflet marker managed by SufiBlazor.
/// </summary>
public sealed class SbMarkerOptions
{
    /// <summary>
    /// Marker latitude.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Marker longitude.
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Whether the marker can be dragged.
    /// </summary>
    public bool Draggable { get; set; }

    /// <summary>
    /// Optional tooltip / title text.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Optional accuracy radius in meters (draws a circle).
    /// </summary>
    public double? AccuracyMeters { get; set; }

    /// <summary>
    /// Optional circle color (CSS color).
    /// </summary>
    public string? Color { get; set; }
}
