using System;
using System.Collections.Generic;

namespace SufiChain.SufiBlazor.Contracts.Editors;

/// <summary>
/// Options for configuring the markdown editor toolbar system.
/// </summary>
public class MdToolbarOptions
{
    public List<Type> Contributors { get; } = new();
    public bool IncludeDefaultItems { get; set; } = true;

    public List<string> GroupOrder { get; set; } = new()
    {
        "history",
        "formatting",
        "heading",
        "list",
        "insert",
        "view",
        "custom",
        "actions"
    };

    public MdToolbarOptions AddContributor<T>() where T : IMdToolbarContributor
    {
        Contributors.Add(typeof(T));
        return this;
    }

    public MdToolbarOptions AddContributor(Type contributorType)
    {
        if (!typeof(IMdToolbarContributor).IsAssignableFrom(contributorType))
        {
            throw new ArgumentException(
                $"Type {contributorType.Name} does not implement IMdToolbarContributor",
                nameof(contributorType));
        }

        Contributors.Add(contributorType);
        return this;
    }
}
