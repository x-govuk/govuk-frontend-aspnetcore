using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the month item in a GDS date input component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = DateInputTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = DateInputTagHelper.TagName)]
[HtmlTargetElement(TagName, ParentTag = DateInputFieldsetTagHelper.TagName)]
[RestrictChildren(DateInputMonthLabelTagHelper.TagName, DateInputMonthLabelTagHelper.ShortTagName)]
public class DateInputMonthTagHelper : DateInputItemTagHelperBase
{
    internal const string TagName = "govuk-date-input-month";
    internal const string ShortTagName = ShortTagNames.Month;

    /// <summary>
    /// Creates a <see cref="DateInputMonthTagHelper"/>.
    /// </summary>
    public DateInputMonthTagHelper() : base(DateInputItemTypes.Month, labelTagName: DateInputMonthLabelTagHelper.TagName)
    {
    }

    /// <inheritdoc/>
    private protected override string ItemTagName => TagName;
}
