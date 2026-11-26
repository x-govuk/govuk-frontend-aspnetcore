using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the status of a task in a GDS task list component.
/// </summary>
/// <remarks>
/// To show the status as a tag, put a <c>&lt;govuk-tag&gt;</c> element inside this element.
/// Attributes are added to the generated status <c>div</c> element.
/// </remarks>
[HtmlTargetElement(TagName, ParentTag = TaskListItemTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = TaskListItemTagHelper.ShortTagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the task's status.")]
public class TaskListItemStatusTagHelper : TagHelper
{
    internal const string TagName = "govuk-task-list-item-status";
    internal const string ShortTagName = ShortTagNames.Status;

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

        itemContext.SetStatus(
            new TaskListOptionsItemStatus
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
