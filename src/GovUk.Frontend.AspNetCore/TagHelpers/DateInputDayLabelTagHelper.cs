using GovUk.Frontend.AspNetCore.HtmlGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the label in the day item of a GDS date input component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = DateInputDayTagHelper.TagName)]
[OutputElementHint(ComponentGenerator.LabelElement)]
public class DateInputDayLabelTagHelper : DateInputItemLabelTagHelperBase
{
    internal const string TagName = "govuk-date-input-day-label";

    /// <summary>
    /// Creates a <see cref="DateInputItemLabelTagHelperBase"/>.
    /// </summary>
    public DateInputDayLabelTagHelper()
    {
    }
}
