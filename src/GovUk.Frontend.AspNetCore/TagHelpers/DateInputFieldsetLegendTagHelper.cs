using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the legend in a GDS date input component's fieldset.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = DateInputFieldsetTagHelper.TagName)]
// Short tag name support provided by LegendTagHelper
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the legend.")]
public class DateInputFieldsetLegendTagHelper : FormGroupFieldsetLegendTagHelperBase
{
    internal const string TagName = "govuk-date-input-fieldset-legend";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [
        TagName,
        ShortTagName
    ];
}
