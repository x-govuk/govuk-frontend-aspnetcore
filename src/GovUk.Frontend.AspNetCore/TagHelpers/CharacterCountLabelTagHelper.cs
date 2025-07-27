using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the label in a GDS character count component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CharacterCountTagHelper.TagName)]
//[HtmlTargetElement(ShortTagName, ParentTag = CharacterCountTagHelper.TagName)]
public class CharacterCountLabelTagHelper : FormGroupLabelTagHelperBase
{
    internal const string TagName = "govuk-character-count-label";
}
