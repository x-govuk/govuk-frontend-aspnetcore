using System.Globalization;
using Microsoft.Playwright;

namespace GovUk.Frontend.AspNetCore.Testing.Playwright;

/// <summary>
/// Locates the parts of a GOV.UK form field: a text input, textarea, character count, select, password input, file
/// upload, date input, radios or checkboxes.
/// </summary>
/// <remarks>
/// <para>
/// A field's <see cref="Label"/> is its <c>&lt;label&gt;</c> or, for date inputs, radios and checkboxes, the
/// <c>&lt;legend&gt;</c> of its fieldset.
/// </para>
/// <para>
/// <see cref="ErrorMessage"/>'s text starts with visually hidden text (<c>Error:</c> by default), so check it with
/// <see cref="GovUkLocatorAssertions.ToHaveErrorMessageAsync"/> rather than <c>ToHaveTextAsync</c>.
/// </para>
/// </remarks>
public class FieldLocator
{
    // Direct children only, so that the hints and errors of fields in a conditional reveal aren't included.
    private const string LabelSelector =
        ":scope > .govuk-label, :scope > .govuk-label-wrapper > .govuk-label, " +
        ":scope > .govuk-fieldset > .govuk-fieldset__legend";

    private const string HintSelector =
        ":scope > .govuk-hint:not(.govuk-character-count__message), :scope > .govuk-fieldset > .govuk-hint";

    private const string ErrorMessageSelector =
        ":scope > .govuk-error-message, :scope > .govuk-fieldset > .govuk-error-message";

    internal FieldLocator(ILocator formGroup)
    {
        Locator = formGroup;
        Label = formGroup.Locator(LabelSelector);
        Hint = formGroup.Locator(HintSelector);
        ErrorMessage = formGroup.Locator(ErrorMessageSelector);
    }

    /// <summary>
    /// Gets the field's form group.
    /// </summary>
    public ILocator Locator { get; }

    /// <summary>
    /// Gets the field's <c>&lt;label&gt;</c> or <c>&lt;legend&gt;</c>.
    /// </summary>
    public ILocator Label { get; }

    /// <summary>
    /// Gets the field's hint.
    /// </summary>
    public ILocator Hint { get; }

    /// <summary>
    /// Gets the field's error message.
    /// </summary>
    /// <remarks>
    /// Its text starts with visually hidden text (<c>Error:</c> by default), so check it with
    /// <see cref="GovUkLocatorAssertions.ToHaveErrorMessageAsync"/> rather than <c>ToHaveTextAsync</c>.
    /// </remarks>
    public ILocator ErrorMessage { get; }
}

/// <summary>
/// Locates the parts of a GOV.UK text input.
/// </summary>
public sealed class TextInputLocator : FieldLocator
{
    internal TextInputLocator(ILocator input)
        : base(input.Locator(Selectors.ClosestFormGroup))
    {
        Input = input;
    }

    /// <summary>
    /// Gets the <c>&lt;input&gt;</c>.
    /// </summary>
    public ILocator Input { get; }
}

/// <summary>
/// Locates the parts of a GOV.UK textarea.
/// </summary>
public class TextareaLocator : FieldLocator
{
    internal TextareaLocator(ILocator textarea)
        : base(textarea.Locator(Selectors.ClosestFormGroup))
    {
        Textarea = textarea;
    }

    /// <summary>
    /// Gets the <c>&lt;textarea&gt;</c>.
    /// </summary>
    public ILocator Textarea { get; }
}

/// <summary>
/// Locates the parts of a GOV.UK character count.
/// </summary>
public sealed class CharacterCountLocator : TextareaLocator
{
    internal CharacterCountLocator(ILocator textarea)
        : base(textarea)
    {
        Status = Locator.Locator(":scope > .govuk-character-count__status");
    }

    /// <summary>
    /// Gets the count message that the component's JavaScript shows below the textarea, e.g.
    /// <c>You have 10 characters remaining</c>.
    /// </summary>
    public ILocator Status { get; }
}

/// <summary>
/// Locates the parts of a GOV.UK select.
/// </summary>
public sealed class SelectLocator : FieldLocator
{
    internal SelectLocator(ILocator select)
        : base(select.Locator(Selectors.ClosestFormGroup))
    {
        Select = select;
    }

    /// <summary>
    /// Gets the <c>&lt;select&gt;</c>.
    /// </summary>
    public ILocator Select { get; }

    /// <summary>
    /// Selects the option whose text is <paramref name="text"/>.
    /// </summary>
    /// <returns>The values of the selected options.</returns>
    public Task<IReadOnlyList<string>> SelectOptionByTextAsync(string text) =>
        Select.SelectOptionAsync(new SelectOptionValue { Label = text });
}

