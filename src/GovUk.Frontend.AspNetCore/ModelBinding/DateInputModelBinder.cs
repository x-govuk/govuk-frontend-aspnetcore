using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GovUk.Frontend.AspNetCore.ModelBinding;

/// <summary>
/// An <see cref="IModelBinder"/> for binding Date input components.
/// </summary>
public class DateInputModelBinder : IModelBinder
{
    internal const string DayInputName = "Day";
    internal const string MonthInputName = "Month";
    internal const string YearInputName = "Year";

    internal static DateInputItemTypes[] SupportedItemTypes { get; } =
    [
        DateInputItemTypes.DayMonthAndYear,
        DateInputItemTypes.DayAndMonth,
        DateInputItemTypes.MonthAndYear
    ];

    private readonly DateInputModelConverter _modelConverter;

    /// <summary>
    /// Initializes a new instance of the <see cref="DateInputModelBinder"/> class.
    /// </summary>
    /// <param name="modelConverter">The <see cref="DateInputModelConverter"/>.</param>
    public DateInputModelBinder(DateInputModelConverter modelConverter)
    {
        ArgumentNullException.ThrowIfNull(modelConverter);

        _modelConverter = modelConverter;
    }

    /// <inheritdoc/>
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var optionsAccessor = bindingContext.HttpContext.RequestServices.GetRequiredService<IOptions<GovUkFrontendOptions>>();

        var modelType = bindingContext.ModelMetadata.UnderlyingOrModelType;

        var dayModelName = ModelNames.CreatePropertyModelName(bindingContext.ModelName, DayInputName);
        var monthModelName = ModelNames.CreatePropertyModelName(bindingContext.ModelName, MonthInputName);
        var yearModelName = ModelNames.CreatePropertyModelName(bindingContext.ModelName, YearInputName);

        var dayValueProviderResult = bindingContext.ValueProvider.GetValue(dayModelName);
        var monthValueProviderResult = bindingContext.ValueProvider.GetValue(monthModelName);
        var yearValueProviderResult = bindingContext.ValueProvider.GetValue(yearModelName);

        if ((dayValueProviderResult == ValueProviderResult.None || dayValueProviderResult.FirstValue == string.Empty) &&
            (monthValueProviderResult == ValueProviderResult.None || monthValueProviderResult.FirstValue == string.Empty) &&
            (yearValueProviderResult == ValueProviderResult.None || yearValueProviderResult.FirstValue == string.Empty))
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelMetadata.TryGetDateInputModelMetadata(out var dateInputModelMetadata);
        var itemTypes = dateInputModelMetadata?.ItemTypes ?? _modelConverter.DefaultItemTypes ?? DateInputItemTypes.DayMonthAndYear;

        if (!SupportedItemTypes.Contains(itemTypes))
        {
            // Weird combination of fields submitted; we're done
            return Task.CompletedTask;
        }

        var parseErrors = Parse(
            itemTypes,
            dayValueProviderResult.FirstValue,
            monthValueProviderResult.FirstValue,
            yearValueProviderResult.FirstValue,
            optionsAccessor.Value.AcceptMonthNamesInDateInputs,
            out var dateParts);

        if (parseErrors == DateInputParseErrors.None)
        {
            var createModelContext = new DateInputConvertToModelContext(modelType, itemTypes, dateParts);
            var model = _modelConverter.ConvertToModel(createModelContext);
            bindingContext.Result = ModelBindingResult.Success(model);
        }
        else
        {
            var overallAttemptedValueParts = new List<ValueProviderResult>();

            if (itemTypes.HasFlag(DateInputItemTypes.Day))
            {
                bindingContext.ModelState.SetModelValue(dayModelName, dayValueProviderResult);
                overallAttemptedValueParts.Add(dayValueProviderResult);
            }

            if (itemTypes.HasFlag(DateInputItemTypes.Month))
            {
                bindingContext.ModelState.SetModelValue(monthModelName, monthValueProviderResult);
                overallAttemptedValueParts.Add(monthValueProviderResult);
            }

            if (itemTypes.HasFlag(DateInputItemTypes.Year))
            {
                bindingContext.ModelState.SetModelValue(yearModelName, yearValueProviderResult);
                overallAttemptedValueParts.Add(yearValueProviderResult);
            }

            var overallAttemptedValue = string.Join(",", overallAttemptedValueParts.Select(vpr => vpr.FirstValue ?? ""));

            var errorMessage = GetModelStateErrorMessage(parseErrors, bindingContext.ModelMetadata);
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, rawValue: null, attemptedValue: overallAttemptedValue);
            var modelError = new ModelError(new DateInputParseException(errorMessage, parseErrors), errorMessage);
            bindingContext.ModelState[bindingContext.ModelName]!.Errors.Add(modelError);

