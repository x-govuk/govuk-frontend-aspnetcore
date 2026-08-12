using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the value of a GDS character count component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CharacterCountTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = CharacterCountTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the generated textarea.")]
public class CharacterCountValueTagHelper : TagHelper
{
    internal const string TagName = "govuk-character-count-value";
    internal const string ShortTagName = ShortTagNames.Value;

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var characterCountContext = context.GetContextItem<CharacterCountContext>();

        characterCountContext.SetValue(content.Snapshot(), context.TagName);

        output.SuppressOutput();
    }
}
