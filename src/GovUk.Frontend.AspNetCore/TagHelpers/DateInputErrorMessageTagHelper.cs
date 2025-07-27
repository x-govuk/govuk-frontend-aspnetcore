using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <inheritdoc/>
[HtmlTargetElement(TagName, ParentTag = DateInputTagHelper.TagName)]
//[HtmlTargetElement(ShortTagName, ParentTag = DateInputTagHelper.TagName)]
[HtmlTargetElement(TagName, ParentTag = DateInputFieldsetTagHelper.TagName)]
//[HtmlTargetElement(ShortTagName, ParentTag = DateInputFieldsetTagHelper.TagName)]
//[HtmlTargetElement(TagName, ParentTag = DateInputFieldsetTagHelper.ShortTagName)]
//[HtmlTargetElement(ShortTagName, ParentTag = DateInputFieldsetTagHelper.ShortTagName)]
public class DateInputErrorMessageTagHelper : FormGroupErrorMessageTagHelperBase
{
    internal const string TagName = "govuk-date-input-error-message";

    private const string ErrorItemsAttributeName = "error-items";

    /// <summary>
    /// The components of the date that have errors (day, month and/or year).
    /// </summary>
    /// <remarks>
    /// If the value for the parent <see cref="DateInputTagHelper"/> was specified using <see cref="FormGroupTagHelperBase.For"/>
    /// then <see cref="ErrorItems"/> will be computed from model binding errors.
    /// </remarks>
    [HtmlAttributeName(ErrorItemsAttributeName)]
    public DateInputItemTypes? ErrorItems { get; set; }

    private protected override void SetErrorMessage(TagHelperContent? content, TagHelperContext context, TagHelperOutput output)
    {
        var dateInputContext = context.GetContextItem<DateInputContext>();

        var attributes = new AttributeCollection(output.Attributes);

        dateInputContext.SetErrorMessage(
            ErrorItems,
            VisuallyHiddenText,
            attributes,
            content?.ToTemplateString(),
            output.TagName);
    }
}
