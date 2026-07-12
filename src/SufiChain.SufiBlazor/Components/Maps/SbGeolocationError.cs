namespace SufiChain.SufiBlazor.Components.Maps;

/// <summary>
/// Error from the browser Geolocation API.
/// </summary>
public sealed class SbGeolocationError
{
    /// <summary>
    /// Error code (1=PERMISSION_DENIED, 2=POSITION_UNAVAILABLE, 3=TIMEOUT).
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// Human-readable message.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
