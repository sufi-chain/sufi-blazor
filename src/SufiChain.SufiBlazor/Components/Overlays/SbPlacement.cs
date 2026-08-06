namespace SufiChain.SufiBlazor.Components.Overlays;

/// <summary>
/// Placement options for positioned overlays (popovers, tooltips, menus).
/// Uses logical Start/End naming for RTL/LTR compatibility.
/// </summary>
public enum SbPlacement
{
    /// <summary>Top center, aligned to start.</summary>
    TopStart,
    
    /// <summary>Top center.</summary>
    Top,
    
    /// <summary>Top center, aligned to end.</summary>
    TopEnd,
    
    /// <summary>Right side (or left in RTL), aligned to start.</summary>
    EndStart,
    
    /// <summary>Right side (or left in RTL), centered.</summary>
    End,
    
    /// <summary>Right side (or left in RTL), aligned to end.</summary>
    EndEnd,
    
    /// <summary>Bottom center, aligned to start.</summary>
    BottomStart,
    
    /// <summary>Bottom center.</summary>
    Bottom,
    
    /// <summary>Bottom center, aligned to end.</summary>
    BottomEnd,
    
    /// <summary>Left side (or right in RTL), aligned to start.</summary>
    StartStart,
    
    /// <summary>Left side (or right in RTL), centered.</summary>
    Start,
    
    /// <summary>Left side (or right in RTL), aligned to end.</summary>
    StartEnd
}

/// <summary>
/// Extension methods for SbPlacement.
/// </summary>
public static class SbPlacementExtensions
{
    /// <summary>
    /// Converts placement to CSS class suffix.
    /// </summary>
    public static string ToCssClass(this SbPlacement placement) => placement switch
    {
        SbPlacement.TopStart => "top-start",
        SbPlacement.Top => "top",
        SbPlacement.TopEnd => "top-end",
        SbPlacement.EndStart => "end-start",
        SbPlacement.End => "end",
        SbPlacement.EndEnd => "end-end",
        SbPlacement.BottomStart => "bottom-start",
        SbPlacement.Bottom => "bottom",
        SbPlacement.BottomEnd => "bottom-end",
        SbPlacement.StartStart => "start-start",
        SbPlacement.Start => "start",
        SbPlacement.StartEnd => "start-end",
        _ => "bottom"
    };
}
