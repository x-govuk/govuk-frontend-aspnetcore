using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class PhaseBannerContext
{
    public (TagOptions Options, string TagName)? Tag { get; private set; }

    public void SetTag(TagOptions options, string tagName)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(tagName);

        if (Tag is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(tagName, PhaseBannerTagHelper.TagName);
        }

        Tag = (options, tagName);
    }

    public void ThrowIfIncomplete()
    {
        if (Tag is null)
        {
            throw ExceptionHelper.AChildElementMustBeProvided(PhaseBannerTagTagHelper.TagName);
        }
    }
}
