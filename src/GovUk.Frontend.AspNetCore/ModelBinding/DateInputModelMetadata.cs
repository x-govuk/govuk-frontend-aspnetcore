namespace GovUk.Frontend.AspNetCore.ModelBinding;

/// <summary>
/// Contains the date input metadata for a model property or parameter.
/// </summary>
/// <remarks>
/// An instance is added to <see cref="Microsoft.AspNetCore.Mvc.ModelBinding.ModelMetadata.AdditionalValues"/>,
/// keyed on <c>typeof(DateInputModelMetadata)</c>, for every property and parameter with a <see cref="DateInputAttribute"/>.
/// </remarks>
public class DateInputModelMetadata
{
    /// <summary>
    /// Gets or sets the prefix used in error messages.
    /// </summary>
    /// <remarks>
    /// This prefix is used at the start of error messages produced by <see cref="DateInputModelBinder"/>
    /// e.g. <c>{ErrorMessagePrefix} must be a real date</c>
    /// </remarks>
    public string? ErrorMessagePrefix { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="DateInputItemTypes"/> that should be created on the date input for this property.
    /// </summary>
    public DateInputItemTypes? ItemTypes { get; set; }
}
