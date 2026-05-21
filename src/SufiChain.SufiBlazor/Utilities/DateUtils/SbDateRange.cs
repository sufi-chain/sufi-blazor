namespace SufiChain.SufiBlazor.Utilities.DateUtils;

/// <summary>
/// Represents a date range with start and end dates.
/// </summary>
public record SbDateRange
{
    /// <summary>
    /// The start date of the range.
    /// </summary>
    public DateOnly? Start { get; init; }
    
    /// <summary>
    /// The end date of the range.
    /// </summary>
    public DateOnly? End { get; init; }
    
    /// <summary>
    /// Creates an empty date range.
    /// </summary>
    public SbDateRange() { }
    
    /// <summary>
    /// Creates a date range with specified start and end dates.
    /// </summary>
    public SbDateRange(DateOnly? start, DateOnly? end)
    {
        Start = start;
        End = end;
    }
    
    /// <summary>
    /// Returns true if both start and end dates are set.
    /// </summary>
    public bool IsComplete => Start.HasValue && End.HasValue;
    
    /// <summary>
    /// Returns true if the range is empty (no dates set).
    /// </summary>
    public bool IsEmpty => !Start.HasValue && !End.HasValue;
    
    /// <summary>
    /// Returns true if the specified date falls within this range.
    /// </summary>
    public bool Contains(DateOnly date)
    {
        if (!IsComplete) return false;
        return date >= Start!.Value && date <= End!.Value;
    }
    
    /// <summary>
    /// Gets the number of days in the range.
    /// </summary>
    public int? DayCount => IsComplete 
        ? End!.Value.DayNumber - Start!.Value.DayNumber + 1 
        : null;
}
