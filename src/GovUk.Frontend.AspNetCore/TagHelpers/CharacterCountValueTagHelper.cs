using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the value of a GDS character count component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CharacterCountTagHelper.TagName)]
public class CharacterCountValueTagHelper : TagHelper
{
    internal const string TagName = "govuk-character-count-value";

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

        characterCountContext.SetValue(content.ToTemplateString(), output.TagName);

        output.SuppressOutput();
    }
}
