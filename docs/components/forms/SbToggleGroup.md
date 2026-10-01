# SbToggleGroup

Grouped toggle buttons. Use one group for a single value, several values, or a value that can be cleared. Put each option in an `SbToggleItem`.

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| Value | TValue? | null | Selected value for single and toggle selection |
| ValueChanged | EventCallback\<TValue?\> | | Raised when Value changes |
| Values | IEnumerable\<TValue?\>? | null | Selected values for multi selection |
| ValuesChanged | EventCallback\<IEnumerable\<TValue?\>?\> | | Raised when Values changes |
| SelectionMode | SbToggleSelectionMode | SingleSelection | SingleSelection, MultiSelection, or ToggleSelection |
| SelectedClass | string? | null | Extra CSS classes applied to the selected item |
| CheckMark | bool | false | Shows a check mark on the selected item |
| CheckMarkClass | string? | null | Extra CSS classes on the check mark |
| FixedContent | bool | false | Reserves the check-mark slot on every item |
| Orientation | SbOrientation | Horizontal | Horizontal or vertical layout |
| Outlined | bool | true | Shared border around the group |
| Delimiters | bool | true | Divider between items |
| Size | SbSize | Md | Item size |
| Color | SbColor | Primary | Built-in selected color |
| Disabled | bool | false | Disables every item |
| FullWidth | bool | false | Stretches the group to its container |
| Label | string? | null | Visible label and accessible name |
| AriaLabel | string? | null | Accessible name when Label is empty |
| Class | string? | null | Additional CSS classes |
| Style | string? | null | Inline styles |

## Selection modes

- **SingleSelection** keeps one item selected. Choosing another item replaces the value.
- **MultiSelection** toggles each item in `Values`. Bind `Values`, not `Value`.
- **ToggleSelection** keeps one item selected, and choosing that item again clears the value.

## Custom selection style

`Color` sets the built-in selected fill. `SelectedClass` adds classes on the selected item so you can replace that fill.

Built-in classes:

- `sb-toggle-selected-soft`
- `sb-toggle-selected-gradient`
- `sb-toggle-selected-striped`

```razor
<SbToggleGroup TValue="string" SelectedClass="sb-toggle-selected-gradient" @bind-Value="value">
    <SbToggleItem Value="@("antimatter")" Text="Antimatter" />
    <SbToggleItem Value="@("dark-matter")" Text="Dark Matter" />
    <SbToggleItem Value="@("dark-energy")" Text="Dark Energy" />
</SbToggleGroup>
```

## Custom content

`SbToggleItem` child content is `RenderFragment<bool>`. The boolean is true when that item is selected.

```razor
<SbToggleItem TValue="string" Value="@("pro")">
    <ChildContent Context="selected">
        <span>@(selected ? "Selected" : "Pro")</span>
    </ChildContent>
</SbToggleItem>
```

## Examples

### Single selection

```razor
<SbToggleGroup TValue="string" CheckMark="true" @bind-Value="alignment">
    <SbToggleItem Value="@("left")" Text="Left" />
    <SbToggleItem Value="@("center")" Text="Center" />
    <SbToggleItem Value="@("right")" Text="Right" />
</SbToggleGroup>
```

### Multi selection

```razor
<SbToggleGroup TValue="string"
               SelectionMode="SbToggleSelectionMode.MultiSelection"
               @bind-Values="permissions">
    <SbToggleItem Value="@("read")" Text="Read" />
    <SbToggleItem Value="@("write")" Text="Write" />
</SbToggleGroup>
```

### Toggle selection

```razor
<SbToggleGroup TValue="string"
               SelectionMode="SbToggleSelectionMode.ToggleSelection"
               @bind-Value="filter">
    <SbToggleItem Value="@("open")" Text="Open" />
    <SbToggleItem Value="@("closed")" Text="Closed" />
</SbToggleGroup>
```

## CSS classes

- `sb-toggle-group` — group
- `sb-toggle-group--horizontal` / `sb-toggle-group--vertical`
- `sb-toggle-group--outlined`
- `sb-toggle-group--full`
- `sb-toggle-group--{color}` — selected color
- `sb-toggle-item` — item button
- `sb-toggle-item--selected`
- `sb-toggle-item--disabled`
- `sb-toggle-item__mark` — check slot
- `sb-toggle-item__content`

## Accessibility

- The group uses `role="group"`.
- A visible label is referenced with `aria-labelledby`. Otherwise set `AriaLabel`.
- Each item uses `role="button"` and `aria-pressed`, and responds to Enter and Space.
- Disabled items and a disabled group do not change the value.
