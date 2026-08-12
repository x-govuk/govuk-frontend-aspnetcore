using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the legend in a GDS form component's fieldset.
/// </summary>
[HtmlTargetElement(CheckboxesTagName, ParentTag = CheckboxesFieldsetTagHelper.TagName)]
[HtmlTargetElement(CheckboxesTagName, ParentTag = CheckboxesTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = CheckboxesTagHelper.TagName)]
[HtmlTargetElement(DateInputTagName, ParentTag = DateInputFieldsetTagHelper.TagName)]
[HtmlTargetElement(DateInputTagName, ParentTag = DateInputTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = DateInputTagHelper.TagName)]
[HtmlTargetElement(RadiosTagName, ParentTag = RadiosFieldsetTagHelper.TagName)]
[HtmlTargetElement(RadiosTagName, ParentTag = RadiosTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = RadiosTagHelper.TagName)]
[TagHelperDocumentation(CheckboxesTagName, ContentDescription = "The content is the HTML to use within the legend. When this element is specified directly inside the root checkboxes element a fieldset is generated automatically.")]
[TagHelperDocumentation(DateInputTagName, ContentDescription = "The content is the HTML to use within the legend. When this element is specified directly inside the root date input element a fieldset is generated automatically.")]
[TagHelperDocumentation(RadiosTagName, ContentDescription = "The content is the HTML to use within the legend. When this element is specified directly inside the root radios element a fieldset is generated automatically.")]
public class FormGroupFieldsetLegendTagHelper : TagHelper
{
    internal const string CheckboxesTagName = "govuk-checkboxes-fieldset-legend";
    internal const string DateInputTagName = "govuk-date-input-fieldset-legend";
    internal const string RadiosTagName = "govuk-radios-fieldset-legend";
    internal const string ShortTagName = ShortTagNames.Legend;

    private const string IsPageHeadingAttributeName = "is-page-heading";

    /// <summary>
    /// Whether the legend also acts as the heading for the page.
    /// </summary>
    /// <remarks>
    /// The default is <c>false</c>.
    /// </remarks>
    [HtmlAttributeName(IsPageHeadingAttributeName)]
    public bool? IsPageHeading { get; set; }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var fieldsetContext = context.GetContextItem<FormGroupFieldsetContext2>();

        var content = output.TagMode == TagMode.StartTagAndEndTag ?
            await output.GetChildContentAsync() :
            null;

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        fieldsetContext.SetLegend(
            IsPageHeading,
            new AttributeCollection(output.Attributes),
            html: content?.Snapshot(),
            context.TagName);

        output.SuppressOutput();
    }
}
