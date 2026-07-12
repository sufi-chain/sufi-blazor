namespace SufiChain.SufiBlazor.Contracts.Maps;

/// <summary>
/// Options passed to the Leaflet map initializer.
/// </summary>
public sealed class SbMapInitOptions
{
    /// <summary>
    /// Initial center latitude.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Initial center longitude.
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Initial zoom level.
    /// </summary>
    public int Zoom { get; set; } = 2;

    /// <summary>
    /// Minimum zoom.
    /// </summary>
    public int? MinZoom { get; set; }

    /// <summary>
    /// Maximum zoom.
    /// </summary>
    public int? MaxZoom { get; set; }

    /// <summary>
    /// Whether the user can pan and zoom.
    /// </summary>
    public bool Interactive { get; set; } = true;

    /// <summary>
    /// Whether to show OSM attribution.
    /// </summary>
    public bool ShowAttribution { get; set; } = true;

    /// <summary>
    /// Tile URL template.
    /// </summary>
    public string TileUrl { get; set; } = "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png";

    /// <summary>
    /// Tile attribution HTML.
    /// </summary>
    public string TileAttribution { get; set; } =
        "&copy; <a href=\"https://www.openstreetmap.org/copyright\">OpenStreetMap</a>";
}
