namespace SufiChain.SufiBlazor.Contracts.Maps;

/// <summary>
/// Options for browser geolocation.
/// </summary>
public sealed class SbGeolocationOptions
{
    /// <summary>
    /// Request high-accuracy GPS when available.
    /// </summary>
    public bool EnableHighAccuracy { get; set; } = true;

    /// <summary>
    /// Timeout in milliseconds.
    /// </summary>
    public int TimeoutMs { get; set; } = 10000;

    /// <summary>
    /// Maximum cached age in milliseconds.
    /// </summary>
    public int MaximumAgeMs { get; set; }
}