            bindingContext.Result = ModelBindingResult.Failed();
        }

        return Task.CompletedTask;
    }

    // internal for testing
    internal static string GetModelStateErrorMessage(DateInputParseErrors parseErrors, ModelMetadata modelMetadata)
    {
        // TODO Make these messages configurable
        // (see Microsoft.AspNetCore.Mvc.ModelBinding.Metadata.ModelBindingMessageProvider)

        Debug.Assert(parseErrors != DateInputParseErrors.None);
        Debug.Assert(parseErrors != (DateInputParseErrors.MissingDay | DateInputParseErrors.MissingMonth | DateInputParseErrors.MissingYear));

        modelMetadata.TryGetDateInputModelMetadata(out var dateInputModelMetadata);

        var displayName = dateInputModelMetadata?.ErrorMessagePrefix ?? modelMetadata.DisplayName ?? modelMetadata.PropertyName;

        var missingFields = new List<string>();

        if ((parseErrors & DateInputParseErrors.MissingDay) != 0)
        {
            missingFields.Add("day");
        }
        if ((parseErrors & DateInputParseErrors.MissingMonth) != 0)
        {
            missingFields.Add("month");
        }
        if ((parseErrors & DateInputParseErrors.MissingYear) != 0)
        {
            missingFields.Add("year");
        }

        if (missingFields.Count > 0)
        {
            Debug.Assert(missingFields.Count <= 2);
            return $"{displayName} must include a {string.Join(" and ", missingFields)}";
        }

        return $"{displayName} must be a real date";
    }

    // internal for testing
    internal static DateInputParseErrors Parse(
        DateInputItemTypes itemTypes,
        string? day,
        string? month,
        string? year,
        bool acceptMonthNames,
        out DateInputItemValues dateParts)
    {
        day ??= string.Empty;
        month ??= string.Empty;
        year ??= string.Empty;

        var errors = DateInputParseErrors.None;
        int parsedYear = 0, parsedMonth = 0, parsedDay = 0;
        int? maxDaysInMonth = null;

        var expectYear = (itemTypes & DateInputItemTypes.Year) != 0;
        Debug.Assert((itemTypes & DateInputItemTypes.Month) != 0);
        var expectMonth = true;
        var expectDay = (itemTypes & DateInputItemTypes.Day) != 0;

        if (expectYear)
        {
            if (string.IsNullOrEmpty(year))
            {
                errors |= DateInputParseErrors.MissingYear;
            }
            else if (!TryParseYear(year, out parsedYear) || parsedYear < 1 || parsedYear > 9999 || year.Length != 4)
            {
                errors |= DateInputParseErrors.InvalidYear;
            }
        }

        var yearIsValid = (errors & (DateInputParseErrors.InvalidYear | DateInputParseErrors.MissingYear)) == 0;

        if (expectMonth)
        {
            if (string.IsNullOrEmpty(month))
            {
                errors |= DateInputParseErrors.MissingMonth;
            }
            else if (!TryParseMonth(month, out parsedMonth) || parsedMonth < 1 || parsedMonth > 12)
            {
                errors |= DateInputParseErrors.InvalidMonth;
            }
        }

        var monthIsValid = (errors & (DateInputParseErrors.InvalidMonth | DateInputParseErrors.MissingMonth)) == 0;

        if (expectDay)
        {
            // If we know the year and month we can figure out the days in the month.
            // If we only have the month and not a year, we should assume the year could be a leap year.
            if (monthIsValid)
            {
                var y = expectYear && yearIsValid ? parsedYear : 2000;  // 2000 is a leap year
                maxDaysInMonth = DateTime.DaysInMonth(y, parsedMonth);
            }

            if (string.IsNullOrEmpty(day))
            {
                errors |= DateInputParseErrors.MissingDay;
            }
            else if (!TryParseDay(day, out parsedDay) || parsedDay < 1 || parsedDay > 31 || parsedDay > maxDaysInMonth)
            {
                errors |= DateInputParseErrors.InvalidDay;
            }
        }

        dateParts = errors == DateInputParseErrors.None ?
            new DateInputItemValues(
                expectDay ? parsedDay : null,
                expectMonth ? parsedMonth : null,
                expectYear ? parsedYear : null) :
            default;

        return errors;

        bool TryParseDay(string value, out int result) => int.TryParse(value, out result);

        bool TryParseMonth(string value, out int result)
        {
            if (string.IsNullOrEmpty(value))
            {
                result = 0;
                return false;
            }

            if (!int.TryParse(value, out result) && acceptMonthNames)
            {
                result = value.ToLowerInvariant() switch
                {
                    "jan" => 1,
                    "january" => 1,
                    "feb" => 2,
                    "february" => 2,
                    "mar" => 3,
                    "march" => 3,
                    "apr" => 4,
                    "april" => 4,
                    "may" => 5,
                    "jun" => 6,
                    "june" => 6,
                    "jul" => 7,
                    "july" => 7,
                    "aug" => 8,
                    "august" => 8,
                    "sep" => 9,
                    "september" => 9,
                    "oct" => 10,
                    "october" => 10,
                    "nov" => 11,
                    "november" => 11,
                    "dec" => 12,
                    "december" => 12,
                    _ => 0
                };
            }

            return result is not 0;
        }

        bool TryParseYear(string value, out int result) => int.TryParse(value, out result);
    }
}
