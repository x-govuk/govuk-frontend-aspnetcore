using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the legend in a GDS radios component fieldset.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = RadiosFieldsetTagHelper.TagName)]
// Short tag name support provided by LegendTagHelper
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the legend.")]
public class RadiosFieldsetLegendTagHelper : FormGroupFieldsetLegendTagHelperBase
{
    internal const string TagName = "govuk-radios-fieldset-legend";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [
        TagName,
        ShortTagName
    ];
}
