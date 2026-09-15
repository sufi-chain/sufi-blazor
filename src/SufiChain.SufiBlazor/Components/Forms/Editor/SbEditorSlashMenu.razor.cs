using Microsoft.AspNetCore.Components;
using SufiChain.SufiBlazor.Contracts.Editors;

namespace SufiChain.SufiBlazor.Components.Forms.Editor;

public partial class SbEditorSlashMenu : ComponentBase
{
    [Parameter] public bool Open { get; set; }
    [Parameter] public IReadOnlyList<EditorToolbarItem> Items { get; set; } = Array.Empty<EditorToolbarItem>();
    [Parameter] public EventCallback<EditorToolbarItem> OnItemClick { get; set; }
    [Parameter] public string AriaLabel { get; set; } = "Insert block";
}
