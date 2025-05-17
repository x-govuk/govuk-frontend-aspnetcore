namespace GovUk.Frontend.AspNetCore;

/// <summary>
/// Represents the fields of a date input component.
/// </summary>
[Flags]
public enum DateInputItems
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    None = 0,
    Day = 1,
    Month = 2,
    Year = 4,
    All = Day | Month | Year
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
