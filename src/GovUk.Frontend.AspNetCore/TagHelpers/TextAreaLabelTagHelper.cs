using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the label in a GDS textarea component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TextAreaTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = TextAreaTagHelper.TagName)]
#endif
public class TextAreaLabelTagHelper : FormGroupLabelTagHelperBase
{
    internal const string TagName = "govuk-textarea-label";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [
        TagName
#if SHORT_TAG_NAMES
        ,
        ShortTagName
#endif
    ];
}
