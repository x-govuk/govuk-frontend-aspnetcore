using GovUk.Frontend.AspNetCore.ModelBinding;

namespace GovUk.Frontend.AspNetCore;

/// <summary>
/// Represents the errors that occured when parsing a date input's values.
/// </summary>
public class DateInputParseException : Exception
{
    /// <summary>
    /// Creates a new <see cref="DateInputParseException"/>.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="parseErrors">The <see cref="DateInputParseErrors"/>.</param>
    public DateInputParseException(string message, DateInputParseErrors parseErrors)
        : base(message)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (parseErrors is DateInputParseErrors.None)
        {
            throw new ArgumentException(
                $"Cannot create a {nameof(DateInputParseException)} with no {nameof(DateInputParseErrors)}.", nameof(parseErrors));
        }

        ParseErrors = parseErrors;
    }

    /// <summary>
    /// Gets the <see cref="DateInputParseErrors"/>.
    /// </summary>
    public DateInputParseErrors ParseErrors { get; }
}
