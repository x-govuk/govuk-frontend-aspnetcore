namespace GovUk.Frontend.AspNetCore.ModelBinding;

/// <summary>
/// Represents the day, month and year of a date.
/// </summary>
/// <param name="Day">The day.</param>
/// <param name="Month">The month.</param>
/// <param name="Year">The year.</param>
public record struct DateInputItemValues(int? Day, int? Month, int? Year);
