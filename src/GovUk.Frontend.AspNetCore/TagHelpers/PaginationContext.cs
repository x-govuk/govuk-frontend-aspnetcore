using System.Diagnostics;
using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class PaginationContext
{
    private readonly List<(object Item, string TagName)> _items = [];
    private string? _nextTagName;
    private string? _previousTagName;

    public IReadOnlyCollection<object> Items => _items.Select(i => i.Item).ToArray();

    public PaginationOptionsNext? Next { get; private set; }

    public PaginationOptionsPrevious? Previous { get; private set; }

    public void AddItem(object item, string tagName)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(tagName);
        Debug.Assert(item is PaginationOptionsItem or PaginationOptionsNext or PaginationOptionsPrevious);

        CheckChildTagNameSpelling(tagName);

        if (_nextTagName is string nextTagName)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, nextTagName);
        }

        // Only one 'current' item is allowed.
        if (item is PaginationOptionsItem { Current: true } &&
            _items.Any(i => i.Item is PaginationOptionsItem { Current: true }))
        {
            throw new InvalidOperationException($"Only one current {tagName} is permitted.");
        }

        _items.Add((item, tagName));
    }

    public void SetNext(PaginationOptionsNext next, string tagName)
    {
        ArgumentNullException.ThrowIfNull(next);
        ArgumentNullException.ThrowIfNull(tagName);

        CheckChildTagNameSpelling(tagName);

        if (Next is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                PaginationNextTagHelper.AllTagNames,
                PaginationTagHelper.TagName);
        }

        Next = next;
        _nextTagName = tagName;
    }

    public void SetPrevious(PaginationOptionsPrevious previous, string tagName)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(tagName);

        CheckChildTagNameSpelling(tagName);

        if (_items.Count != 0)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, _items[0].TagName);
        }

        if (_nextTagName is string nextTagName)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, nextTagName);
        }

        if (Previous is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                PaginationPreviousTagHelper.AllTagNames,
                PaginationTagHelper.TagName);
        }

        Previous = previous;
        _previousTagName = tagName;
    }

    /// <summary>
    /// Checks that <paramref name="tagName"/> is spelled the same way as the children specified so far.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The children all have &lt;govuk-pagination&gt; for their parent, so their spelling cannot be
    /// paired up through <c>ParentTag</c>.
    /// </para>
    /// <para>
    /// Every child goes through this context, so the check lives here rather than in the tag helpers,
    /// which keeps it ahead of the ordering checks — reordering does not fix a spelling mismatch.
    /// </para>
    /// </remarks>
    private void CheckChildTagNameSpelling(string tagName)
    {
        var siblingTagName =
            _previousTagName ?? (_items.Count > 0 ? _items[0].TagName : null) ?? _nextTagName;

        if (siblingTagName is not null && UsesShortTagName(tagName) != UsesShortTagName(siblingTagName))
        {
            throw ExceptionHelper.ShortAndGovUkPrefixedTagNamesCannotBeMixed(tagName, siblingTagName);
        }

        static bool UsesShortTagName(string tagName) => tagName is
            PaginationItemTagHelper.ShortTagName or
            PaginationEllipsisItemTagHelper.ShortTagName or
            PaginationPreviousTagHelper.ShortTagName or
            PaginationNextTagHelper.ShortTagName;
    }
}
