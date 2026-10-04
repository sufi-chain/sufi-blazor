using System.Linq.Expressions;

namespace SufiChain.SufiBlazor.Components.Forms;

/// <summary>
/// Member names from a failed save. A field sets <c>aria-invalid</c> when its name matches.
/// </summary>
public sealed class SbSaveFieldErrorSet
{
    public static SbSaveFieldErrorSet Empty { get; } = new(Array.Empty<string>());

    private readonly HashSet<string> _members;

    public SbSaveFieldErrorSet(IEnumerable<string> members)
    {
        _members = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var member in members)
        {
            if (string.IsNullOrWhiteSpace(member))
            {
                continue;
            }

            _members.Add(member);
            var dot = member.LastIndexOf('.');
            if (dot >= 0 && dot < member.Length - 1)
            {
                _members.Add(member[(dot + 1)..]);
            }
        }
    }

    public bool Contains(string? fieldName) =>
        !string.IsNullOrEmpty(fieldName) && _members.Contains(fieldName);

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
}
