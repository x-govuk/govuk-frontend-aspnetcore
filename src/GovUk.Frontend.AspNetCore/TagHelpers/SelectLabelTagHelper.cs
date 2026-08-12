using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the label in a GDS select component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = SelectTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = SelectTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the component's label.")]
public class SelectLabelTagHelper : FormGroupLabelTagHelperBase
{
    internal const string TagName = "govuk-select-label";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];
}
