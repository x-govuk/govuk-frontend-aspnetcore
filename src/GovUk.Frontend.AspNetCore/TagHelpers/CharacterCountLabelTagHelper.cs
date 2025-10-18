using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the label in a GDS character count component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CharacterCountTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = CharacterCountTagHelper.TagName)]
#endif
public class CharacterCountLabelTagHelper : FormGroupLabelTagHelperBase
{
    internal const string TagName = "govuk-character-count-label";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [
        TagName
#if SHORT_TAG_NAMES
        ,
        ShortTagName
#endif
    ];
}
