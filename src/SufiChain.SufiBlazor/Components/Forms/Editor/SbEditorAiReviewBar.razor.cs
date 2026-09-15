using Microsoft.AspNetCore.Components;

namespace SufiChain.SufiBlazor.Components.Forms.Editor;

public partial class SbEditorAiReviewBar : ComponentBase
{
    [Parameter] public bool Visible { get; set; }
    [Parameter] public bool ShowBatchActions { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter] public string AriaLabel { get; set; } = "AI suggestion review";
    [Parameter] public string AcceptText { get; set; } = "Accept";
    [Parameter] public string RejectText { get; set; } = "Reject";
    [Parameter] public string DismissText { get; set; } = "Dismiss";
    [Parameter] public string AcceptAllText { get; set; } = "Accept all";
    [Parameter] public string RejectAllText { get; set; } = "Reject all";
    [Parameter] public EventCallback OnAccept { get; set; }
    [Parameter] public EventCallback OnReject { get; set; }
    [Parameter] public EventCallback OnDismiss { get; set; }
    [Parameter] public EventCallback OnAcceptAll { get; set; }
    [Parameter] public EventCallback OnRejectAll { get; set; }
}
