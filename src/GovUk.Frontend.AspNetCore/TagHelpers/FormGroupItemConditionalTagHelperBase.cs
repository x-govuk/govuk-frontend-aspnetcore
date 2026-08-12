using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the conditional reveal of an item in a GDS form component.
/// </summary>
public abstract class FormGroupItemConditionalTagHelperBase : TagHelper
{
    private protected FormGroupItemConditionalTagHelperBase()
    {
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var itemContext = context.GetContextItem<FormGroupItemContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        itemContext.SetConditional(
            new AttributeCollection(output.Attributes),
            content.Snapshot(),
            context.TagName);

        output.SuppressOutput();
    }
}
