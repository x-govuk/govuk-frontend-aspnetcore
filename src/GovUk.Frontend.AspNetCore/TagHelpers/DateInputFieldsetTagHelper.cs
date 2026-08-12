using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the fieldset in a GDS date input component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = DateInputTagHelper.TagName)]
[RestrictChildren(
    FormGroupFieldsetLegendTagHelper.DateInputTagName,
    DateInputHintTagHelper.TagName,
    DateInputErrorMessageTagHelper.TagName,
    DateInputDayTagHelper.TagName,
    DateInputMonthTagHelper.TagName,
    DateInputYearTagHelper.TagName,
    DateInputBeforeInputsTagHelper.TagName,
    DateInputAfterInputsTagHelper.TagName
    )]
[TagHelperDocumentation(ContentDescription = "A container element used when the date input should be contained within a fieldset element. When used, every other child element must be placed inside this element rather than the root date input element, and each must use its govuk- prefixed name; the short names are only available directly inside the root date input element.")]
public class DateInputFieldsetTagHelper : FormGroupFieldsetTagHelperBase
{
    internal const string TagName = "govuk-date-input-fieldset";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName];

    /// <summary>
    /// Creates a <see cref="DateInputFieldsetTagHelper"/>.
    /// </summary>
    public DateInputFieldsetTagHelper()
    {
    }

    private protected override string LegendTagName => FormGroupFieldsetLegendTagHelper.DateInputTagName;
}
