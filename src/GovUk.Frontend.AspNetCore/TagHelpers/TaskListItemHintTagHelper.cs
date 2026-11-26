using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the hint for a task in a GDS task list component.
/// </summary>
/// <remarks>
/// Attributes are added to the generated hint <c>div</c> element.
/// </remarks>
[HtmlTargetElement(TagName, ParentTag = TaskListItemTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = TaskListItemTagHelper.ShortTagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the task's hint.")]
public class TaskListItemHintTagHelper : TagHelper
{
    internal const string TagName = "govuk-task-list-item-hint";
    internal const string ShortTagName = ShortTagNames.Hint;

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

        itemContext.SetHint(
            new TaskListOptionsItemHint
            {
                Text = null,
                Html = content.Snapshot(),
                Attributes = attributes
            },
            context.TagName);

        output.SuppressOutput();
    }
}
