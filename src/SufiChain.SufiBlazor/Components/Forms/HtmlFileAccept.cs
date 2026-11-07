namespace SufiChain.SufiBlazor.Components.Forms;

/// <summary>
/// Builds a valid HTML file-input <c>accept</c> value.
/// Tokens are comma-separated dotted extensions (<c>.png</c>), MIME types (<c>image/png</c>),
/// or the wildcards <c>image/*</c>, <c>audio/*</c>, and <c>video/*</c>.
/// Callers may pass bare extensions, a leading dot, <c>*.png</c>, mixed case, extra whitespace,
/// duplicates, or spaces and semicolons where commas belong. A space-separated dotted list is one
/// HTML token, and the Windows picker then hides files whose extension is in that list.
/// </summary>
public static class HtmlFileAccept
{
    /// <summary>
    /// Returns a canonical accept attribute, or an empty string when <paramref name="value"/> has no usable token.
    /// A lone <c>*/*</c> is preserved. It is dropped when real type tokens are present, so the picker keeps its type filter.
    /// </summary>
    public static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var tokens = new List<string>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var raw in SplitTokens(value))
        {
            if (!TryCanonicalize(raw, out var token))
            {
                continue;
            }

            if (seen.Add(token))
            {
                tokens.Add(token);
            }
        }

        if (tokens.Count > 1)
        {
            tokens.RemoveAll(token => token == "*/*");
        }

        return string.Join(",", tokens);
    }

    /// <summary>
    /// True when <paramref name="fileName"/> or <paramref name="mimeType"/> matches the normalized accept list.
    /// An empty list, or a lone <c>*/*</c>, does not add a type constraint.
    /// </summary>
    public static bool Allows(string? accept, string? fileName, string? mimeType = null)
    {
        var normalized = Normalize(accept);
        if (normalized.Length == 0 || normalized == "*/*")
        {
            return true;
        }

        var name = fileName ?? string.Empty;
        var mime = (mimeType ?? string.Empty).Trim();
        foreach (var token in normalized.Split(','))
        {
            if (token is "image/*" or "audio/*" or "video/*")
            {
                var prefix = token[..^1];
                if (mime.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                continue;
            }

            if (token.Contains('/'))
            {
                if (mime.Equals(token, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                continue;
            }

            if (name.EndsWith(token, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static IEnumerable<string> SplitTokens(string value)
    {
        var parts = value.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var part in parts)
        {
            foreach (var piece in part.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                yield return piece;
            }
        }
    }

    private static bool TryCanonicalize(string raw, out string token)
    {
        token = string.Empty;
        var text = raw.Trim().Trim('"').Trim('\'').Trim();
        if (text.Length == 0)
        {
            return false;
        }

        if (text.Equals("*/*", StringComparison.OrdinalIgnoreCase))
        {
            token = "*/*";
            return true;
        }

        if (text.Equals("image/*", StringComparison.OrdinalIgnoreCase)
            || text.Equals("audio/*", StringComparison.OrdinalIgnoreCase)
            || text.Equals("video/*", StringComparison.OrdinalIgnoreCase))
        {
            token = text.ToLowerInvariant();
            return true;
        }

        if (text.Contains('/'))
        {
            return TryMime(text, out token);
        }

        return TryExtension(text, out token);
    }

    private static bool TryMime(string text, out string token)
    {
        token = string.Empty;
        var value = text.Trim().ToLowerInvariant();
        var slash = value.IndexOf('/');
        if (slash <= 0 || slash == value.Length - 1)
        {
            return false;
        }

        var type = value[..slash];
        var subtype = value[(slash + 1)..];
        if (!IsMimeToken(type) || !IsMimeToken(subtype))
        {
            return false;
        }

        token = type + "/" + subtype;
        return true;
    }

    private static bool TryExtension(string text, out string token)
    {
        token = string.Empty;
        var value = text.Trim();
        if (value.StartsWith("*.", StringComparison.Ordinal))
        {
            value = value[2..];
        }
        else if (value.StartsWith('.'))
        {
            value = value[1..];
        }
        else if (value.Contains('.'))
        {
            return false;
        }

        value = value.Trim().ToLowerInvariant();
        if (!IsExtensionBody(value))
        {
            return false;
        }

        token = "." + value;
        return true;
    }

    private static bool IsExtensionBody(string value)
    {
        var segments = value.Split('.');
        if (segments.Length is < 1 or > 2)
        {
            return false;
        }

        foreach (var segment in segments)
        {
            if (segment.Length is < 1 or > 16)
            {
                return false;
            }

            foreach (var ch in segment)
            {
                if (ch is (>= 'a' and <= 'z') or (>= '0' and <= '9') or '_' or '-')
                {
                    continue;
                }

                return false;
            }
        }

        return true;
    }

    private static bool IsMimeToken(string value)
    {
        if (value.Length is < 1 or > 127)
        {
            return false;
        }

        for (var i = 0; i < value.Length; i++)
        {
            var ch = value[i];
            var allowed = ch is (>= 'a' and <= 'z') or (>= '0' and <= '9')
                or '!' or '#' or '$' or '&' or '^' or '_' or '.' or '+' or '-';
            if (!allowed)
            {
                return false;
            }

            if (i == 0 && ch is not (>= 'a' and <= 'z') and not (>= '0' and <= '9'))
            {
                return false;
            }
        }

        return true;
    }
}
