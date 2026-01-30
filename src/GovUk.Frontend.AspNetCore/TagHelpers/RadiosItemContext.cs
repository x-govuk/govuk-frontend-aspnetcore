using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class RadiosItemContext
{
    public (RadiosOptionsItemConditional Options, string TagName)? Conditional { get; private set; }
    public (HintOptions Options, string TagName)? Hint { get; private set; }

    public void SetConditional(RadiosOptionsItemConditional options, string tagName)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(tagName);

        if (Conditional is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                RadiosItemConditionalTagHelper.TagName,
                RadiosItemTagHelper.TagName);
        }

        Conditional = (options, tagName);
    }

    public void SetHint(HintOptions options, string tagName)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(tagName);

        if (Hint is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                RadiosItemHintTagHelper.TagName,
                RadiosItemTagHelper.TagName);
        }

        if (Conditional is not null)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                RadiosItemHintTagHelper.TagName,
                RadiosItemConditionalTagHelper.TagName);
        }

        Hint = (options, tagName);
    }
}
