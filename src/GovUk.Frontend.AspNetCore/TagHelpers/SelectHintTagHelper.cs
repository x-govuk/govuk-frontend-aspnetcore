using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the hint in a GDS select component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = SelectTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = SelectTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the component's hint.")]
public class SelectHintTagHelper : FormGroupHintTagHelperBase
{
    internal const string TagName = "govuk-select-hint";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];
}
