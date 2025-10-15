using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the prefix element in a GDS input component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TextInputTagHelper.TagName)]
//[HtmlTargetElement(ShortTagName, ParentTag = TextInputTagHelper.TagName)]
public class TextInputPrefixTagHelper : TagHelper
{
    internal const string TagName = "govuk-input-prefix";
    //internal const string ShortTagName = ShortTagNames.Prefix;

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var inputContext = context.GetContextItem<TextInputContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        inputContext.SetPrefix(
            new InputOptionsPrefix()
            {
                Text = null,
                Html = content.ToTemplateString(),
                Classes = classes,
                Attributes = attributes
            },
            output.TagName);

        output.SuppressOutput();
    }
}
