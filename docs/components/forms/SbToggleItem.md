# SbToggleItem

One option inside an [SbToggleGroup](./SbToggleGroup.md). The item must be nested in a group of the same `TValue`.

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| Value | TValue? | null | Value selected when the item is pressed |
| Text | string? | null | Text shown when ChildContent is not set. Falls back to `Value` |
| Disabled | bool | false | Prevents this item from being toggled |
| SelectedIcon | string | check | Icon in the check slot when selected |
| UnselectedIcon | string? | null | Icon in the check slot when not selected |
| Class | string? | null | Additional CSS classes |
| Style | string? | null | Inline styles |

## Templates

| Slot | Type | Description |
|------|------|-------------|
| ChildContent | RenderFragment\<bool\> | Custom content. The boolean is true when this item is selected. Replaces Text. |

## Example

```razor
<SbToggleGroup TValue="bool" @bind-Value="useWorkspace" FullWidth="true">
    <SbToggleItem TValue="bool" Value="true" Text="Use workspace connection" />
    <SbToggleItem TValue="bool" Value="false" Text="Use a different connection" />
</SbToggleGroup>
```
