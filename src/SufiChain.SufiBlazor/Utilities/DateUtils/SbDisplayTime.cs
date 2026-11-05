using System.Globalization;

namespace SufiChain.SufiBlazor.Utilities.DateUtils;

/// <summary>
/// Converts a UTC instant for display. Follows the same IANA and Windows id lookup as
/// CalendarTimeZoneResolver and does not keep an offset table.
/// An unset id is Asia/Tehran (UTC+3:30), not the server zone.
/// </summary>
public static class SbDisplayTime
{
    public const string DefaultTimeZoneId = "Asia/Tehran";

    public const string UnspecifiedTimeZone = "Unspecified";

    public static TimeZoneInfo Resolve(string? timeZoneId)
    {
        var id = Normalize(timeZoneId);
        return TryFind(id) ?? TryFind(DefaultTimeZoneId) ?? TimeZoneInfo.Utc;
    }

    public static DateTimeOffset ToDisplayTime(DateTimeOffset value, string? timeZoneId) =>
        TimeZoneInfo.ConvertTime(value, Resolve(timeZoneId));

    public static string Format(DateTimeOffset value, string format, string? timeZoneId, CultureInfo? culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        return ToDisplayTime(value, timeZoneId).ToString(format, culture);
    }

    private static string? Normalize(string? timeZoneId)
    {
        if (string.IsNullOrWhiteSpace(timeZoneId))
        {
            return null;
        }

        var trimmed = timeZoneId.Trim();
        return trimmed.Equals(UnspecifiedTimeZone, StringComparison.OrdinalIgnoreCase) ? null : trimmed;
    }

    private static TimeZoneInfo? TryFind(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        if (TimeZoneInfo.TryFindSystemTimeZoneById(id, out var zone))
        {
            return zone;
        }

        if (TimeZoneInfo.TryConvertIanaIdToWindowsId(id, out var windowsId)
            && TimeZoneInfo.TryFindSystemTimeZoneById(windowsId, out zone))
        {
            return zone;
        }

        if (TimeZoneInfo.TryConvertWindowsIdToIanaId(id, out var ianaId)
            && TimeZoneInfo.TryFindSystemTimeZoneById(ianaId, out zone))
        {
            return zone;
        }

        return null;
    }
}
