using GovUk.Frontend.AspNetCore.Components;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class SummaryListRowContext
{
    public SummaryListOptionsRowKey? Key { get; private set; }

    public SummaryListOptionsRowValue? Value { get; private set; }

    public SummaryListOptionsRowActions? Actions { get; private set; }

    public void SetActions(SummaryListOptionsRowActions actions)
    {
        ArgumentNullException.ThrowIfNull(actions);

        if (Actions is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                SummaryListRowActionsTagHelper.TagName,
                SummaryListRowTagHelper.TagName);
        }

        Actions = actions;
    }

    public void SetKey(SummaryListOptionsRowKey key)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (Key is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                SummaryListRowKeyTagHelper.TagName,
                SummaryListRowTagHelper.TagName);
        }

        if (Value is not null)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                SummaryListRowKeyTagHelper.TagName,
                SummaryListRowValueTagHelper.TagName);
        }

        if (Actions is not null)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                SummaryListRowKeyTagHelper.TagName,
                SummaryListRowActionsTagHelper.TagName);
        }

        Key = key;
    }

    public void SetValue(SummaryListOptionsRowValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (Value is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                SummaryListRowValueTagHelper.TagName,
                SummaryListRowTagHelper.TagName);
        }

        if (Actions is not null)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                SummaryListRowValueTagHelper.TagName,
                SummaryListRowActionsTagHelper.TagName);
        }

        Value = value;
    }

    public void ThrowIfIncomplete()
    {
        if (Key is null)
        {
            throw ExceptionHelper.AChildElementMustBeProvided(SummaryListRowKeyTagHelper.TagName);
        }
    }
}
