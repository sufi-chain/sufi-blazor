# Builder Components

Low-level primitives for **visual editors, page builders, and design tools**. Compose these to let users drag, reorder, inspect, and configure content on screen.

Builder components are **not** a turnkey page builder — they are building blocks you wire together for product-specific editors.

## When to use Builder

Use Builder when building:

- Landing page or email template builders
- Form or workflow designers
- Menu / navigation editors
- CMS block editors
- Layer panels and property inspectors (Figma-style sidebar UX)

For general UI (buttons, forms, tables), use the standard component families instead.

## Component groups

| Group | Components | Purpose |
| --- | --- | --- |
| Drag and drop | `SbDraggableItem`, `SbDropZone`, `SbDragHandle` | Palette → canvas drag; typed drop targets |
| Reorder | `SbSortableList<T>` | Drag-to-reorder lists (tasks, tabs, layers) |
| Inspector | `SbInspectorPanel`, `SbInspectorSection`, `SbPropertyGrid` | Sidebar for selected element properties |
| Layout | `SbSplitPane`, `SbResizable` | Canvas + sidebar split; resizable panels/widgets |
| Actions | `SbInlineToolbar`, `SbAddButton` | Context toolbar on selection; add-block affordance |

## Typical composition

```
[SbSplitPane]
  FirstPane  → canvas (SbDropZone) + SbInlineToolbar on selection
  SecondPane → SbInspectorPanel
                 ├─ SbSortableList (layers)
                 └─ SbPropertyGrid (properties)

Palette (sidebar or drawer):
  SbDraggableItem per block type → drop onto SbDropZone

Add affordance:
  SbAddButton with dropdown options for block types
```

## Drag and drop

### SbDraggableItem

Wraps content for HTML5 drag-and-drop. Carries `Data` and optional `ItemType` for drop-target filtering.

```razor
<SbDraggableItem Data="@block" ItemType="block">
    <SbCard>@block.Label</SbCard>
</SbDraggableItem>
```

### SbDropZone

Accepts dropped items. Filter with `AcceptedTypes`. Fires `OnItemDropped` with coordinates and payload.

```razor
<SbDropZone AcceptedTypes="@(new[] { "block" })"
            PlaceholderText="Drop blocks here"
            OnItemDropped="HandleDrop" />
```

### SbDragHandle

Optional explicit drag handle inside a sortable or draggable row.

## Reorderable lists

### SbSortableList\<T\>

Generic list with drag-to-reorder. Supports vertical/horizontal orientation, optional remove button, empty template, and disabled state.

```razor
<SbSortableList Items="@layers" @bind-Items="layers" Removable="true">
    <ItemTemplate Context="layer">
        <span>@layer.Name</span>
    </ItemTemplate>
</SbSortableList>
```

Common uses: task priority, tab order, layer stacking in an inspector panel.

## Property inspector

### SbInspectorPanel / SbInspectorSection

Collapsible sidebar panel for inspecting the selected element. Use `Sections` slot for grouped `SbInspectorSection` blocks (Layout, Appearance, Typography, etc.).

### SbPropertyGrid

Auto-generates editors from `SbPropertyDefinition` list. Editor types:

| Type | Control |
| --- | --- |
| `Text` | Text input |
| `Number` | Numeric input |
| `Checkbox` | Boolean |
| `Select` | Dropdown (`SbPropertyOption[]`) |
| `Color` | Color picker |

Fires `OnPropertyChanged` with `SbPropertyChangeEventArgs` (property + new value).

```razor
<SbInspectorPanel Title="Button Properties">
    <SbPropertyGrid Properties="@buttonProperties" OnPropertyChanged="UpdateButton" />
</SbInspectorPanel>
```

Build definitions dynamically from the selected element model.

## Layout chrome

### SbSplitPane

Resizable two-pane container (horizontal or vertical split). Use for canvas + inspector, or preview + code.

### SbResizable

Resize handles on individual panels or widgets.

## Context actions

### SbInlineToolbar

Floating toolbar for selection-specific actions (move, resize, duplicate, delete). Positions: Top, Bottom, Left, Right, or Float with X/Y.

Actions are `SbToolbarAction` items with icons, tooltips, active state, danger variant, and separators.

### SbAddButton

"Add" button for builder interfaces. Direct click when no options; dropdown menu when `Options` list is provided (`SbAddOption`).

## Example scenarios

| Scenario | Components |
| --- | --- |
| Page builder | `SbDraggableItem` palette + `SbDropZone` canvas + `SbInspectorPanel` + `SbPropertyGrid` |
| Menu editor | `SbSortableList` reorder + `SbPropertyGrid` per item + `SbAddButton` |
| Form designer | Drag fields into `SbDropZone`; reorder with `SbSortableList`; edit with inspector |
| Layer panel | `SbSortableList` inside `SbInspectorPanel`; visibility/lock toggles in item template |

## Limitations

- **HTML5 DnD** — `SbDraggableItem` / `SbDropZone` use browser drag-and-drop. Cross-list moves and complex gestures are your responsibility; within-list reordering is handled by `SbSortableList`.
- **No demo pages yet** — Builder components are documented under `docs/components/builder/` but not yet in the SufiBlazor demo gallery menu.
- **Compose yourself** — There is no single `SbPageBuilder` component; you assemble primitives for your domain model.

## Per-component reference

| Component | Doc |
| --- | --- |
| SbAddButton | [components/builder/SbAddButton.md](components/builder/SbAddButton.md) |
| SbDraggableItem | [components/builder/SbDraggableItem.md](components/builder/SbDraggableItem.md) |
| SbDragHandle | [components/builder/SbDragHandle.md](components/builder/SbDragHandle.md) |
| SbDropZone | [components/builder/SbDropZone.md](components/builder/SbDropZone.md) |
| SbInlineToolbar | [components/builder/SbInlineToolbar.md](components/builder/SbInlineToolbar.md) |
| SbInspectorPanel | [components/builder/SbInspectorPanel.md](components/builder/SbInspectorPanel.md) |
| SbInspectorSection | [components/builder/SbInspectorSection.md](components/builder/SbInspectorSection.md) |
| SbPropertyGrid | [components/builder/SbPropertyGrid.md](components/builder/SbPropertyGrid.md) |
| SbResizable | [components/builder/SbResizable.md](components/builder/SbResizable.md) |
| SbSortableList | [components/builder/SbSortableList.md](components/builder/SbSortableList.md) |
| SbSplitPane | [components/builder/SbSplitPane.md](components/builder/SbSplitPane.md) |

## Related docs

- [Overview](overview.md)
- [Components](components.md)
- [DataGrid](data-grid.md)
