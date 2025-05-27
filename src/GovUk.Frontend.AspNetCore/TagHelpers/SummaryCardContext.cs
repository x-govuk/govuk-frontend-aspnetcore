using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class SummaryCardContext
{
    public SummaryListOptionsCardTitle? Title { get; private set; }

    public SummaryListOptionsCardActions? Actions { get; private set; }

    public SummaryListOptions? SummaryList { get; private set; }

    public void SetTitle(SummaryListOptionsCardTitle title)
    {
        ArgumentNullException.ThrowIfNull(title);

        if (Title is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                SummaryCardTitleTagHelper.TagName,
                SummaryCardTagHelper.TagName);
        }

        if (Actions is not null)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                SummaryCardTitleTagHelper.TagName,
                SummaryCardActionsTagHelper.TagName);
        }

        if (SummaryList is not null)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                SummaryCardTitleTagHelper.TagName,
                SummaryListTagHelper.TagName);
        }

        Title = title;
    }

    public void SetActions(SummaryListOptionsCardActions actions)
    {
        ArgumentNullException.ThrowIfNull(actions);

        if (Actions is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                SummaryCardActionsTagHelper.TagName,
                SummaryCardTagHelper.TagName);
        }

        if (SummaryList is not null)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                SummaryCardActionsTagHelper.TagName,
                SummaryListTagHelper.TagName);
        }

        Actions = actions;
    }

    public void SetSummaryList(SummaryListOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (SummaryList is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                SummaryListTagHelper.TagName,
                SummaryCardTagHelper.TagName);
        }

        SummaryList = options;
    }

    public void ThrowIfNotComplete()
    {
        if (SummaryList is null)
        {
            throw ExceptionHelper.AChildElementMustBeProvided(SummaryListTagHelper.TagName);
        }
    }
}
