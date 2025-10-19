using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the prefix element in a GDS text input component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TextInputTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = TextInputTagHelper.TagName)]
#endif
public class TextInputPrefixTagHelper : TagHelper
{
    internal const string TagName = "govuk-input-prefix";
#if SHORT_TAG_NAMES
    internal const string ShortTagName = ShortTagNames.Prefix;
#endif

    internal static IReadOnlyCollection<string> AllTagNames { get; } = new[]
    {
        TagName
#if SHORT_TAG_NAMES
        ,
        ShortTagName
#endif
    };

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
