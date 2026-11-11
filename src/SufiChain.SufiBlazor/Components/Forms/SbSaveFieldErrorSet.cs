using System.Linq.Expressions;

namespace SufiChain.SufiBlazor.Components.Forms;

/// <summary>
/// Member names from a failed save. A field sets <c>aria-invalid</c> when its name matches
/// and shows the validation message when the failure carried one.
/// </summary>
public sealed class SbSaveFieldErrorSet
{
    public static SbSaveFieldErrorSet Empty { get; } = new(Array.Empty<string>());

    private readonly HashSet<string> _members;
    private readonly Dictionary<string, string> _messages;

    public SbSaveFieldErrorSet(IEnumerable<string> members)
        : this(members.Select(member => (member, (string?)null)))
    {
    }

    public SbSaveFieldErrorSet(IEnumerable<(string Member, string? Message)> errors)
    {
        _members = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        _messages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var (member, message) in errors)
        {
            if (string.IsNullOrWhiteSpace(member))
            {
                continue;
            }

            Add(member, message);
            var dot = member.LastIndexOf('.');
            if (dot >= 0 && dot < member.Length - 1)
            {
                Add(member[(dot + 1)..], message);
            }
        }
    }

    public bool Contains(string? fieldName) =>
        !string.IsNullOrEmpty(fieldName) && _members.Contains(fieldName);

    public string? Message(string? fieldName) =>
        !string.IsNullOrEmpty(fieldName) && _messages.TryGetValue(fieldName, out var message)
            ? message
            : null;

    public bool Matches<T>(Expression<Func<T>>? expression) =>
        Contains(MemberName(expression));

    public static string? MemberName<T>(Expression<Func<T>>? expression)
    {
        var body = expression?.Body;
        if (body is UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked } unary)
        {
            body = unary.Operand;
        }

        return body is MemberExpression member ? member.Member.Name : null;
    }

    private void Add(string member, string? message)
    {
        _members.Add(member);
        if (!string.IsNullOrWhiteSpace(message))
        {
            _messages[member] = message;
        }
    }
}
