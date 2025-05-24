using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the day item in a GDS date input component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = DateInputTagHelper.TagName)]
[HtmlTargetElement(TagName, ParentTag = DateInputFieldsetTagHelper.TagName)]
[RestrictChildren(DateInputDayLabelTagHelper.TagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.FormGroup)]
public class DateInputDayTagHelper : DateInputItemTagHelperBase
{
    internal const string TagName = "govuk-date-input-day";

    /// <summary>
    /// Creates a <see cref="DateInputDayTagHelper"/>.
    /// </summary>
    public DateInputDayTagHelper() : base(DateInputItemTypes.Day, labelTagName: DateInputDayLabelTagHelper.TagName)
    {
    }
}
