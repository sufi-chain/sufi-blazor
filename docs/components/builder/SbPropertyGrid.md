# SbPropertyGrid

A grid-based property editor that renders SufiBlazor form controls from property definitions. Useful for inspectors and schema-driven editors.

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| Properties | List&lt;SbPropertyDefinition&gt; | [] | Property definitions to display |
| Class | string? | null | Additional CSS classes |

## Events

| Event | Type | Description |
|-------|------|-------------|
| OnPropertyChanged | EventCallback&lt;SbPropertyChangeEventArgs&gt; | Fired when any property value changes |

## SbPropertyDefinition Class

| Property | Type | Description |
|----------|------|-------------|
| Id | string | Property identifier (used as input id) |
| Name / Label | string | Display label |
| Value | object? | Current value |
| EditorType | SbPropertyEditorType | Type of editor to render |
| Required | bool | Whether property is required |
| IsReadOnly | bool | Disables the editor |
| Options | IEnumerable&lt;SbPropertyOption&gt;? | Options for Select editor type |
| EditorKey | string? | Optional host key for resolving a custom editor |
| CustomEditor | RenderFragment? | Host-supplied editor when `EditorType` is `Custom` |

## SbPropertyEditorType Enum

| Value | SufiBlazor control |
|-------|--------------------|
| Text | `SbTextField` (pass `ValueExpression` when not using `@bind-Value`) |
| Number | `SbNumberField` |
| Boolean / Checkbox | `SbCheckbox` |
| Select | `SbSimpleSelect` |
| Color | `SbColorPicker` |
| Date | `SbDatePicker` |
| TextArea | `SbTextArea` |
| Custom | `CustomEditor` fragment (or `-` if missing) |

## Examples

### Basic Property Grid

```razor
<SbPropertyGrid Properties="@properties" OnPropertyChanged="HandleChange" />

@code {
    private List<SbPropertyDefinition> properties = new()
    {
        new() { Id = "name", Name = "Name", EditorType = SbPropertyEditorType.Text, Value = "My Element" },
        new() { Id = "width", Name = "Width", EditorType = SbPropertyEditorType.Number, Value = 100 },
        new() { Id = "visible", Name = "Visible", EditorType = SbPropertyEditorType.Checkbox, Value = true }
    };
    
    private void HandleChange(SbPropertyChangeEventArgs args)
    {
        Console.WriteLine($"{args.Property.Name} = {args.NewValue}");
    }
}
```

### Custom module picker

```razor
@code {
    private SbPropertyDefinition fileProperty = new("fileItemId", "Image", SbPropertyEditorType.Custom)
    {
        CustomEditor = builder =>
        {
            builder.OpenComponent<MyFilePicker>(0);
            // ... bind Value / ValueChanged
            builder.CloseComponent();
        }
    };
}
```
