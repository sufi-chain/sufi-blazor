using System.Globalization;

namespace SufiChain.SufiBlazor.Utilities.DateUtils;

/// <summary>
/// Helper class for calendar operations supporting multiple calendar systems.
/// </summary>
public static class SbCalendarHelper
{
    /// <summary>
    /// Derives the calendar system from the given culture (e.g. fa-IR → Persian, ar-SA → Hijri).
    /// Use this when the component should follow the UI culture's calendar.
    /// </summary>
    public static SbCalendarSystem GetCalendarSystemFromCulture(CultureInfo culture)
    {
        if (culture.Calendar is PersianCalendar) return SbCalendarSystem.Persian;
        if (culture.Calendar is HijriCalendar) return SbCalendarSystem.Hijri;
        return SbCalendarSystem.Gregorian;
    }

    /// <summary>
    /// Gets the appropriate CultureInfo for the specified calendar system.
    /// </summary>
    public static CultureInfo GetCulture(SbCalendarSystem calendarSystem)
    {
        return calendarSystem switch
        {
            SbCalendarSystem.Persian => new CultureInfo("fa-IR"),
            SbCalendarSystem.Hijri => new CultureInfo("ar-SA"),
            _ => CultureInfo.CurrentUICulture.Calendar is GregorianCalendar 
                ? CultureInfo.CurrentUICulture 
                : CultureInfo.InvariantCulture
        };
    }
    
    /// <summary>
    /// Gets the first day of the month for the given date in the specified culture.
    /// </summary>
    public static DateTime GetMonthStart(DateTime date, CultureInfo culture)
    {
        var calendar = culture.Calendar;
        var year = calendar.GetYear(date);
        var month = calendar.GetMonth(date);
        return calendar.ToDateTime(year, month, 1, 0, 0, 0, 0);
    }
    
    /// <summary>
    /// Gets the last day of the month for the given date in the specified culture.
    /// </summary>
    public static DateTime GetMonthEnd(DateTime date, CultureInfo culture)
    {
        var calendar = culture.Calendar;
        var year = calendar.GetYear(date);
        var month = calendar.GetMonth(date);
        var daysInMonth = calendar.GetDaysInMonth(year, month);
        return calendar.ToDateTime(year, month, daysInMonth, 23, 59, 59, 0);
    }
    
    /// <summary>
    /// Gets all days in the week containing the specified date.
    /// </summary>
    public static IEnumerable<DateTime> GetWeekDays(DateTime date, DayOfWeek firstDayOfWeek)
    {
        var startOfWeek = date.AddDays(-(int)((date.DayOfWeek - firstDayOfWeek + 7) % 7));
        for (int i = 0; i < 7; i++)
        {
            yield return startOfWeek.AddDays(i);
        }
    }
    
    /// <summary>
    /// Gets all the days to display in a calendar month view (6 weeks).
    /// </summary>
    public static IEnumerable<DateTime> GetMonthViewDays(DateTime month, CultureInfo culture, DayOfWeek firstDayOfWeek)
    {
        var monthStart = GetMonthStart(month, culture);
        var firstDayOffset = ((int)monthStart.DayOfWeek - (int)firstDayOfWeek + 7) % 7;
        var calendarStart = monthStart.AddDays(-firstDayOffset);
        
        // Return 6 weeks (42 days) to fill the calendar grid
        for (int i = 0; i < 42; i++)
        {
            yield return calendarStart.AddDays(i);
        }
    }
    
    /// <summary>
    /// Formats a date according to the specified culture.
    /// </summary>
    public static string FormatDate(DateTime date, string? format, CultureInfo culture)
    {
        return date.ToString(format ?? culture.DateTimeFormat.ShortDatePattern, culture);
    }
    
    /// <summary>
    /// Parses a date string according to the specified culture.
    /// </summary>
    public static bool TryParseDate(string? dateString, CultureInfo culture, out DateTime result)
    {
        if (string.IsNullOrWhiteSpace(dateString))
        {
            result = default;
            return false;
        }
        
        return DateTime.TryParse(dateString, culture, DateTimeStyles.None, out result);
    }
    
    /// <summary>
    /// Gets the abbreviated day names starting from the specified first day of week.
    /// </summary>
    public static string[] GetDayNames(CultureInfo culture, DayOfWeek firstDayOfWeek)
    {
        var names = culture.DateTimeFormat.AbbreviatedDayNames;
        var result = new string[7];
        var start = (int)firstDayOfWeek;
        
        for (int i = 0; i < 7; i++)
        {
            result[i] = names[(start + i) % 7];
        }
        
        return result;
    }
    
    /// <summary>
    /// Gets the month name for the specified date.
    /// </summary>
    public static string GetMonthName(DateTime date, CultureInfo culture)
    {
        var month = culture.Calendar.GetMonth(date);
        return culture.DateTimeFormat.MonthNames[month - 1];
    }
    
    /// <summary>
    /// Gets the year for the specified date in the culture's calendar.
    /// </summary>
    public static int GetYear(DateTime date, CultureInfo culture)
    {
        return culture.Calendar.GetYear(date);
    }
    
    /// <summary>
    /// Gets the month for the specified date in the culture's calendar.
    /// </summary>
    public static int GetMonth(DateTime date, CultureInfo culture)
    {
        return culture.Calendar.GetMonth(date);
    }
    
    /// <summary>
    /// Gets the day of month for the specified date in the culture's calendar.
    /// </summary>
    public static int GetDayOfMonth(DateTime date, CultureInfo culture)
    {
        return culture.Calendar.GetDayOfMonth(date);
    }
    
    /// <summary>
    /// Determines if the text direction should be RTL for the specified calendar system.
    /// </summary>
    public static bool IsRtl(SbCalendarSystem calendarSystem)
    {
        return calendarSystem is SbCalendarSystem.Persian or SbCalendarSystem.Hijri;
    }

    /// <summary>
    /// Returns whether the given date falls on a weekend for the specified calendar system.
    /// Persian/Jalali and Hijri: Friday only. Gregorian: Saturday and Sunday.
    /// </summary>
    public static bool IsWeekend(DateTime date, SbCalendarSystem calendarSystem)
    {
        var dow = date.DayOfWeek;
        return calendarSystem switch
        {
            SbCalendarSystem.Persian => dow == DayOfWeek.Friday,
            SbCalendarSystem.Hijri => dow == DayOfWeek.Friday,
            _ => dow == DayOfWeek.Saturday || dow == DayOfWeek.Sunday
        };
    }
    
    /// <summary>
    /// Converts DateOnly to DateTime.
    /// </summary>
    public static DateTime ToDateTime(DateOnly date)
    {
        return date.ToDateTime(TimeOnly.MinValue);
    }
    
    /// <summary>
    /// Converts DateTime to DateOnly.
    /// </summary>
    public static DateOnly ToDateOnly(DateTime date)
    {
        return DateOnly.FromDateTime(date);
    }
}
