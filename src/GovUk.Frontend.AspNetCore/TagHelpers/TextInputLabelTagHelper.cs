using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the label in a GDS input component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TextInputTagHelper.TagName)]
//[HtmlTargetElement(ShortTagName, ParentTag = TextInputTagHelper.TagName)]
public class TextInputLabelTagHelper : FormGroupLabelTagHelperBase
{
    internal const string TagName = "govuk-input-label";
}
