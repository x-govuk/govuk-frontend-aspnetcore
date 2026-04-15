using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class SummaryListRowContext(string rowTagName)
{
    private (SummaryListOptionsRowKey Options, string TagName)? _key;
    private (SummaryListOptionsRowValue Options, string TagName)? _value;
    private (SummaryListOptionsRowActions Options, string TagName)? _actions;

    public SummaryListOptionsRowKey? Key => _key?.Options;

    public SummaryListOptionsRowValue? Value => _value?.Options;

    public SummaryListOptionsRowActions? Actions => _actions?.Options;

    public void SetActions(SummaryListOptionsRowActions options, string tagName)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(tagName);

        if (_actions is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                [SummaryListRowActionsTagHelper.ShortTagName, SummaryListRowActionsTagHelper.TagName],
                rowTagName);
        }

        _actions = (options, tagName);
    }

    public void SetKey(SummaryListOptionsRowKey options, string tagName)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(tagName);

        if (_key is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                [SummaryListRowKeyTagHelper.ShortTagName, SummaryListRowKeyTagHelper.TagName],
                rowTagName);
        }

        if (_value is var (_, valueTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                valueTagName);
        }

        if (_actions is var (_, actionsTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                actionsTagName);
        }

        _key = (options, tagName);
    }

    public void SetValue(SummaryListOptionsRowValue value, string tagName)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(tagName);

        if (_value is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                [SummaryListRowValueTagHelper.ShortTagName, SummaryListRowValueTagHelper.TagName],
                rowTagName);
        }

        if (_actions is var (_, actionsTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                actionsTagName);
        }

        _value = (value, tagName);
    }

    public void ThrowIfIncomplete()
    {
        if (_key is null)
        {
            throw ExceptionHelper.AChildElementMustBeProvided(SummaryListRowKeyTagHelper.TagName);
        }
    }
}
