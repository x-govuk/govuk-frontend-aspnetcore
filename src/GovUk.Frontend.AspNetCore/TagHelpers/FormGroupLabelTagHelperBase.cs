using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.HtmlGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the label in a form component.
/// </summary>
public abstract class FormGroupLabelTagHelperBase : TagHelper
{
    private const string IsPageHeadingAttributeName = "is-page-heading";

#if SHORT_TAG_NAMES
    internal const string ShortTagName = ShortTagNames.Label;
#endif

    private protected FormGroupLabelTagHelperBase()
    {
    }

    /// <summary>
    /// Whether the label also acts as the heading for the page.
    /// </summary>
    /// <remarks>
    /// The default is <c>false</c>.
    /// </remarks>
    [HtmlAttributeName(IsPageHeadingAttributeName)]
    public bool IsPageHeading { get; set; } = ComponentGenerator.LabelDefaultIsPageHeading;

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var content = output.TagMode == TagMode.StartTagAndEndTag ?
            await output.GetChildContentAsync() :
            null;

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var formGroupContext = context.GetContextItem<FormGroupContext3>();

        formGroupContext.SetLabel(
            IsPageHeading,
            new AttributeCollection(output.Attributes),
            content?.ToTemplateString(),
            output.TagName);

        output.SuppressOutput();
    }
}
