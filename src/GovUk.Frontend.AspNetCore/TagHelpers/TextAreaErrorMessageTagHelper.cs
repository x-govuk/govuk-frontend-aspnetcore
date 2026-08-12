using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the error message in a GDS textarea component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TextAreaTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = TextAreaTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the component's error message.")]
public class TextAreaErrorMessageTagHelper : FormGroupErrorMessageTagHelperBase
{
    internal const string TagName = "govuk-textarea-error-message";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];
}
