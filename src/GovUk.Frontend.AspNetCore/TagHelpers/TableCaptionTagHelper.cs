using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents a caption for a GDS table component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TableTagHelper.TagName)]
public class TableCaptionTagHelper : TagHelper
{
    internal const string TagName = "govuk-table-caption";

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var tableContext = context.GetContextItem<TableContext>();

        var childContent = await output.GetChildContentAsync();

        if (tableContext.Caption is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(TagName, TableTagHelper.TagName);
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        tableContext.Caption = new(childContent.GetContent());
        tableContext.CaptionClasses = classes;

        output.SuppressOutput();
    }
}
