using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <inheritdoc/>
[HtmlTargetElement(TagName, ParentTag = DateInputTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = DateInputTagHelper.TagName)]
[HtmlTargetElement(TagName, ParentTag = DateInputFieldsetTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the component's error message.")]
public class DateInputErrorMessageTagHelper : FormGroupErrorMessageTagHelperBase
{
    internal const string TagName = "govuk-date-input-error-message";

    private const string ErrorItemsAttributeName = "error-items";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];

    /// <summary>
    /// The components of the date that have errors (day, month and/or year).
    /// </summary>
    /// <remarks>
    /// If the value for the parent <see cref="DateInputTagHelper"/> was specified using <see cref="DateInputTagHelper.For"/>
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
            content?.Snapshot(),
            context.TagName);
    }
}
