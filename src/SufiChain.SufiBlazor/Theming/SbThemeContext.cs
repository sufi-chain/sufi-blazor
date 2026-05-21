namespace SufiChain.SufiBlazor.Theming;

/// <summary>
/// Provides theme context to child components via CascadingValue.
/// </summary>
public class SbThemeContext
{
    /// <summary>
    /// Gets the current theme.
    /// </summary>
    public SbTheme Theme { get; init; } = SbTheme.Light;

    /// <summary>
    /// Gets whether dark mode is enabled.
    /// </summary>
    public bool IsDark => Theme.IsDark;

    /// <summary>
    /// Gets the current text/layout direction.
    /// </summary>
    public SbDirection Direction { get; init; } = SbDirection.Ltr;

    /// <summary>
    /// Gets the resolved direction attribute value.
    /// </summary>
    public string DirectionAttribute => Direction switch
    {
        SbDirection.Rtl => "rtl",
        SbDirection.Auto => "auto",
        _ => "ltr"
    };
}
