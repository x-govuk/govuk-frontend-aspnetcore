using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal abstract class FormGroupItemContext
{
    internal record ConditionalInfo(AttributeCollection Attributes, IHtmlContent? Html, string TagName);

    public ConditionalInfo? Conditional { get; private set; }

    public (HintOptions Options, string TagName)? Hint { get; private set; }

    protected abstract string ConditionalTagName { get; }

    protected abstract string HintTagName { get; }

    protected abstract string ItemTagName { get; }

    public void SetConditional(AttributeCollection attributes, IHtmlContent? html, string tagName)
    {
        ArgumentNullException.ThrowIfNull(attributes);
        ArgumentNullException.ThrowIfNull(tagName);

        if (Conditional is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(ConditionalTagName, ItemTagName);
        }

        Conditional = new ConditionalInfo(attributes, html, tagName);
    }

    public void SetHint(HintOptions options, string tagName)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(tagName);

        if (Hint is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(HintTagName, ItemTagName);
        }

        if (Conditional is not null)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(HintTagName, ConditionalTagName);
        }

        Hint = (options, tagName);
    }
}
