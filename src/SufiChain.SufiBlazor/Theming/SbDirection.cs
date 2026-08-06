namespace SufiChain.SufiBlazor.Theming;

/// <summary>
/// Represents the text/layout direction for bidirectional UI support.
/// </summary>
public enum SbDirection
{
    /// <summary>
    /// Left-to-right direction (default for Latin scripts).
    /// </summary>
    Ltr,

    /// <summary>
    /// Right-to-left direction (for Arabic, Persian, Hebrew scripts).
    /// </summary>
    Rtl,

    /// <summary>
    /// Automatically determine direction based on content.
    /// </summary>
    Auto
}