/// <summary>
/// Locates the parts of a GOV.UK password input.
/// </summary>
public sealed class PasswordInputLocator : FieldLocator
{
    internal PasswordInputLocator(ILocator input)
        : base(input.Locator(Selectors.ClosestFormGroup))
    {
        Input = input;
        ShowHideButton = Locator.Locator(".govuk-password-input__toggle");
    }

    /// <summary>
    /// Gets the <c>&lt;input&gt;</c>.
    /// </summary>
    public ILocator Input { get; }

    /// <summary>
    /// Gets the button that shows or hides the password. It's hidden until the component's JavaScript has run.
    /// </summary>
    public ILocator ShowHideButton { get; }
}

/// <summary>
/// Locates the parts of a GOV.UK file upload.
/// </summary>
public sealed class FileUploadLocator : FieldLocator
{
    internal FileUploadLocator(ILocator input)
        : base(input.Locator(Selectors.ClosestFormGroup))
    {
        Input = input;
    }

    /// <summary>
    /// Gets the <c>&lt;input type="file"&gt;</c>, e.g. for <c>SetInputFilesAsync</c>.
    /// </summary>
    public ILocator Input { get; }
}

/// <summary>
/// Locates the parts of a GOV.UK date input.
/// </summary>
/// <remarks>
/// Each part is found by the end of its input's <c>name</c>, e.g. <c>DateOfBirth.Day</c> or <c>dob-day</c>, so
/// localised labels don't matter.
/// </remarks>
public sealed class DateInputLocator : FieldLocator
{
    internal DateInputLocator(ILocator fieldset)
        : base(fieldset.Locator(Selectors.ClosestFormGroup))
    {
        Inputs = fieldset.Locator(":scope > .govuk-date-input .govuk-date-input__input");
        Day = Inputs.And(fieldset.Page.Locator("[name$='day' i]"));
        Month = Inputs.And(fieldset.Page.Locator("[name$='month' i]"));
        Year = Inputs.And(fieldset.Page.Locator("[name$='year' i]"));
    }

    /// <summary>
    /// Gets every input in the date input, in order.
    /// </summary>
    public ILocator Inputs { get; }

    /// <summary>
    /// Gets the day input.
    /// </summary>
    public ILocator Day { get; }

    /// <summary>
    /// Gets the month input.
    /// </summary>
    public ILocator Month { get; }

    /// <summary>
    /// Gets the year input.
    /// </summary>
    public ILocator Year { get; }

    /// <summary>
    /// Fills in the date input with <paramref name="date"/>, leaving out any part the date input doesn't have, e.g.
    /// the day of a month and year input.
    /// </summary>
    /// <remarks>
    /// For a month and year or a day and month input, <see cref="FillMonthAndYearAsync"/> and
    /// <see cref="FillDayAndMonthAsync"/> don't need a made-up day or year.
    /// </remarks>
    public Task FillAsync(DateOnly date) =>
        FillPartsAsync(Format(date.Day), Format(date.Month), Format(date.Year), skipMissingParts: true);

    /// <summary>
    /// Fills in the month and year of a month and year date input, e.g. one bound to a <c>(int, int)</c> with
    /// <c>[DateInput(DateInputItemTypes.MonthAndYear)]</c>.
    /// </summary>
    /// <exception cref="InvalidOperationException">The date input doesn't have a month or a year input.</exception>
    public Task FillMonthAndYearAsync(int month, int year) =>
        FillPartsAsync(day: null, Format(month), Format(year), skipMissingParts: false);

    /// <summary>
    /// Fills in the day and month of a day and month date input, e.g. one bound to a <c>(int, int)</c> with
    /// <c>[DateInput(DateInputItemTypes.DayAndMonth)]</c>.
    /// </summary>
    /// <exception cref="InvalidOperationException">The date input doesn't have a day or a month input.</exception>
    public Task FillDayAndMonthAsync(int day, int month) =>
        FillPartsAsync(Format(day), Format(month), year: null, skipMissingParts: false);

    /// <summary>
    /// Fills in each part of the date input as given, for values that aren't a valid date, e.g. to check validation.
    /// A part that's <see langword="null"/> is left as it is.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// A value is given for a part the date input doesn't have, e.g. a year for a day and month input.
    /// </exception>
    public Task FillAsync(string? day, string? month, string? year) =>
        FillPartsAsync(day, month, year, skipMissingParts: false);

