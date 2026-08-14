using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class DetailsContext
{
    private string? _summaryTagName;
    private string? _textTagName;

    public (AttributeCollection Attributes, IHtmlContent Content)? Summary { get; private set; }

    public (AttributeCollection Attributes, IHtmlContent Content)? Text { get; private set; }

    public void SetSummary(AttributeCollection attributes, IHtmlContent content, string tagName)
    {
        ArgumentNullException.ThrowIfNull(attributes);
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(tagName);

        CheckChildTagNameSpelling(tagName);

        if (Summary is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                DetailsSummaryTagHelper.AllTagNames,
                DetailsTagHelper.TagName);
        }

        if (_textTagName is string textTagName)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, textTagName);
        }

        Summary = (attributes, content);
        _summaryTagName = tagName;
    }

    public void SetText(AttributeCollection attributes, IHtmlContent content, string tagName)
    {
        ArgumentNullException.ThrowIfNull(attributes);
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(tagName);

        CheckChildTagNameSpelling(tagName);

        if (Text is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                DetailsTextTagHelper.AllTagNames,
                DetailsTagHelper.TagName);
        }

        Text = (attributes, content);
        _textTagName = tagName;
    }

    public void ThrowIfNotComplete()
    {
        if (Summary is null)
        {
            throw ExceptionHelper.AChildElementMustBeProvided(DetailsSummaryTagHelper.AllTagNames);
        }

        if (Text is null)
        {
            throw ExceptionHelper.AChildElementMustBeProvided(DetailsTextTagHelper.AllTagNames);
        }
    }

    /// <summary>
    /// Checks that <paramref name="tagName"/> is spelled the same way as the children specified so far.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Both children have &lt;govuk-details&gt; for their parent, so their spelling cannot be paired
    /// up through <c>ParentTag</c>.
    /// </para>
    /// <para>
    /// Both go through this context, so the check lives here rather than in the tag helpers, which
    /// keeps it ahead of the ordering check — reordering does not fix a spelling mismatch.
    /// </para>
    /// </remarks>
    private void CheckChildTagNameSpelling(string tagName)
    {
        var siblingTagName = _summaryTagName ?? _textTagName;

        if (siblingTagName is not null && UsesShortTagName(tagName) != UsesShortTagName(siblingTagName))
        {
            throw ExceptionHelper.ShortAndGovUkPrefixedTagNamesCannotBeMixed(tagName, siblingTagName);
        }

        static bool UsesShortTagName(string tagName) => tagName is
            DetailsSummaryTagHelper.ShortTagName or
            DetailsTextTagHelper.ShortTagName;
    }
}
