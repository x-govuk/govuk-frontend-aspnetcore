using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the legend in a GDS form component's fieldset.
/// </summary>
public abstract class FormGroupFieldsetLegendTagHelperBase : TagHelper
{
    private const string IsPageHeadingAttributeName = "is-page-heading";

    private protected FormGroupFieldsetLegendTagHelperBase()
    {
    }

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
