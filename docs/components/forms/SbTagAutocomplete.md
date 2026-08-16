# SbTagAutocomplete

Multi-select autocomplete with chip display. Search remote or local lists, add multiple tags, and optionally create new tags from typed text.

Search text uses Blazor's `oninput` bind pipeline so async search rerenders preserve the newest browser value.

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| SelectedItems | IReadOnlyList\<TItem\> | empty | Current selected tags (two-way via `SelectedItemsChanged`) |
| SelectedItemsChanged | EventCallback\<IReadOnlyList\<TItem\>\> | — | Fired when selection changes |
| TextField | Func\<TItem, string\> | `ToString()` | Display text for items and chips |
| ValueField | Func\<TItem, object\> | identity | Unique value for deduplication |
| SearchFunc | Func\<string, Task\<IEnumerable\<TItem\>\>\>? | null | Async search; required for dropdown results |
| OnCreateAsync | Func\<string, Task\<TItem?\>\>? | null | Creates a new item from search text when `AllowCreate` is true |
| AllowCreate | bool | false | Show "Create …" option for unmatched search text |
| MinLength | int | 0 | Minimum characters before search runs |
| DebounceMs | int | 300 | Search debounce interval |
| MaxResults | int | 20 | Maximum dropdown options |
| MaxTags | int? | null | Optional tag limit; shows counter when set |
| Placeholder | string? | null | Input placeholder (defaults to localized `AddTag_Placeholder`) |
| Label | string? | null | Field label |
| Required | bool | false | Shows required indicator |
| Disabled | bool | false | Disables input and chip removal |
| ReadOnly | bool | false | Read-only display |
| Id | string? | null | Input element id |
| Class / Style | string? | null | Root element styling |

## Templates

| Slot | Type | Description |
|------|------|-------------|
| ItemTemplate | RenderFragment\<TItem\> | Custom dropdown option |
| ChipTemplate | RenderFragment\<TItem\> | Custom chip content |
| NoResultsTemplate | RenderFragment | Empty search state |

## Keyboard

- **ArrowUp/Down** — Highlight options (including create row)
- **Enter** — Select highlighted option or create tag
- **Backspace** — Remove last chip when input is empty
- **Escape** — Close dropdown

## Example

```razor
<SbTagAutocomplete TItem="TagDto"
                   @bind-SelectedItems="selectedTags"
                   TextField="t => t.Name"
                   ValueField="t => t.Id"
                   SearchFunc="SearchTagsAsync"
                   AllowCreate="true"
                   OnCreateAsync="CreateTagAsync"
                   Label="Tags"
                   MaxTags="5" />
```

## See also

- [SbTagInput](./SbTagInput.md) — free-form tag entry without autocomplete
- [SbAutocomplete](./SbAutocomplete.md) — single-value autocomplete
