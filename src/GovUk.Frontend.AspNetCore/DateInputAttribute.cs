using GovUk.Frontend.AspNetCore.ModelBinding;

namespace GovUk.Frontend.AspNetCore;

/// <summary>
/// An attribute that can specify the error message prefix to use in model binding from date input components.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class DateInputAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="DateInputAttribute"/>.
    /// </summary>
    public DateInputAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="DateInputAttribute"/> with <see cref="DateInputItemTypes"/>.
    /// </summary>
    /// <param name="itemTypes">The <see cref="DateInputItemTypes"/>.</param>
    public DateInputAttribute(DateInputItemTypes itemTypes)
    {
        ItemTypes = itemTypes;
    }

    /// <summary>
    /// Gets or sets the prefix used in error messages.
    /// </summary>
    /// <remarks>
    /// This prefix is used at the start of error messages produced by <see cref="DateInputModelBinder"/>
    /// e.g. <c>{ErrorMessagePrefix} must be a real date</c>
    /// </remarks>
    public string? ErrorMessagePrefix { get; set; }

    /// <summary>
    /// Gets the <see cref="DateInputItemTypes"/> that should be created on the date input for this property.
    /// </summary>
    public DateInputItemTypes? ItemTypes { get; }
}
