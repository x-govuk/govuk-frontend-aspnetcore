using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the hint in a GDS select component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = SelectTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = SelectTagHelper.TagName)]
#endif
public class SelectHintTagHelper : FormGroupHintTagHelperBase
{
    internal const string TagName = "govuk-select-hint";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [
        TagName
#if SHORT_TAG_NAMES
        ,
        ShortTagName
#endif
    ];
}
