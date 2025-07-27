using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the hint in a GDS date input component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = DateInputTagHelper.TagName)]
//[HtmlTargetElement(ShortTagName, ParentTag = DateInputTagHelper.TagName)]
public class DateInputHintTagHelper : FormGroupHintTagHelperBase
{
    internal const string TagName = "govuk-date-input-hint";
}
