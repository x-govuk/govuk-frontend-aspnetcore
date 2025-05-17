using GovUk.Frontend.AspNetCore.Components;
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

        var childContent = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            childContent = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);

        dateInputItemContext.SetLabel(
            childContent.ToTemplateString(),
            attributes,
            output.TagName);

        output.SuppressOutput();
    }
}
