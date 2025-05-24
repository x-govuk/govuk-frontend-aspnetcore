using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GovUk.Frontend.AspNetCore.ModelBinding;

/// <summary>
/// Stores additional information from model binding.
/// </summary>
public class BindingResultInfoProvider
{
    private readonly Dictionary<string, Entry> _entries;

    /// <summary>
    /// Initializes a new instance of the <see cref="BindingResultInfoProvider"/> class.
    /// </summary>
    public BindingResultInfoProvider()
    {
        _entries = new();
    }

    /// <summary>
    /// Sets the date input parse errors for a model.
    /// </summary>
    /// <param name="modelName">The model name.</param>
    /// <param name="parseErrors">The <see cref="DateInputParseErrors"/>.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="modelName"/> argument is null.</exception>
    public void SetDateInputParseErrors(string modelName, DateInputParseErrors parseErrors)
    {
        ArgumentNullException.ThrowIfNull(modelName);

        _entries.TryAdd(modelName, new Entry());
        _entries[modelName].ParseErrors = parseErrors;
    }

    /// <summary>
    /// Gets the <see cref="DateInputParseErrors"/> for a <see cref="ModelStateEntry"/>.
    /// </summary>
    /// <param name="modelName">The <see cref="ModelStateEntry"/>.</param>
    /// <param name="errors">The <see cref="DateInputParseErrors"/>.</param>
    /// <returns><see langword="true"/> if errors have been set for the specified model name; otherwise <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="modelName"/> argument is null.</exception>
    public bool TryGetDateInputParseErrorsForModel(string modelName, out DateInputParseErrors errors)
    {
        ArgumentNullException.ThrowIfNull(modelName);

        if (_entries.TryGetValue(modelName, out var entry) && entry.ParseErrors is not null)
        {
            errors = entry.ParseErrors.Value;
            return true;
        }

        errors = default;
        return false;
    }

    private record Entry
    {
        public DateInputParseErrors? ParseErrors { get; set; }
    }
}
