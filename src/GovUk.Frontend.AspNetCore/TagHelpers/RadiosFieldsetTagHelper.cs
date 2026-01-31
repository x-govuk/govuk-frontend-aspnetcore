using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the fieldset in a GDS radios component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = RadiosTagHelper.TagName)]
[RestrictChildren(
    RadiosFieldsetLegendTagHelper.TagName,
    RadiosItemTagHelper.TagName,
    RadiosItemDividerTagHelper.TagName,
    RadiosTagHelper.HintTagName,
    RadiosTagHelper.ErrorMessageTagName
)]
public class RadiosFieldsetTagHelper : TagHelper
{
    internal const string TagName = "govuk-radios-fieldset";

    private const string DescribedByAttributeName = "described-by";

    /// <summary>
    /// One or more element IDs to add to the <c>aria-describedby</c> attribute.
    /// </summary>
    [HtmlAttributeName(DescribedByAttributeName)]
    public string? DescribedBy { get; set; }

    /// <summary>
    /// Creates a <see cref="RadiosFieldsetTagHelper"/>.
    /// </summary>
    public RadiosFieldsetTagHelper()
    {
    }

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        var radiosContext = context.GetContextItem<RadiosContext>();

        var fieldsetContext = new RadiosFieldsetContext(
            DescribedBy,
            @for: radiosContext.For);

        context.SetContextItem(fieldsetContext);
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var radiosContext = context.GetContextItem<RadiosContext>();
        var fieldsetContext = context.GetContextItem<RadiosFieldsetContext>();

        radiosContext.OpenFieldset(fieldsetContext, new AttributeCollection(output.Attributes));

        _ = await output.GetChildContentAsync();

        fieldsetContext.ThrowIfNotComplete(RadiosFieldsetLegendTagHelper.TagName);
        radiosContext.CloseFieldset();

        output.SuppressOutput();
    }
}
