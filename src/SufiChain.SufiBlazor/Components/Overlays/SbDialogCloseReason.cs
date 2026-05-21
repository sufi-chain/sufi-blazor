namespace SufiChain.SufiBlazor.Components.Overlays;

/// <summary>
/// Reasons why a dialog was closed.
/// </summary>
public enum SbDialogCloseReason
{
    /// <summary>User pressed escape key.</summary>
    Escape,
    
    /// <summary>User clicked the backdrop.</summary>
    Backdrop,
    
    /// <summary>User clicked the close button.</summary>
    CloseButton,
    
    /// <summary>Dialog was closed programmatically.</summary>
    Programmatic
}
