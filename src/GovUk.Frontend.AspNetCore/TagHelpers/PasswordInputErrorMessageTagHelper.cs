using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the error message in a GDS password input component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = PasswordInputTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = PasswordInputTagHelper.TagName)]
#endif
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the component's error message.")]
public class PasswordInputErrorMessageTagHelper : FormGroupErrorMessageTagHelperBase
{
    internal const string TagName = "govuk-password-input-error-message";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [
        TagName
#if SHORT_TAG_NAMES
        ,
        ShortTagName
#endif
    ];
}
