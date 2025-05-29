namespace GovUk.Frontend.AspNetCore;

/// <summary>
/// Represents the item types of a date input component.
/// </summary>
[Flags]
public enum DateInputItemTypes
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    None = 0,
    Day = 1,
    Month = 2,
    Year = 4,
    DayAndMonth = Day | Month,
    MonthAndYear = Month | Year,
    DayMonthAndYear = Day | Month | Year,
    All = DayMonthAndYear
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
