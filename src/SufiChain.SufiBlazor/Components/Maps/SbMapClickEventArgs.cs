namespace SufiChain.SufiBlazor.Components.Maps;

/// <summary>
/// Event args for map click.
/// </summary>
public sealed class SbMapClickEventArgs
{
    /// <summary>
    /// Click latitude.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Click longitude.
    /// </summary>
    public double Longitude { get; set; }
}
