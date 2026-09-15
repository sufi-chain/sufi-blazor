using Microsoft.AspNetCore.Components;

namespace SufiChain.SufiBlazor.Components.Forms.Editor;

public partial class SbEditorAiMenu : ComponentBase
{
    [Parameter] public bool Open { get; set; }
    [Parameter] public EventCallback<bool> OpenChanged { get; set; }
    [Parameter] public string Prompt { get; set; } = "";
    [Parameter] public EventCallback<string> PromptChanged { get; set; }
    [Parameter] public string? StatusText { get; set; }
    [Parameter] public bool Busy { get; set; }
    [Parameter] public string Title { get; set; } = "Ask AI";
    [Parameter] public string Placeholder { get; set; } = "Describe the change…";
    [Parameter] public string SubmitText { get; set; } = "Run";
    [Parameter] public string CancelText { get; set; } = "Cancel";
    [Parameter] public EventCallback OnSubmit { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    private Task OnPromptChangedAsync(string? value) => PromptChanged.InvokeAsync(value ?? "");
}
