using System.Globalization;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace GovUk.Frontend.AspNetCore.Testing;

/// <summary>
/// A date input component.
/// </summary>
public sealed class DateInput : FormField
{
    internal const string Selector = ".govuk-date-input";

    internal DateInput(IElement dateInput, IElement formGroup, IElement? fieldset, IElement container)
        : base(formGroup, GetLegend(fieldset), container)
    {
        DateInputElement = dateInput;
        Items = Lookup.Children(dateInput, "govuk-date-input__item", e => e.QuerySelector("input"))
            .OfType<IHtmlInputElement>()
            .Select(i => new DateInputItem(i, GetLabel(dateInput, i)))
            .ToArray();
    }

    /// <summary>
    /// Gets the <c>govuk-date-input</c> element that contains the inputs.
    /// </summary>
    public IElement DateInputElement { get; }

    /// <summary>
    /// Gets the inputs, in order.
    /// </summary>
    public IReadOnlyList<DateInputItem> Items { get; }

    /// <summary>
    /// Gets the day input, or <see langword="null"/> if there isn't one.
    /// </summary>
    public DateInputItem? Day => Items.FirstOrDefault(i => i.Part == DateInputPart.Day);

    /// <summary>
    /// Gets the month input, or <see langword="null"/> if there isn't one.
    /// </summary>
    public DateInputItem? Month => Items.FirstOrDefault(i => i.Part == DateInputPart.Month);

    /// <summary>
    /// Gets the year input, or <see langword="null"/> if there isn't one.
    /// </summary>
    public DateInputItem? Year => Items.FirstOrDefault(i => i.Part == DateInputPart.Year);

    /// <summary>
    /// Gets the date entered, or <see langword="null"/> if there isn't a day, month and year input or their values
    /// aren't a real date.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The month can be a number or, as model binding accepts by default, a full or abbreviated English month name,
    /// e.g. <c>3</c>, <c>Mar</c> or <c>March</c>.
    /// </para>
    /// <para>
    /// Use <see cref="DateInputItem.Value"/> on <see cref="Day"/>, <see cref="Month"/> and <see cref="Year"/> to read
    /// values that aren't a date.
    /// </para>
    /// </remarks>
    public DateOnly? Value
    {
        get
        {
            if (TryParse(Day, out var day) && TryParseMonth(Month, out var month) && TryParse(Year, out var year) &&
                year is >= 1 and <= 9999 && month is >= 1 and <= 12 && day >= 1 && day <= DateTime.DaysInMonth(year, month))
            {
                return new DateOnly(year, month, day);
            }

            return null;

            static bool TryParse(DateInputItem? item, out int value)
            {
                value = 0;
                return item is not null &&
                    int.TryParse(item.Value.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out value);
            }

            static bool TryParseMonth(DateInputItem? item, out int value)
            {
                if (TryParse(item, out value))
                {
                    return true;
                }

                if (item is null)
                {
                    return false;
                }

                var name = item.Value.Trim();
                value = Array.FindIndex(MonthNames, names => names.Contains(name, StringComparer.OrdinalIgnoreCase)) + 1;
                return value != 0;
            }
        }
    }

    // The month names DateInputModelBinder accepts when GovUkFrontendOptions.AcceptMonthNamesInDateInputs is true (the default).
    private static readonly string[][] MonthNames =
    [
        ["jan", "january"],
        ["feb", "february"],
        ["mar", "march"],
        ["apr", "april"],
        ["may"],
        ["jun", "june"],
        ["jul", "july"],
        ["aug", "august"],
        ["sep", "september"],
        ["oct", "october"],
        ["nov", "november"],
        ["dec", "december"]
    ];

    internal override string Kind => "date input";
}

/// <summary>
/// Identifies an input in a <see cref="DateInput"/>.
/// </summary>
public enum DateInputPart
{
    /// <summary>
    /// The input isn't recognized as a day, month or year.
    /// </summary>
    Unknown,

    /// <summary>
    /// The day.
    /// </summary>
    Day,

    /// <summary>
    /// The month.
    /// </summary>
    Month,

    /// <summary>
    /// The year.
    /// </summary>
    Year
}

/// <summary>
/// One of the inputs in a <see cref="DateInput"/>.
/// </summary>
public sealed class DateInputItem
{
    internal DateInputItem(IHtmlInputElement input, IElement? label)
    {
        Input = input;
        Label = TextExtraction.GetText(label);
        Part = GetPart(Name) ?? GetPart(input.Id) ?? GetPart(Label) ?? DateInputPart.Unknown;
    }

    /// <summary>
    /// Gets the <c>&lt;input&gt;</c> element.
    /// </summary>
    public IHtmlInputElement Input { get; }

    /// <summary>
    /// Gets the input's label, e.g. <c>Day</c>.
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// Gets the input's <c>name</c>, e.g. <c>DateOfBirth.Day</c>.
    /// </summary>
    public string? Name => Input.GetAttribute("name");

    /// <summary>
    /// Gets which part of the date the input is for.
    /// </summary>
    /// <remarks>
    /// This comes from the last part of the input's <c>name</c> or <c>id</c> (after a <c>.</c> or <c>-</c>), e.g.
    /// <c>DateOfBirth.Day</c> or <c>dob-day</c>, or failing that from its label.
    /// </remarks>
    public DateInputPart Part { get; }

    /// <summary>
    /// Gets the input's current value.
    /// </summary>
    public string Value => Input.Value;

    /// <summary>
    /// Gets whether the input is styled as having an error.
    /// </summary>
    public bool HasError => Input.ClassList.Contains("govuk-input--error");

    /// <inheritdoc/>
    public override string ToString() => $"{Label}: {Value}";

    private static DateInputPart? GetPart(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        var lastPart = value[(value.LastIndexOfAny(['.', '-']) + 1)..];

        return lastPart.ToUpperInvariant() switch
        {
            "DAY" => DateInputPart.Day,
            "MONTH" => DateInputPart.Month,
            "YEAR" => DateInputPart.Year,
            _ => null
        };
    }
}
