using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class SummaryCardContext
{
    private (SummaryListOptionsCardTitle Options, string TagName)? _title;
    private (SummaryListOptionsCardActions Options, string TagName)? _actions;
    private (SummaryListOptions Options, string TagName)? _summaryList;

    public SummaryListOptionsCardTitle? Title => _title?.Options;

    public SummaryListOptionsCardActions? Actions => _actions?.Options;

    public SummaryListOptions? SummaryList => _summaryList?.Options;

    public void SetTitle(SummaryListOptionsCardTitle options, string tagName)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(tagName);

        if (_title is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                [SummaryCardTitleTagHelper.ShortTagName, SummaryCardTitleTagHelper.TagName],
                SummaryCardTagHelper.TagName);
        }

        if (_actions is var (_, actionsTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                actionsTagName);
        }

        if (_summaryList is var (_, summaryListTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                summaryListTagName);
        }

        _title = (options, tagName);
    }

    public void SetActions(SummaryListOptionsCardActions options, string tagName)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(tagName);

        if (_actions is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                [SummaryCardActionsTagHelper.ShortTagName, SummaryCardActionsTagHelper.TagName],
                SummaryCardTagHelper.TagName);
        }

        if (_summaryList is var (_, summaryListTagName))
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                tagName,
                summaryListTagName);
        }

        _actions = (options, tagName);
    }

    public void SetSummaryList(SummaryListOptions options, string tagName)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(tagName);

        if (SummaryList is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                [SummaryListTagHelper.TagName],
                SummaryCardTagHelper.TagName);
        }

        _summaryList = (options, tagName);
    }

    public void ThrowIfNotComplete()
    {
        if (SummaryList is null)
        {
            throw ExceptionHelper.AChildElementMustBeProvided(SummaryListTagHelper.TagName);
        }
    }
}
