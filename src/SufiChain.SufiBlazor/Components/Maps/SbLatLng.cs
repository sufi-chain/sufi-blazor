namespace SufiChain.SufiBlazor.Components.Maps;

/// <summary>
/// Geographic coordinate (WGS84).
/// </summary>
public sealed class SbLatLng : IEquatable<SbLatLng>
{
    /// <summary>
    /// Creates a coordinate at (0, 0).
    /// </summary>
    public SbLatLng()
    {
    }

    /// <summary>
    /// Creates a coordinate at the given latitude and longitude.
    /// </summary>
    public SbLatLng(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    /// <summary>
    /// Latitude in decimal degrees.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude in decimal degrees.
    /// </summary>
    public double Longitude { get; set; }

    /// <inheritdoc />
    public bool Equals(SbLatLng? other)
    {
        if (other is null)
        {
            return false;
        }

        return Latitude.Equals(other.Latitude) && Longitude.Equals(other.Longitude);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as SbLatLng);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Latitude, Longitude);

    /// <inheritdoc />
    public override string ToString() =>
        $"{Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
}
