namespace SufiChain.SufiBlazor.Contracts.Maps;

/// <summary>
/// Options for Nominatim place search.
/// </summary>
public sealed class SbNominatimSearchOptions
{
    /// <summary>
    /// Nominatim search endpoint base URL (no trailing slash).
    /// Override with a proxy to comply with Nominatim usage policy in production.
    /// </summary>
    public string BaseUrl { get; set; } = "https://nominatim.openstreetmap.org";

    /// <summary>
    /// Preferred language (e.g. "en", "fa").
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Comma-separated ISO 3166-1 alpha-2 country codes to limit results.
    /// </summary>
    public string? CountryCodes { get; set; }

    /// <summary>
    /// Maximum number of results.
    /// </summary>
    public int Limit { get; set; } = 8;
}