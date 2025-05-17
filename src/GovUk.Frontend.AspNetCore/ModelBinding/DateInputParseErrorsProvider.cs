using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GovUk.Frontend.AspNetCore.ModelBinding;

/// <summary>
/// Stores the <see cref="DateInputParseErrors"/> for a given <see cref="ModelStateEntry"/>.
/// </summary>
public class DateInputParseErrorsProvider
{
    private readonly Dictionary<ModelStateEntry, DateInputParseErrors> _errors;

    /// <summary>
    /// Initializes a new instance of the <see cref="DateInputParseErrorsProvider"/> class.
    /// </summary>
    public DateInputParseErrorsProvider()
    {
        _errors = new Dictionary<ModelStateEntry, DateInputParseErrors>();
    }

    /// <summary>
    /// Sets the date input parse errors for a model name.
    /// </summary>
    /// <param name="modelStateEntry">The <see cref="ModelStateEntry"/>.</param>
    /// <param name="parseErrors">The <see cref="DateInputParseErrors"/>.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="modelStateEntry"/> argument is null.</exception>
    public void SetErrorsForModel(ModelStateEntry modelStateEntry, DateInputParseErrors parseErrors)
    {
        ArgumentNullException.ThrowIfNull(modelStateEntry);

        _errors[modelStateEntry] = parseErrors;
    }

    /// <summary>
    /// Gets the <see cref="DateInputParseErrors"/> for a <see cref="ModelStateEntry"/>.
    /// </summary>
    /// <param name="modelStateEntry">The <see cref="ModelStateEntry"/>.</param>
    /// <param name="errors">The <see cref="DateInputParseErrors"/>.</param>
    /// <returns><see langword="true"/> if errors have been set for the specified model name; otherwise <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="modelStateEntry"/> argument is null.</exception>
    public bool TryGetErrorsForModel(ModelStateEntry modelStateEntry, out DateInputParseErrors errors)
    {
        ArgumentNullException.ThrowIfNull(modelStateEntry);

        return _errors.TryGetValue(modelStateEntry, out errors);
    }
}
