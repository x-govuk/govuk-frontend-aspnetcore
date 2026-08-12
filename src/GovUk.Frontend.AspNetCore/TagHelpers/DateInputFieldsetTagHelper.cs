using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the fieldset in a GDS date input component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = DateInputTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = DateInputTagHelper.TagName)]
#endif
[RestrictChildren(
    FormGroupFieldsetLegendTagHelper.DateInputTagName,
    DateInputHintTagHelper.TagName,
    DateInputErrorMessageTagHelper.TagName,
    DateInputDayTagHelper.TagName,
    DateInputMonthTagHelper.TagName,
    DateInputYearTagHelper.TagName,
    DateInputBeforeInputsTagHelper.TagName,
    DateInputAfterInputsTagHelper.TagName
#if SHORT_TAG_NAMES
    ,
    FormGroupHintTagHelperBase.ShortTagName,
    FormGroupErrorMessageTagHelperBase.ShortTagName
#endif
    )]
[TagHelperDocumentation(ContentDescription = "A container element used when the date input should be contained within a fieldset element. When used, every other child element must be placed inside this element rather than the root date input element.")]
public class DateInputFieldsetTagHelper : FormGroupFieldsetTagHelperBase
{
    internal const string TagName = "govuk-date-input-fieldset";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [
        TagName
#if SHORT_TAG_NAMES
        ,
        ShortTagName
#endif
    ];

    /// <summary>
    /// Creates a <see cref="DateInputFieldsetTagHelper"/>.
    /// </summary>
    public DateInputFieldsetTagHelper()
    {
    }

    private protected override string LegendTagName => FormGroupFieldsetLegendTagHelper.DateInputTagName;
}
