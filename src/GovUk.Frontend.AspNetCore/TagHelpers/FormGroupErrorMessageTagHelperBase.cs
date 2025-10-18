using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the error message in a GDS form component.
/// </summary>
public abstract class FormGroupErrorMessageTagHelperBase : TagHelper
{
#if SHORT_TAG_NAMES
    internal const string ShortTagName = ShortTagNames.ErrorMessage;
#endif

    private const string VisuallyHiddenTextAttributeName = "visually-hidden-text";

    private protected FormGroupErrorMessageTagHelperBase()
    {
    }

    /// <summary>
    /// A visually hidden prefix used before the error message.
    /// </summary>
    /// <remarks>
    /// The default is <c>&quot;Error&quot;</c>.
    /// </remarks>
    [HtmlAttributeName(VisuallyHiddenTextAttributeName)]
    public string? VisuallyHiddenText { get; set; }

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

        SetErrorMessage(content, context, output);

        output.SuppressOutput();
    }

    private protected virtual void SetErrorMessage(TagHelperContent? content, TagHelperContext context, TagHelperOutput output)
    {
        var formGroupContext3 = context.GetContextItem<FormGroupContext3>();

        formGroupContext3.SetErrorMessage(
            VisuallyHiddenText,
            new AttributeCollection(output.Attributes),
            content?.ToTemplateString(),
            output.TagName);
    }
}
