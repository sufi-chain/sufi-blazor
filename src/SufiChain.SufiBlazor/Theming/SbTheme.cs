namespace SufiChain.SufiBlazor.Theming;

/// <summary>
/// Represents a theme configuration with design tokens.
/// </summary>
public class SbTheme
{
    /// <summary>
    /// Gets or sets the theme name.
    /// </summary>
    public string Name { get; set; } = "Default";

    /// <summary>
    /// Gets or sets whether this is a dark theme.
    /// </summary>
    public bool IsDark { get; set; }

    /// <summary>
    /// Gets the default light theme.
    /// </summary>
    public static SbTheme Light => new()
    {
        Name = "Sufi Light",
        IsDark = false
    };

    /// <summary>
    /// Gets the default dark theme.
    /// </summary>
    public static SbTheme Dark => new()
    {
        Name = "Sufi Dark",
        IsDark = true
    };
}
