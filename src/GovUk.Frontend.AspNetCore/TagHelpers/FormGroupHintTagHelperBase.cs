using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the hint in a GDS form component.
/// </summary>
public class FormGroupHintTagHelperBase : TagHelper
{
    //private protected const string ShortTagName = ShortTagNames.Hint;

    private protected FormGroupHintTagHelperBase()
    {
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var content = output.TagMode == TagMode.StartTagAndEndTag ?
            await output.GetChildContentAsync() :
            null;

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var formGroupContext = context.GetContextItem<FormGroupContext3>();

        formGroupContext.SetHint(
            new AttributeCollection(output.Attributes),
            content?.ToTemplateString(),
            output.TagName);

        output.SuppressOutput();
    }
}
