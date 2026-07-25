using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the fieldset in a GDS radios component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = RadiosTagHelper.TagName)]
[RestrictChildren(
    RadiosFieldsetLegendTagHelper.TagName,
    RadiosItemTagHelper.TagName,
    RadiosItemDividerTagHelper.TagName,
    RadiosTagHelper.HintTagName,
    RadiosTagHelper.ErrorMessageTagName,
    RadiosBeforeInputsTagHelper.TagName,
    RadiosAfterInputsTagHelper.TagName
)]
[TagHelperDocumentation(ContentDescription = "A container element used when the radios should be contained within a fieldset element. When used, every hint, error message, item and divider must be placed inside this element rather than the root radios element.")]
public class RadiosFieldsetTagHelper : FormGroupFieldsetTagHelperBase
{
    internal const string TagName = "govuk-radios-fieldset";

    /// <summary>
    /// Creates a <see cref="RadiosFieldsetTagHelper"/>.
    /// </summary>
    public RadiosFieldsetTagHelper()
    {
    }

    private protected override string LegendTagName => RadiosFieldsetLegendTagHelper.TagName;
}
