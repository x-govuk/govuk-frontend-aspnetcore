using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the fieldset in a GDS checkboxes component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CheckboxesTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = CheckboxesTagHelper.TagName)]
[RestrictChildren(
    CheckboxesFieldsetLegendTagHelper.TagName,
    CheckboxesFieldsetLegendTagHelper.ShortTagName,
    CheckboxesItemTagHelper.TagName,
    CheckboxesItemDividerTagHelper.TagName,
    CheckboxesHintTagHelper.TagName,
    CheckboxesErrorMessageTagHelper.TagName,
    CheckboxesBeforeInputsTagHelper.TagName,
    CheckboxesAfterInputsTagHelper.TagName
#if SHORT_TAG_NAMES
    ,
    FormGroupHintTagHelperBase.ShortTagName,
    FormGroupErrorMessageTagHelperBase.ShortTagName
#endif
)]
[TagHelperDocumentation(ContentDescription = "A container element used when the checkboxes should be contained within a fieldset element. When used, every hint, error message, item and divider must be placed inside this element rather than the root checkboxes element.")]
public class CheckboxesFieldsetTagHelper : FormGroupFieldsetTagHelperBase
{
    internal const string TagName = "govuk-checkboxes-fieldset";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [
        TagName,
        ShortTagName
    ];

    /// <summary>
    /// Creates a <see cref="CheckboxesFieldsetTagHelper"/>.
    /// </summary>
    public CheckboxesFieldsetTagHelper()
    {
    }

    private protected override string LegendTagName => CheckboxesFieldsetLegendTagHelper.TagName;
}
