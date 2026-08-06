namespace SufiChain.SufiBlazor.Components.Layout;

/// <summary>
/// Main-axis justification options.
/// </summary>
public enum SbJustify
{
    /// <summary>
    /// Items are packed toward the start.
    /// </summary>
    Start,

    /// <summary>
    /// Items are centered.
    /// </summary>
    Center,

    /// <summary>
    /// Items are packed toward the end.
    /// </summary>
    End,

    /// <summary>
    /// Items are evenly distributed with first item at start and last at end.
    /// </summary>
    SpaceBetween,

    /// <summary>
    /// Items are evenly distributed with equal space around them.
    /// </summary>
    SpaceAround,

    /// <summary>
    /// Items are evenly distributed with equal space between them.
    /// </summary>
    SpaceEvenly
}
