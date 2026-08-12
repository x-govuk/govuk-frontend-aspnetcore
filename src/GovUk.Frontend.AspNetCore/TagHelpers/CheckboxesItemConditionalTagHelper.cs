using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the conditional reveal of a checkbox item in a GDS checkboxes component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CheckboxesItemTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the conditional reveal for the item.")]
public class CheckboxesItemConditionalTagHelper : FormGroupItemConditionalTagHelperBase
{
    internal const string TagName = "govuk-checkboxes-item-conditional";
}
