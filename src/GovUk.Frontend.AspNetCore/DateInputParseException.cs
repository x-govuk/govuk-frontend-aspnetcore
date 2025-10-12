using System.Globalization;
using GovUk.Frontend.AspNetCore.ModelBinding;

namespace GovUk.Frontend.AspNetCore;

/// <summary>
/// Represents the errors that occured when parsing a date input's values.
/// </summary>
public class DateInputParseException : Exception
{
    private readonly string _messageTemplate;

    internal DateInputParseException(string messageTemplate, string displayName, DateInputParseErrors parseErrors)
        : base(GetMessage(messageTemplate, displayName))
    {
        ArgumentNullException.ThrowIfNull(messageTemplate);
        ArgumentNullException.ThrowIfNull(displayName);

        if (parseErrors is DateInputParseErrors.None)
        {
            throw new ArgumentException(
                $"Cannot create a {nameof(DateInputParseException)} with no {nameof(DateInputParseErrors)}.", nameof(parseErrors));
        }

        ParseErrors = parseErrors;
        _messageTemplate = messageTemplate;
    }

    /// <summary>
    /// Gets the <see cref="DateInputParseErrors"/>.
    /// </summary>
    public DateInputParseErrors ParseErrors { get; }

    internal string GetMessage(string errorMessagePrefix) => GetMessage(_messageTemplate, errorMessagePrefix);

    private static string GetMessage(string messageTemplate, string errorMessagePrefix) =>
        string.Format(CultureInfo.InvariantCulture, messageTemplate, errorMessagePrefix);
}
