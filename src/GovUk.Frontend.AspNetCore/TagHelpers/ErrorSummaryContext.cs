using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class ErrorSummaryContext
{
    private readonly List<ErrorSummaryContextItem> _items;
    private string? _firstItemTagName;
    private string? _descriptionTagName;
    private string? _titleTagName;

    public ErrorSummaryContext()
    {
        _items = [];
    }

    public bool HaveExplicitItems { get; set; }

    public IReadOnlyCollection<ErrorSummaryContextItem> Items => _items;

    public (AttributeCollection Attributes, IHtmlContent Html)? Description { get; private set; }

    public (AttributeCollection Attributes, IHtmlContent Html)? Title { get; private set; }

    public void AddItem(ErrorSummaryContextItem item, string tagName)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(tagName);

        CheckChildTagNameSpelling(tagName);

        _items.Add(item);
        _firstItemTagName ??= tagName;
        HaveExplicitItems = true;
    }

    public void SetDescription(AttributeCollection attributes, IHtmlContent html, string tagName)
    {
        ArgumentNullException.ThrowIfNull(attributes);
        ArgumentNullException.ThrowIfNull(html);
        ArgumentNullException.ThrowIfNull(tagName);

        CheckChildTagNameSpelling(tagName);

        if (Description is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                ErrorSummaryDescriptionTagHelper.AllTagNames,
                ErrorSummaryTagHelper.TagName);
        }

        Description = (attributes, html);
        _descriptionTagName = tagName;
    }

    public void SetTitle(AttributeCollection attributes, IHtmlContent html, string tagName)
    {
        ArgumentNullException.ThrowIfNull(attributes);
        ArgumentNullException.ThrowIfNull(html);
        ArgumentNullException.ThrowIfNull(tagName);

        CheckChildTagNameSpelling(tagName);

        if (Title is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                ErrorSummaryTitleTagHelper.AllTagNames,
                ErrorSummaryTagHelper.TagName);
        }

        Title = (attributes, html);
        _titleTagName = tagName;
    }

    /// <summary>
    /// Checks that <paramref name="tagName"/> is spelled the same way as the children specified so far.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The children all have &lt;govuk-error-summary&gt; for their parent, so their spelling cannot be
    /// paired up through <c>ParentTag</c>.
    /// </para>
    /// <para>
    /// Every child goes through this context, so the check lives here rather than in the tag helpers,
    /// as the pagination's does.
    /// </para>
    /// </remarks>
    private void CheckChildTagNameSpelling(string tagName)
    {
        var siblingTagName = _titleTagName ?? _descriptionTagName ?? _firstItemTagName;

        if (siblingTagName is not null && UsesShortTagName(tagName) != UsesShortTagName(siblingTagName))
        {
            throw ExceptionHelper.ShortAndGovUkPrefixedTagNamesCannotBeMixed(tagName, siblingTagName);
        }

        static bool UsesShortTagName(string tagName) => tagName is
            ErrorSummaryTitleTagHelper.ShortTagName or
            ErrorSummaryDescriptionTagHelper.ShortTagName or
            ErrorSummaryItemTagHelper.ShortTagName;
    }
}

internal record ErrorSummaryContextItem(
    TemplateString? Href,
    IHtmlContent Html,
    AttributeCollection Attributes,
    AttributeCollection ItemAttributes);
