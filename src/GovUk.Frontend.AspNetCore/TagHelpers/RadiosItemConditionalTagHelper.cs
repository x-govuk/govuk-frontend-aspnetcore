using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the conditional reveal of a radios item in a GDS radios component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = RadiosItemTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the conditional reveal for the item.")]
public class RadiosItemConditionalTagHelper : FormGroupItemConditionalTagHelperBase
{
    internal const string TagName = "govuk-radios-item-conditional";
}
