using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class TableContext
{
    private readonly List<TableOptionsRow?> _rows = [];

    private string? _captionTagName;
    private string? _headTagName;
    private string? _rowTagName;

    public (TemplateString Content, TemplateString? Classes, AttributeCollection Attributes)? Caption { get; private set; }

    public (IReadOnlyCollection<TableOptionsHead> Cells, AttributeCollection Attributes)? Head { get; private set; }

    public IReadOnlyCollection<TableOptionsRow?> Rows => _rows.AsReadOnly();

    public void SetCaption(TemplateString content, TemplateString? classes, AttributeCollection attributes, string tagName)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(attributes);
        ArgumentNullException.ThrowIfNull(tagName);

        CheckChildTagNameSpelling(tagName);

        if (Caption is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                TableCaptionTagHelper.AllTagNames,
                TableTagHelper.TagName);
        }

        if (_headTagName is string headTagName)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, headTagName);
        }

        if (_rowTagName is string rowTagName)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, rowTagName);
        }

        Caption = (content, classes, attributes);
        _captionTagName = tagName;
    }

    public void SetHead(IReadOnlyCollection<TableOptionsHead> cells, AttributeCollection attributes, string tagName)
    {
        ArgumentNullException.ThrowIfNull(cells);
        ArgumentNullException.ThrowIfNull(attributes);
        ArgumentNullException.ThrowIfNull(tagName);

        CheckChildTagNameSpelling(tagName);

        if (Head is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(
                TableHeadTagHelper.AllTagNames,
                TableTagHelper.TagName);
        }

        if (_rowTagName is string rowTagName)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(tagName, rowTagName);
        }

        Head = (cells, attributes);
        _headTagName = tagName;
    }

    public void AddRow(TableOptionsRow row, string tagName)
    {
        ArgumentNullException.ThrowIfNull(row);
        ArgumentNullException.ThrowIfNull(tagName);

        CheckChildTagNameSpelling(tagName);

        _rows.Add(row);
        _rowTagName = tagName;
    }

    /// <summary>
    /// Checks that <paramref name="tagName"/> is spelled the same way as the children specified so far.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Every child has &lt;govuk-table&gt; for its parent, so their spelling cannot be paired up
    /// through <c>ParentTag</c>.
    /// </para>
    /// <para>
    /// They all go through this context, so the check lives here rather than in the tag helpers, which
    /// keeps it ahead of the ordering check — reordering does not fix a spelling mismatch.
    /// </para>
    /// </remarks>
    private void CheckChildTagNameSpelling(string tagName)
    {
        var siblingTagName = _captionTagName ?? _headTagName ?? _rowTagName;

        if (siblingTagName is not null && UsesShortTagName(tagName) != UsesShortTagName(siblingTagName))
        {
            throw ExceptionHelper.ShortAndGovUkPrefixedTagNamesCannotBeMixed(tagName, siblingTagName);
        }

        static bool UsesShortTagName(string tagName) => tagName is
            TableCaptionTagHelper.ShortTagName or
            TableHeadTagHelper.ShortTagName or
            TableRowTagHelper.ShortTagName;
    }
}
