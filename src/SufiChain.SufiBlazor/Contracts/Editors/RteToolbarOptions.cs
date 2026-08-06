using System;
using System.Collections.Generic;

namespace SufiChain.SufiBlazor.Contracts.Editors;

/// <summary>
/// Options for configuring the rich text editor toolbar system.
/// </summary>
public class RteToolbarOptions
{
    /// <summary>
    /// The list of toolbar contributor types registered with the system.
    /// These will be resolved from DI and invoked to configure the toolbar.
    /// </summary>
    public List<Type> Contributors { get; } = new();

    /// <summary>
    /// Whether to include the default toolbar items.
    /// If false, only contributed items will be shown.
    /// </summary>
    public bool IncludeDefaultItems { get; set; } = true;

    /// <summary>
    /// The groups and their order. Groups not in this list will appear at the end.
    /// </summary>
    public List<string> GroupOrder { get; set; } = new()
    {
        "history",      // Undo/Redo
        "heading",      // Headers
        "formatting",   // Bold, Italic, etc.
        "list",         // Lists
        "alignment",    // Text alignment
        "insert",       // Links, Images, Files
        "blocks",       // Blockquote, Code
        "custom",       // Custom contributed items
        "actions"       // Clear formatting, etc.
    };

    /// <summary>
    /// Add a toolbar contributor type.
    /// </summary>
    /// <typeparam name="T">The contributor type implementing IRteToolbarContributor.</typeparam>
    public RteToolbarOptions AddContributor<T>() where T : IRteToolbarContributor
    {
        Contributors.Add(typeof(T));
        return this;
    }

    /// <summary>
    /// Add a toolbar contributor type.
    /// </summary>
    /// <param name="contributorType">The contributor type implementing IRteToolbarContributor.</param>
    public RteToolbarOptions AddContributor(Type contributorType)
    {
        if (!typeof(IRteToolbarContributor).IsAssignableFrom(contributorType))
        {
            throw new ArgumentException(
                $"Type {contributorType.Name} does not implement IRteToolbarContributor",
                nameof(contributorType));
        }

        Contributors.Add(contributorType);
        return this;
    }
}
