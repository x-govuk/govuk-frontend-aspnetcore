using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS task list component.
/// </summary>
[HtmlTargetElement(TagName)]
[RestrictChildren(TaskListItemTagHelper.TagName, TaskListItemTagHelper.ShortTagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.TaskList)]
public class TaskListTagHelper : TagHelper
{
    internal const string TagName = "govuk-task-list";

    private const string IdPrefixAttributeName = "id-prefix";

    private readonly IComponentGenerator _componentGenerator;

    /// <summary>
    /// Creates a new <see cref="TaskListTagHelper"/>.
    /// </summary>
    public TaskListTagHelper(IComponentGenerator componentGenerator)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);

        _componentGenerator = componentGenerator;
    }

    /// <summary>
    /// The prefix for the <c>id</c> attributes of each item's hint and status.
    /// </summary>
    /// <remarks>
    /// If not specified, <c>task-list</c> will be used.
    /// </remarks>
    [HtmlAttributeName(IdPrefixAttributeName)]
    public string? IdPrefix { get; set; }

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.SetContextItem(new TaskListContext());
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var taskListContext = context.GetContextItem<TaskListContext>();

        _ = await output.GetChildContentAsync();

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        var component = await _componentGenerator.GenerateTaskListAsync(new TaskListOptions
        {
            Items = taskListContext.Items,
            Classes = classes,
            Attributes = attributes,
            IdPrefix = IdPrefix
        });

        component.ApplyToTagHelper(output);
    }
}
