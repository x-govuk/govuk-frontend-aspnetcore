using GovUk.Frontend.AspNetCore.HtmlGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the divider text to separate items in a GDS checkboxes component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CheckboxesTagHelper.TagName)]
[HtmlTargetElement(TagName, ParentTag = CheckboxesFieldsetTagHelper.TagName)]
public class CheckboxesItemDividerTagHelper : TagHelper
{
    internal const string TagName = "govuk-checkboxes-divider";

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var checkboxesContext = context.GetContextItem<CheckboxesContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        checkboxesContext.AddItem(new CheckboxesItemDivider()
        {
            Attributes = output.Attributes.ToAttributeDictionary(),
            Content = content.Snapshot()
        });

        output.SuppressOutput();
    }
}
