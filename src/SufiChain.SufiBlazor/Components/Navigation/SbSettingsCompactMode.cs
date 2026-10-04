namespace SufiChain.SufiBlazor.Components.Navigation;

/// <summary>
/// How <see cref="SbSettingsLayout"/> presents sections at the compact breakpoint (768px and below).
/// Wide layouts always use the icon and label rail.
/// </summary>
public enum SbSettingsCompactMode
{
    /// <summary>
    /// A horizontal strip when six or fewer sections are visible, otherwise a picker.
    /// </summary>
    Auto,

    /// <summary>
    /// Always a horizontal icon strip on compact screens.
    /// </summary>
    Strip,

    /// <summary>
    /// Always a section picker on compact screens.
    /// </summary>
    Picker
}
