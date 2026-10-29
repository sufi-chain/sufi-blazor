namespace SufiChain.SufiBlazor.Components.Forms.Internal;

/// <summary>
/// Keeps a server-rendered text input in step with what the user is typing on a slow circuit.
/// <para>
/// The element renders <see cref="DomValue"/>, and <see cref="DomValue"/> changes only when the
/// host genuinely sets the text (load, clear, reset after save, programmatic fill) or the component
/// itself acts (Clear, slugify). Typing changes only <see cref="Draft"/>.
/// A <c>@bind-Value</c> echo is not a host edit: the server has accepted a keystroke, but on a
/// slow circuit the browser already holds later characters. Writing that echo into the value
/// attribute makes Blazor assign <c>element.value</c> and drops them. Account and profile forms
/// echo every keystroke, which is why they lost text after the 9.1 guard (sufi-chain#533).
/// The conversation composer already skips this write (sufi-chain#386).
/// </para>
/// </summary>
internal sealed class SbTextInputSync
{
    /// <summary>
    /// How many values stay eligible for echo and stale-render rejection. A lagging host can only
    /// replay a value it was told about, and the oldest ones are dropped past this backlog.
    /// </summary>
    private const int MaxRememberedValues = 32;

    private readonly List<string> _sent = new(MaxRememberedValues + 1);
    private readonly List<string> _superseded = new(MaxRememberedValues + 1);

    /// <summary>
    /// Text written into the rendered <c>value</c> attribute. Typing never changes it.
    /// </summary>
    public string DomValue { get; private set; } = string.Empty;

    /// <summary>
    /// Text the browser holds, including input that has not round-tripped to the host yet.
    /// </summary>
    public string Draft { get; private set; } = string.Empty;

    /// <summary>
    /// Last value the host handed to this control. A repeat is an unrelated re-render.
    /// </summary>
    public string? LastParentValue { get; private set; }

    /// <summary>
    /// Set when the DOM must be rewritten through JS because the rendered value did not change.
    /// </summary>
    public bool DomSyncPending { get; private set; }

    /// <summary>
    /// Adopts an external host value, or ignores a re-render or echo that must not reach the DOM.
    /// </summary>
    /// <returns>True when the host genuinely changed the text.</returns>
    public bool ObserveParent(string? parentValue)
    {
        var incoming = parentValue ?? string.Empty;

        if (LastParentValue == null)
        {
            LastParentValue = incoming;
            Draft = incoming;
            DomValue = incoming;
            return true;
        }

        if (string.Equals(incoming, LastParentValue, StringComparison.Ordinal))
        {
            // A poll, a validation pass or any other unrelated re-render replays the same value.
            return false;
        }

        if (string.Equals(incoming, Draft, StringComparison.Ordinal))
        {
            // The host caught up with a keystroke this circuit has already accepted.
            // Leave DomValue alone. Mirroring the echo into the rendered value is what
            // dropped characters on 9.5: the render arrives after the user has typed more,
            // and Blazor then sets element.value to the older echo. Clear-button and
            // validation UI follow Draft, so they do not need this write.
            Acknowledge(incoming);
            LastParentValue = incoming;
            return false;
        }

        if (Contains(_sent, incoming))
        {
            // A lagging echo of an earlier keystroke. Letting it through would drop characters.
            return false;
        }

        if (Contains(_superseded, incoming))
        {
            // The stale side of a component action (Clear, slugify): the host has not caught up.
            return false;
        }

        // A genuine external set. It supersedes anything still in flight, and the DOM has to be
        // written through JS when the rendered value did not change (a clear while the rendered
        // value was already empty).
        LastParentValue = incoming;
        _sent.Clear();
        _superseded.Clear();
        Draft = incoming;
        DomSyncPending |= string.Equals(DomValue, incoming, StringComparison.Ordinal);
        DomValue = incoming;
        return true;
    }

    /// <summary>
    /// Records a value typed in the browser.
    /// </summary>
    /// <returns>True when the value differs from the current draft.</returns>
    public bool WriteInput(string? value)
    {
        var incoming = value ?? string.Empty;

        if (string.Equals(incoming, Draft, StringComparison.Ordinal))
        {
            return false;
        }

        Draft = incoming;
        _superseded.Clear();
        _sent.Add(incoming);
        Trim(_sent);
        return true;
    }

    /// <summary>
    /// Sets the text from a component action (Clear, slugify, generate). The DOM is synced through
    /// JS as well, because the rendered value may already equal the new text.
    /// </summary>
    public void SetFromComponent(string? value)
    {
        var next = value ?? string.Empty;

        if (LastParentValue != null && !string.Equals(LastParentValue, next, StringComparison.Ordinal))
        {
            _superseded.Add(LastParentValue);
            Trim(_superseded);
        }

        _sent.Clear();
        Draft = next;
        DomSyncPending |= string.Equals(DomValue, next, StringComparison.Ordinal);
        DomValue = next;
    }

    /// <summary>
    /// Marks the pending JS write as done.
    /// </summary>
    public void MarkDomSynced() => DomSyncPending = false;

    private void Acknowledge(string value)
    {
        var index = _sent.LastIndexOf(value);
        if (index >= 0)
        {
            _sent.RemoveRange(0, index + 1);
        }
        else
        {
            _sent.Clear();
        }

        _superseded.Clear();
    }

    private static bool Contains(List<string> values, string value)
    {
        for (var i = values.Count - 1; i >= 0; i--)
        {
            if (string.Equals(values[i], value, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static void Trim(List<string> values)
    {
        if (values.Count > MaxRememberedValues)
        {
            values.RemoveAt(0);
        }
    }
}
