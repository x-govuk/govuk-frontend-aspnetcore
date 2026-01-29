using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the label in a GDS select component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = SelectTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = SelectTagHelper.TagName)]
#endif
public class SelectLabelTagHelper : FormGroupLabelTagHelperBase
{
    internal const string TagName = "govuk-select-label";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [
        TagName
#if SHORT_TAG_NAMES
        ,
        ShortTagName
#endif
    ];
}
