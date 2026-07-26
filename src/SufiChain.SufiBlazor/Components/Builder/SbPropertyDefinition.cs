using Microsoft.AspNetCore.Components;

namespace SufiChain.SufiBlazor.Components.Builder;

/// <summary>
/// Defines a property for the property grid.
/// </summary>
public class SbPropertyDefinition
{
    /// <summary>
    /// Unique identifier for the property.
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Display name for the property.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Display label (alias for Name).
    /// </summary>
    public string Label
    {
        get => Name;
        set => Name = value;
    }
    
    /// <summary>
    /// Property description or help text.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// The type of editor to use.
    /// </summary>
    public SbPropertyEditorType EditorType { get; set; } = SbPropertyEditorType.Text;
    
    /// <summary>
    /// Current value of the property.
    /// </summary>
    public object? Value { get; set; }
    
    /// <summary>
    /// Default value for the property.
    /// </summary>
    public object? DefaultValue { get; set; }
    
    /// <summary>
    /// Category/group for the property.
    /// </summary>
    public string? Category { get; set; }
    
    /// <summary>
    /// Whether the property is read-only.
    /// </summary>
    public bool IsReadOnly { get; set; }
    
    /// <summary>
    /// Whether the property is required.
    /// </summary>
    public bool Required { get; set; }
    
    /// <summary>
    /// Options for select/dropdown editors.
    /// </summary>
    public IEnumerable<SbPropertyOption>? Options { get; set; }

    /// <summary>
    /// Optional key used by hosts to resolve a <see cref="CustomEditor"/> (e.g. module pickers).
    /// </summary>
    public string? EditorKey { get; set; }

    /// <summary>
    /// Custom editor fragment rendered when <see cref="EditorType"/> is <see cref="SbPropertyEditorType.Custom"/>.
    /// </summary>
    public RenderFragment? CustomEditor { get; set; }
    
    /// <summary>
    /// Creates a new property definition.
    /// </summary>
    public SbPropertyDefinition() { }
    
    /// <summary>
    /// Creates a new property definition with id and name.
    /// </summary>
    public SbPropertyDefinition(string id, string name, SbPropertyEditorType editorType = SbPropertyEditorType.Text)
    {
        Id = id;
        Name = name;
        EditorType = editorType;
    }
}

/// <summary>
/// Represents an option for property editors.
/// </summary>
public class SbPropertyOption
{
    /// <summary>
    /// Display label.
    /// </summary>
    public string Label { get; set; } = string.Empty;
    
    /// <summary>
    /// Option value.
    /// </summary>
    public object? Value { get; set; }
    
    /// <summary>
    /// Creates a new property option.
    /// </summary>
    public SbPropertyOption() { }
    
    /// <summary>
    /// Creates a new property option with label and value.
    /// </summary>
    public SbPropertyOption(string label, object? value = null)
    {
        Label = label;
        Value = value ?? label;
    }
}

/// <summary>
/// Types of property editors.
/// </summary>
public enum SbPropertyEditorType
{
    Text,
    Number,
    Boolean,
    Checkbox,
    Select,
    Color,
    Date,
    TextArea,
    /// <summary>Host-supplied editor via <see cref="SbPropertyDefinition.CustomEditor"/>.</summary>
    Custom
}
