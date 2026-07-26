namespace SufiChain.SufiBlazor.Components.Builder;

/// <summary>
/// Cascading Blazor-native drag payload shared between <see cref="SbDraggableItem"/> and
/// <see cref="SbDropZone"/>. HTML5 DataTransfer is not required for same-circuit drops.
/// </summary>
public sealed class SbDragSession
{
    /// <summary>
    /// Whether a drag is currently in progress.
    /// </summary>
    public bool IsDragging { get; private set; }

    /// <summary>
    /// Payload supplied by the drag source.
    /// </summary>
    public object? Data { get; private set; }

    /// <summary>
    /// Optional type filter key (e.g. <c>cms-element</c>).
    /// </summary>
    public string? ItemType { get; private set; }

    /// <summary>
    /// Raised when drag begins or ends so drop affordances can refresh.
    /// </summary>
    public event Action? Changed;

    /// <summary>
    /// Starts a drag session.
    /// </summary>
    public void Begin(object? data, string? itemType = null)
    {
        IsDragging = true;
        Data = data;
        ItemType = itemType;
        Changed?.Invoke();
    }

    /// <summary>
    /// Clears the drag session.
    /// </summary>
    public void End()
    {
        if (!IsDragging && Data is null && ItemType is null)
        {
            return;
        }

        IsDragging = false;
        Data = null;
        ItemType = null;
        Changed?.Invoke();
    }

    /// <summary>
    /// Returns whether the current payload is accepted by the given type filter.
    /// </summary>
    public bool IsAccepted(string[]? acceptedTypes)
    {
        if (!IsDragging)
        {
            return false;
        }

        if (acceptedTypes is null || acceptedTypes.Length == 0)
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(ItemType))
        {
            return false;
        }

        return acceptedTypes.Contains(ItemType, StringComparer.OrdinalIgnoreCase);
    }
}
