namespace SufiChain.SufiBlazor.Components.Overlays;

/// <summary>
/// Placement options for drawers using logical Start/End for RTL compatibility.
/// </summary>
public enum SbDrawerPlacement
{
    /// <summary>Left in LTR, Right in RTL.</summary>
    Start,
    
    /// <summary>Right in LTR, Left in RTL.</summary>
    End,
    
    /// <summary>Top of the screen.</summary>
    Top,
    
    /// <summary>Bottom of the screen.</summary>
    Bottom
}
