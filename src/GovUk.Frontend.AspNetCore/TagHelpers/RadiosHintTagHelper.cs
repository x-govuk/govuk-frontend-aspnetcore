using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the hint in a GDS radios component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = RadiosTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = RadiosTagHelper.TagName)]
[HtmlTargetElement(TagName, ParentTag = RadiosFieldsetTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the component's hint.")]
public class RadiosHintTagHelper : FormGroupHintTagHelperBase
{
    internal const string TagName = "govuk-radios-hint";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];
}
