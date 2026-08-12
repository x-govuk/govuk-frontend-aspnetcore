using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the fieldset in a GDS checkboxes component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CheckboxesTagHelper.TagName)]
[RestrictChildren(
    FormGroupFieldsetLegendTagHelper.CheckboxesTagName,
    CheckboxesItemTagHelper.TagName,
    CheckboxesItemDividerTagHelper.TagName,
    CheckboxesHintTagHelper.TagName,
    CheckboxesErrorMessageTagHelper.TagName,
    CheckboxesBeforeInputsTagHelper.TagName,
    CheckboxesAfterInputsTagHelper.TagName
)]
[TagHelperDocumentation(ContentDescription = "A container element used when the checkboxes should be contained within a fieldset element. When used, every hint, error message, item and divider must be placed inside this element rather than the root checkboxes element, and each must use its govuk- prefixed name; the short names are only available directly inside the root checkboxes element.")]
public class CheckboxesFieldsetTagHelper : FormGroupFieldsetTagHelperBase
{
    internal const string TagName = "govuk-checkboxes-fieldset";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName];

    /// <summary>
    /// Creates a <see cref="CheckboxesFieldsetTagHelper"/>.
    /// </summary>
    public CheckboxesFieldsetTagHelper()
    {
    }

    private protected override string LegendTagName => FormGroupFieldsetLegendTagHelper.CheckboxesTagName;
}
