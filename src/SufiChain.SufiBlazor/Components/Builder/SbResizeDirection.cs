namespace SufiChain.SufiBlazor.Components.Builder;

/// <summary>
/// Directions for resizing.
/// </summary>
[Flags]
public enum SbResizeDirection
{
    None = 0,
    Top = 1,
    Right = 2,
    Bottom = 4,
    Left = 8,
    TopRight = 16,
    BottomRight = 32,
    BottomLeft = 64,
    TopLeft = 128,
    All = Top | Right | Bottom | Left | TopRight | BottomRight | BottomLeft | TopLeft
}
