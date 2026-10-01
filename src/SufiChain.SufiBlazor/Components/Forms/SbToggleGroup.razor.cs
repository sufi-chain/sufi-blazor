using Microsoft.AspNetCore.Components;
using SufiChain.SufiBlazor.Components;

namespace SufiChain.SufiBlazor.Components.Forms;

/// <summary>
/// Grouped toggle buttons for one value, many values, or a value that can be cleared.
/// </summary>
/// <typeparam name="TValue">Type of each item value.</typeparam>
public partial class SbToggleGroup<TValue>
{
    private readonly string _labelId = $"sb-toggle-label-{Guid.NewGuid():N}";

    /// <summary>
    /// Selected value for <see cref="SbToggleSelectionMode.SingleSelection"/>
    /// and <see cref="SbToggleSelectionMode.ToggleSelection"/>.
    /// </summary>
    [Parameter]
    public TValue? Value { get; set; }

    /// <summary>
    /// Raised when <see cref="Value"/> changes.
    /// </summary>
    [Parameter]
    public EventCallback<TValue?> ValueChanged { get; set; }

    /// <summary>
    /// Selected values for <see cref="SbToggleSelectionMode.MultiSelection"/>.
    /// </summary>
    [Parameter]
    public IEnumerable<TValue?>? Values { get; set; }

    /// <summary>
    /// Raised when <see cref="Values"/> changes.
    /// </summary>
    [Parameter]
    public EventCallback<IEnumerable<TValue?>?> ValuesChanged { get; set; }

    /// <summary>
    /// How items are selected. Defaults to single selection.
    /// </summary>
    [Parameter]
    public SbToggleSelectionMode SelectionMode { get; set; } = SbToggleSelectionMode.SingleSelection;

    /// <summary>
    /// Extra CSS classes applied to the selected item. Use this for a custom selected look.
    /// </summary>
    [Parameter]
    public string? SelectedClass { get; set; }

    /// <summary>
    /// Extra CSS classes applied to the check mark.
    /// </summary>
    [Parameter]
    public string? CheckMarkClass { get; set; }

    /// <summary>
    /// Shows a check mark on the selected item.
    /// </summary>
    [Parameter]
    public bool CheckMark { get; set; }

    /// <summary>
    /// Reserves the check-mark slot on every item so labels do not shift.
    /// </summary>
    [Parameter]
    public bool FixedContent { get; set; }

    /// <summary>
    /// Horizontal or vertical layout. Defaults to horizontal.
    /// </summary>
    [Parameter]
    public SbOrientation Orientation { get; set; } = SbOrientation.Horizontal;

    /// <summary>
    /// Draws a shared border around the group.
    /// </summary>
    [Parameter]
    public bool Outlined { get; set; } = true;

    /// <summary>
    /// Draws a divider between items.
    /// </summary>
    [Parameter]
    public bool Delimiters { get; set; } = true;

    /// <summary>
    /// Item size.
    /// </summary>
    [Parameter]
    public SbSize Size { get; set; } = SbSize.Md;

    /// <summary>
    /// Color used for the built-in selected state.
    /// </summary>
    [Parameter]
    public SbColor Color { get; set; } = SbColor.Primary;

    /// <summary>
    /// Prevents every item from being toggled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Stretches the group to the width of its container.
    /// </summary>
    [Parameter]
    public bool FullWidth { get; set; }

    /// <summary>
    /// Visible label above the group. Also used as the accessible name when <see cref="AriaLabel"/> is empty.
    /// </summary>
    [Parameter]
    public string? Label { get; set; }

    /// <summary>
    /// Accessible name when there is no visible <see cref="Label"/>.
    /// </summary>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>
    /// <see cref="SbToggleItem{TValue}"/> children.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Additional CSS classes on the group.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Inline styles on the group.
    /// </summary>
    [Parameter]
    public string? Style { get; set; }

    /// <summary>
    /// Additional HTML attributes on the group.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    internal bool IsSelected(TValue? value)
    {
        if (SelectionMode == SbToggleSelectionMode.MultiSelection)
        {
            if (Values is null)
            {
                return false;
            }

            foreach (var selected in Values)
            {
                if (ValuesEqual(selected, value))
                {
                    return true;
                }
            }

            return false;
        }

        return ValuesEqual(Value, value);
    }

    internal async Task ToggleAsync(TValue? value)
    {
        if (Disabled)
        {
            return;
        }

        if (SelectionMode == SbToggleSelectionMode.MultiSelection)
        {
            var next = new List<TValue?>();
            var removed = false;
            if (Values is not null)
            {
                foreach (var existing in Values)
                {
                    if (!removed && ValuesEqual(existing, value))
                    {
                        removed = true;
                        continue;
                    }

                    next.Add(existing);
                }
            }

            if (!removed)
            {
                next.Add(value);
            }

            Values = next;
            await ValuesChanged.InvokeAsync(next);
            return;
        }

        if (SelectionMode == SbToggleSelectionMode.ToggleSelection && ValuesEqual(Value, value))
        {
            Value = default;
            await ValueChanged.InvokeAsync(default);
            return;
        }

        if (ValuesEqual(Value, value))
        {
            return;
        }

        Value = value;
        await ValueChanged.InvokeAsync(value);
    }

    private string? AccessibleName => string.IsNullOrEmpty(Label) ? AriaLabel : null;

    private string? LabelledBy => string.IsNullOrEmpty(Label) ? null : _labelId;

    private string FieldClass
    {
        get
        {
            var classes = new List<string> { "sb-toggle-group-field" };
            if (FullWidth)
            {
                classes.Add("sb-toggle-group-field--full");
            }

            return string.Join(' ', classes);
        }
    }

    private string CssClass
    {
        get
        {
            var classes = new List<string>
            {
                "sb-toggle-group",
                $"sb-toggle-group--{Orientation.ToString().ToLowerInvariant()}",
                $"sb-toggle-group--{Size.ToString().ToLowerInvariant()}",
                $"sb-toggle-group--{Color.ToString().ToLowerInvariant()}"
            };

            if (Outlined)
            {
                classes.Add("sb-toggle-group--outlined");
            }

            if (Delimiters)
            {
                classes.Add("sb-toggle-group--delimiters");
            }

            if (FullWidth)
            {
                classes.Add("sb-toggle-group--full");
            }

            if (Disabled)
            {
                classes.Add("sb-toggle-group--disabled");
            }

            if (!string.IsNullOrWhiteSpace(Class))
            {
                classes.Add(Class);
            }

            return string.Join(' ', classes);
        }
    }

    private static bool ValuesEqual(TValue? left, TValue? right)
        => EqualityComparer<TValue?>.Default.Equals(left, right);
}
