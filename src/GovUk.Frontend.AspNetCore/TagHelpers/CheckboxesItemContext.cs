using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class CheckboxesItemContext
{
    public (CheckboxesOptionsItemConditional Options, string TagName)? Conditional { get; private set; }
    public (HintOptions Options, string TagName)? Hint { get; private set; }

    public void SetConditional(CheckboxesOptionsItemConditional options, string tagName)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(tagName);

        if (Conditional is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                CheckboxesItemConditionalTagHelper.TagName,
                CheckboxesItemTagHelper.TagName);
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
                CheckboxesItemHintTagHelper.TagName,
                CheckboxesItemTagHelper.TagName);
        }

        if (Conditional is not null)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(
                CheckboxesItemHintTagHelper.TagName,
                CheckboxesItemConditionalTagHelper.TagName);
        }

        Hint = (options, tagName);
    }
}
