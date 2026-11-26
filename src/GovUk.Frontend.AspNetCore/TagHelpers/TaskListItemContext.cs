using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class TaskListItemContext(string itemTagName)
{
    private (TaskListOptionsItemTitle Options, string TagName)? _title;
    private (TaskListOptionsItemHint Options, string TagName)? _hint;
    private (TaskListOptionsItemStatus Options, string TagName)? _status;

    public TaskListOptionsItemTitle? Title => _title?.Options;

    public TaskListOptionsItemHint? Hint => _hint?.Options;

    public TaskListOptionsItemStatus? Status => _status?.Options;

    public void SetTitle(TaskListOptionsItemTitle options, string tagName)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(tagName);

        if (_title is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                [TaskListItemTitleTagHelper.ShortTagName, TaskListItemTitleTagHelper.TagName],
                itemTagName);
        }

        if (_hint is var (_, hintTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, hintTagName);
        }

        if (_status is var (_, statusTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, statusTagName);
        }

        _title = (options, tagName);
    }

    public void SetHint(TaskListOptionsItemHint options, string tagName)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(tagName);

        if (_hint is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                [TaskListItemHintTagHelper.ShortTagName, TaskListItemHintTagHelper.TagName],
                itemTagName);
        }

        if (_status is var (_, statusTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, statusTagName);
        }

        _hint = (options, tagName);
    }

    public void SetStatus(TaskListOptionsItemStatus options, string tagName)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(tagName);

        if (_status is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                [TaskListItemStatusTagHelper.ShortTagName, TaskListItemStatusTagHelper.TagName],
                itemTagName);
        }

        _status = (options, tagName);
    }

    public void ThrowIfIncomplete()
    {
        if (_title is null)
        {
            throw ExceptionHelper.AChildElementMustBeProvided(TaskListItemTitleTagHelper.TagName);
        }
    }
}
