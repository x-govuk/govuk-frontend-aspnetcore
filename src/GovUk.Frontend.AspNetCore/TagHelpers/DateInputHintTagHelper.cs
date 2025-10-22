using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the hint in a GDS date input component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = DateInputTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = DateInputTagHelper.TagName)]
#endif
[HtmlTargetElement(TagName, ParentTag = DateInputFieldsetTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = DateInputFieldsetTagHelper.TagName)]
#endif
public class DateInputHintTagHelper : FormGroupHintTagHelperBase
{
    internal const string TagName = "govuk-date-input-hint";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [
        TagName
#if SHORT_TAG_NAMES
        ,
        ShortTagName
#endif
    ];
}
