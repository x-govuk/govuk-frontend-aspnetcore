using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the hint in a GDS password input component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = PasswordInputTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = PasswordInputTagHelper.TagName)]
#endif
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the component's hint.")]
public class PasswordInputHintTagHelper : FormGroupHintTagHelperBase
{
    internal const string TagName = "govuk-password-input-hint";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [
        TagName
#if SHORT_TAG_NAMES
        ,
        ShortTagName
#endif
    ];
}
