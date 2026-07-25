using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the legend in a GDS checkboxes component fieldset.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CheckboxesFieldsetTagHelper.TagName)]
// Short tag name support provided by LegendTagHelper
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the legend.")]
public class CheckboxesFieldsetLegendTagHelper : FormGroupFieldsetLegendTagHelperBase
{
    internal const string TagName = "govuk-checkboxes-fieldset-legend";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [
        TagName,
        ShortTagName
    ];
}
