using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SufiChain.SufiBlazor.Components.Forms;

public partial class SbSimpleSelect<TValue>
{
    [Microsoft.AspNetCore.Components.CascadingParameter]
    private SbFormFieldContext? FormFieldContext { get; set; }
    private readonly string _generatedId = $"sb-select-{Guid.NewGuid():N}";
    private ElementReference _triggerRef;
    private bool _restoreTriggerFocus;
    private string EffectiveId => Id ?? FormFieldContext?.InputId ?? _generatedId;
    private string LabelledBy => string.IsNullOrEmpty(Label)
        ? FormFieldContext?.LabelId is { } labelId ? $"{labelId} {EffectiveId}-value" : $"{EffectiveId}-value"
        : $"{EffectiveId}-label {EffectiveId}-value";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (_restoreTriggerFocus)
        {
            _restoreTriggerFocus = false;
            await JSRuntime.InvokeVoidAsync("SufiBlazor.focus.set", _triggerRef);
        }
    }
}
