using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the hint in a GDS textarea component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TextAreaTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = TextAreaTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the component's hint.")]
public class TextAreaHintTagHelper : FormGroupHintTagHelperBase
{
    internal const string TagName = "govuk-textarea-hint";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];
}
