using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents a task in a GDS task list component.
/// </summary>
/// <remarks>
/// Specify an <c>href</c> attribute, or use the <c>asp-*</c> attributes, to link the task's title.
/// Any other attributes are added to the generated <c>li</c> element.
/// </remarks>
[HtmlTargetElement(TagName, ParentTag = TaskListTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = TaskListTagHelper.TagName)]
[RestrictChildren(
    TaskListItemTitleTagHelper.TagName,
    TaskListItemTitleTagHelper.ShortTagName,
    TaskListItemHintTagHelper.TagName,
    TaskListItemHintTagHelper.ShortTagName,
    TaskListItemStatusTagHelper.TagName,
    TaskListItemStatusTagHelper.ShortTagName)]
public class TaskListItemTagHelper : TagHelper
{
    internal const string TagName = "govuk-task-list-item";
    internal const string ShortTagName = ShortTagNames.TaskListItem;

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.SetContextItem(new TaskListItemContext(context.TagName));
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var taskListContext = context.GetContextItem<TaskListContext>();
        var itemContext = context.GetContextItem<TaskListItemContext>();

        _ = await output.GetChildContentAsync();

        itemContext.ThrowIfIncomplete();

        var href = output.GetUrlAttribute("href");

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);
        attributes.Remove("href", out _);

        taskListContext.AddItem(new TaskListOptionsItem
        {
            Title = itemContext.Title,
            Hint = itemContext.Hint,
            Status = itemContext.Status,
            Href = href,
            Classes = classes,
            Attributes = attributes
        });

        output.SuppressOutput();
    }
}
