using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the prefix element in a GDS text input component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TextInputTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = TextInputTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the component's prefix.")]
public class TextInputPrefixTagHelper : TagHelper
{
    internal const string TagName = "govuk-input-prefix";
    internal const string ShortTagName = ShortTagNames.Prefix;

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];

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
            new InputOptionsPrefix
            {
                Text = null,
                Html = content.Snapshot(),
                Classes = classes,
                Attributes = attributes
            },
            context.TagName);

        output.SuppressOutput();
    }
}
