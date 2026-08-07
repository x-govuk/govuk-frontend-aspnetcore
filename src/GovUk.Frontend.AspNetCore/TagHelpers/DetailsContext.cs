using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class DetailsContext
{
    public (AttributeCollection Attributes, IHtmlContent Content)? Summary { get; private set; }

    public (AttributeCollection Attributes, IHtmlContent Content)? Text { get; private set; }

    public void SetSummary(AttributeCollection attributes, IHtmlContent content)
    {
        ArgumentNullException.ThrowIfNull(attributes);
        ArgumentNullException.ThrowIfNull(content);

        if (Summary is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(DetailsSummaryTagHelper.TagName, DetailsTagHelper.TagName);
        }

        if (Text is not null)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(DetailsSummaryTagHelper.TagName, DetailsTextTagHelper.TagName);
        }

        Summary = (attributes, content);
    }

    public void SetText(AttributeCollection attributes, IHtmlContent content)
    {
        ArgumentNullException.ThrowIfNull(attributes);
        ArgumentNullException.ThrowIfNull(content);

        if (Text is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(DetailsTextTagHelper.TagName, DetailsTagHelper.TagName);
        }

        Text = (attributes, content);
    }

    public void ThrowIfNotComplete()
    {
        if (Summary is null)
        {
            throw ExceptionHelper.AChildElementMustBeProvided(DetailsSummaryTagHelper.TagName);
        }

        if (Text is null)
        {
            throw ExceptionHelper.AChildElementMustBeProvided(DetailsTextTagHelper.TagName);
        }
    }
}
