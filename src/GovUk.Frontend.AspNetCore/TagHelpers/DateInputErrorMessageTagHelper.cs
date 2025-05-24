using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.HtmlGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <inheritdoc/>
[HtmlTargetElement(DateInputTagHelper.ErrorMessageTagName, ParentTag = DateInputTagHelper.TagName)]
[HtmlTargetElement(DateInputTagHelper.ErrorMessageTagName, ParentTag = DateInputFieldsetTagHelper.TagName)]
[OutputElementHint(ComponentGenerator.ErrorMessageElement)]
public class DateInputErrorMessageTagHelper : FormGroupErrorMessageTagHelper3
{
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

    private protected override void SetErrorMessage(TagHelperContent? childContent, TagHelperContext context, TagHelperOutput output)
    {
        var dateInputContext = context.GetContextItem<DateInputContext>();

        var attributes = new AttributeCollection(output.Attributes);

        dateInputContext.SetErrorMessage(
            ErrorItems,
            VisuallyHiddenText,
            attributes,
            childContent?.ToTemplateString(),
            output.TagName);
    }
}
