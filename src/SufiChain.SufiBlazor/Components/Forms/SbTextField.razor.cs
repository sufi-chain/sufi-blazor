namespace SufiChain.SufiBlazor.Components.Forms;

public partial class SbTextField<TValue>
{
    [Microsoft.AspNetCore.Components.CascadingParameter]
    private SbFormFieldContext? FormFieldContext { get; set; }
}
