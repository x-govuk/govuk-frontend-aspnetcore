using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the title of a task in a GDS task list component.
/// </summary>
/// <remarks>
/// Attributes are added to the generated <c>a</c> element, or to the <c>div</c> element if the task has no link.
/// </remarks>
[HtmlTargetElement(TagName, ParentTag = TaskListItemTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = TaskListItemTagHelper.ShortTagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the task's title.")]
public class TaskListItemTitleTagHelper : TagHelper
{
    internal const string TagName = "govuk-task-list-item-title";
    internal const string ShortTagName = ShortTagNames.TaskTitle;

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var itemContext = context.GetContextItem<TaskListItemContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        itemContext.SetTitle(
            new TaskListOptionsItemTitle
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
