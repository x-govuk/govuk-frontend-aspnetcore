using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the label in a GDS textarea component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TextAreaTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = TextAreaTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the component's label.")]
public class TextAreaLabelTagHelper : FormGroupLabelTagHelperBase
{
    internal const string TagName = "govuk-textarea-label";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];
}
