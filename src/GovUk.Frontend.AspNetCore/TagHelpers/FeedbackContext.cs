using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class FeedbackContext
{
    private string? _titleTagName;
    private string? _bodyTagName;

    public (IHtmlContent Content, AttributeCollection Attributes)? Title { get; private set; }

    public (IHtmlContent Content, AttributeCollection Attributes)? Body { get; private set; }

    public void SetTitle(IHtmlContent content, AttributeCollection attributes, string tagName)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(attributes);
        ArgumentNullException.ThrowIfNull(tagName);

        CheckChildTagNameSpelling(tagName);

        if (Title is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                FeedbackTitleTagHelper.AllTagNames,
                FeedbackTagHelper.TagName);
        }

        if (_bodyTagName is string bodyTagName)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, bodyTagName);
        }

        Title = (content, attributes);
        _titleTagName = tagName;
    }

    public void SetBody(IHtmlContent content, AttributeCollection attributes, string tagName)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(attributes);
        ArgumentNullException.ThrowIfNull(tagName);

        CheckChildTagNameSpelling(tagName);

        if (Body is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                FeedbackBodyTagHelper.AllTagNames,
                FeedbackTagHelper.TagName);
        }

        Body = (content, attributes);
        _bodyTagName = tagName;
    }

    public void ThrowIfNotComplete()
    {
        if (Title is null)
        {
            throw ExceptionHelper.AChildElementMustBeProvided(FeedbackTitleTagHelper.AllTagNames);
        }
    }

    /// <summary>
    /// Checks that <paramref name="tagName"/> is spelled the same way as the children specified so far.
    /// </summary>
    /// <remarks>
    /// Both children have &lt;govuk-feedback&gt; for their parent, so their spelling cannot be paired
    /// up through <c>ParentTag</c>.
    /// </remarks>
    private void CheckChildTagNameSpelling(string tagName)
    {
        var siblingTagName = _titleTagName ?? _bodyTagName;

        if (siblingTagName is not null && UsesShortTagName(tagName) != UsesShortTagName(siblingTagName))
        {
            throw ExceptionHelper.ShortAndGovUkPrefixedTagNamesCannotBeMixed(tagName, siblingTagName);
        }

        static bool UsesShortTagName(string tagName) => tagName is
            FeedbackTitleTagHelper.ShortTagName or
            FeedbackBodyTagHelper.ShortTagName;
    }
}
