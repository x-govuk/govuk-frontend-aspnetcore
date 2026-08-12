using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the hint in a GDS date input component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = DateInputTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = DateInputTagHelper.TagName)]
[HtmlTargetElement(TagName, ParentTag = DateInputFieldsetTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the component's hint.")]
public class DateInputHintTagHelper : FormGroupHintTagHelperBase
{
    internal const string TagName = "govuk-date-input-hint";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];
}
