using Microsoft.AspNetCore.Components;

namespace SufiChain.SufiBlazor.Components.Forms.Editor;

public partial class SbEditorAiStatusPill : ComponentBase
{
    [Parameter] public string Text { get; set; } = "Working…";
}
