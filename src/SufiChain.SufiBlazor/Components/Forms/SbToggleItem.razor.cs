using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace SufiChain.SufiBlazor.Components.Forms;

/// <summary>
/// One option inside an <see cref="SbToggleGroup{TValue}"/>.
/// </summary>
/// <typeparam name="TValue">Type of the item value.</typeparam>
public partial class SbToggleItem<TValue>
{
    /// <summary>
    /// The group that owns this item.
    /// </summary>
    [CascadingParameter]
    public SbToggleGroup<TValue>? Parent { get; set; }

    /// <summary>
    /// Value selected when this item is pressed.
    /// </summary>
    [Parameter]
    public TValue? Value { get; set; }

    /// <summary>
    /// Text shown when <see cref="ChildContent"/> is not set.
    /// </summary>
    [Parameter]
    public string? Text { get; set; }

    /// <summary>
    /// Prevents this item from being toggled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Icon shown in the check slot when this item is selected. Defaults to <c>check</c>.
    /// </summary>
    [Parameter]
    public string SelectedIcon { get; set; } = "check";

    /// <summary>
    /// Icon shown in the check slot when this item is not selected.
    /// </summary>
    [Parameter]
    public string? UnselectedIcon { get; set; }

    /// <summary>
    /// Custom item content. The boolean argument is true when this item is selected.
    /// </summary>
    [Parameter]
    public RenderFragment<bool>? ChildContent { get; set; }

    /// <summary>
    /// Additional CSS classes.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Inline styles.
    /// </summary>
    [Parameter]
    public string? Style { get; set; }

    /// <summary>
    /// Additional HTML attributes.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private bool IsSelected => Parent?.IsSelected(Value) ?? false;

    private bool IsDisabled => Disabled || (Parent?.Disabled ?? false);

    private bool ShowMark => Parent is not null && (Parent.CheckMark || Parent.FixedContent);

    private string SelectedMark => string.IsNullOrWhiteSpace(SelectedIcon) ? "check" : SelectedIcon;

    private string MarkClass
    {
        get
        {
            var classes = new List<string> { "sb-toggle-item__mark" };
            if (!string.IsNullOrWhiteSpace(Parent?.CheckMarkClass))
            {
                classes.Add(Parent.CheckMarkClass);
            }

            return string.Join(' ', classes);
        }
    }

    private string CssClass
    {
        get
        {
            var classes = new List<string> { "sb-toggle-item" };
            if (IsSelected)
            {
                classes.Add("sb-toggle-item--selected");
                if (!string.IsNullOrWhiteSpace(Parent?.SelectedClass))
                {
                    classes.Add(Parent.SelectedClass);
                }
            }

            if (IsDisabled)
            {
                classes.Add("sb-toggle-item--disabled");
            }

            if (!string.IsNullOrWhiteSpace(Class))
            {
                classes.Add(Class);
            }

            return string.Join(' ', classes);
        }
    }

    protected override void OnInitialized()
    {
        if (Parent is null)
        {
            throw new InvalidOperationException("SbToggleItem must be used inside SbToggleGroup.");
        }
    }

    private async Task HandleClickAsync()
    {
        if (IsDisabled || Parent is null)
        {
            return;
        }

        await Parent.ToggleAsync(Value);
    }

    private async Task HandleKeyDownAsync(KeyboardEventArgs args)
    {
        if (args.Key is not ("Enter" or " "))
        {
            return;
        }

        await HandleClickAsync();
    }
}
