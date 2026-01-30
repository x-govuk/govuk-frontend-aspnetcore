using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the hint in a GDS date input component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CheckboxesTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = CheckboxesTagHelper.TagName)]
#endif
[HtmlTargetElement(TagName, ParentTag = CheckboxesFieldsetTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = CheckboxesFieldsetTagHelper.TagName)]
#endif
public class CheckboxesHintTagHelper : FormGroupHintTagHelperBase
{
    internal const string TagName = "govuk-checkboxes-hint";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [
        TagName
#if SHORT_TAG_NAMES
        ,
        ShortTagName
#endif
    ];
}
