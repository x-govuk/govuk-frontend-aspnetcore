using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the label in a GDS date input component item.
/// </summary>
public abstract class DateInputItemLabelTagHelperBase : TagHelper
{
    /// <summary>
    /// Creates a <see cref="DateInputItemLabelTagHelperBase"/>.
    /// </summary>
    private protected DateInputItemLabelTagHelperBase()
    {
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var dateInputItemContext = context.GetContextItem<DateInputItemContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);

        dateInputItemContext.SetLabel(
            content.ToTemplateString(),
            attributes,
            output.TagName);

        output.SuppressOutput();
    }
}