    private async Task FillPartsAsync(string? day, string? month, string? year, bool skipMissingParts)
    {
        // Wait for the date input itself, so that a missing one fails rather than filling nothing.
        await Inputs.First.WaitForAsync().ConfigureAwait(false);

        (ILocator Locator, string Name, string? Value)[] parts = [(Day, "day", day), (Month, "month", month), (Year, "year", year)];
        var toFill = new List<(ILocator Locator, string Value)>();

        // Check every part before filling any, so that a missing part doesn't leave the others half filled in.
        foreach (var (locator, name, value) in parts)
        {
            if (value is null)
            {
                continue;
            }

            if (await locator.CountAsync().ConfigureAwait(false) > 0)
            {
                toFill.Add((locator, value));
            }
            else if (!skipMissingParts)
            {
                throw new InvalidOperationException(
                    $"Can't fill in the {name} of the date input because it doesn't have a {name} input.");
            }
        }

        foreach (var (locator, value) in toFill)
        {
            await locator.FillAsync(value).ConfigureAwait(false);
        }
    }

    private static string Format(int value) => value.ToString(CultureInfo.InvariantCulture);
}

/// <summary>
/// Locates the parts of a set of GOV.UK radios.
/// </summary>
public sealed class RadiosLocator : FieldLocator
{
    internal RadiosLocator(ILocator fieldset)
        : base(fieldset.Locator(Selectors.ClosestFormGroup))
    {
        Items = fieldset.Locator(":scope > .govuk-radios > .govuk-radios__item");
    }

    /// <summary>
    /// Gets every radio item, not including dividers.
    /// </summary>
    public ILocator Items { get; }

    /// <summary>
    /// Gets the radio item labelled <paramref name="label"/>.
    /// </summary>
    public ChoiceItemLocator GetItemByLabel(string label) => ChoiceItemLocator.Create(Items, label, "govuk-radios");

    /// <summary>
    /// Locates the item whose input has the value <paramref name="value"/>, e.g. <c>email</c>.
    /// </summary>
    /// <param name="value">The item's value.</param>
    public ChoiceItemLocator GetItemByValue(string value) => ChoiceItemLocator.CreateByValue(Items, value, "govuk-radios");
}

/// <summary>
/// Locates the parts of a set of GOV.UK checkboxes.
/// </summary>
public sealed class CheckboxesLocator : FieldLocator
{
    internal CheckboxesLocator(ILocator fieldset)
        : base(fieldset.Locator(Selectors.ClosestFormGroup))
    {
        Items = fieldset.Locator(":scope > .govuk-checkboxes > .govuk-checkboxes__item");
    }

    /// <summary>
    /// Gets every checkbox item, not including dividers.
    /// </summary>
    public ILocator Items { get; }

    /// <summary>
    /// Gets the checkbox item labelled <paramref name="label"/>.
    /// </summary>
    public ChoiceItemLocator GetItemByLabel(string label) =>
        ChoiceItemLocator.Create(Items, label, "govuk-checkboxes");

    /// <summary>
    /// Locates the item whose input has the value <paramref name="value"/>, e.g. <c>british</c>.
    /// </summary>
    /// <param name="value">The item's value.</param>
    public ChoiceItemLocator GetItemByValue(string value) =>
        ChoiceItemLocator.CreateByValue(Items, value, "govuk-checkboxes");
}

/// <summary>
/// Locates the parts of a radio or checkbox item.
/// </summary>
public sealed class ChoiceItemLocator
{
    private ChoiceItemLocator(ILocator item, string block)
    {
        Locator = item;
        Input = item.Locator($":scope > .{block}__input");
        Label = item.Locator($":scope > .{block}__label");
        Hint = item.Locator($":scope > .{block}__hint");
        ConditionalContent = item.Locator(
            $"xpath=following-sibling::*[1][contains(concat(' ', normalize-space(@class), ' '), ' {block}__conditional ')]");
    }

    /// <summary>
    /// Gets the item's element.
    /// </summary>
    public ILocator Locator { get; }

    /// <summary>
    /// Gets the item's <c>&lt;input&gt;</c>, e.g. for <c>CheckAsync</c> or <c>ToBeCheckedAsync</c>.
    /// </summary>
    public ILocator Input { get; }

    /// <summary>
    /// Gets the item's label.
    /// </summary>
    public ILocator Label { get; }

    /// <summary>
    /// Gets the item's hint.
    /// </summary>
    public ILocator Hint { get; }

    /// <summary>
    /// Gets the content that's revealed when the item is checked. Scope field lookups to its locator to find the
    /// fields inside it.
    /// </summary>
    public ILocator ConditionalContent { get; }

    internal static ChoiceItemLocator Create(ILocator items, string label, string block) =>
        new(items.Filter(new() { Has = items.Page.GetByLabel(label, new() { Exact = true }) }), block);

    internal static ChoiceItemLocator CreateByValue(ILocator items, string value, string block) =>
        new(items.Filter(new()
        {
            Has = items.Page.Locator($":scope > .{block}__input[value={Selectors.AttributeValue(value)}]")
        }), block);
}
