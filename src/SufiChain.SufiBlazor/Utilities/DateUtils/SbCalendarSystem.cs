namespace SufiChain.SufiBlazor.Utilities.DateUtils;

/// <summary>
/// Specifies the calendar system to use for date display and selection.
/// </summary>
public enum SbCalendarSystem
{
    /// <summary>
    /// Gregorian (Western) calendar - the default calendar for most cultures.
    /// </summary>
    Gregorian,
    
    /// <summary>
    /// Hijri (Islamic) calendar - used in Persian, Arabic, and other cultures.
    /// </summary>
    Hijri,
    
    /// <summary>
    /// Persian (Solar Hijri) calendar - also known as Jalali calendar.
    /// </summary>
    Persian
}
