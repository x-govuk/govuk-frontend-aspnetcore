using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS pagination component.
/// </summary>
[HtmlTargetElement(TagName)]
[RestrictChildren(
    PaginationPreviousTagHelper.TagName,
    PaginationPreviousTagHelper.ShortTagName,
    PaginationItemTagHelper.TagName,
    PaginationItemTagHelper.ShortTagName,
    PaginationEllipsisItemTagHelper.TagName,
    PaginationEllipsisItemTagHelper.ShortTagName,
    PaginationNextTagHelper.TagName,
    PaginationNextTagHelper.ShortTagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.Pagination)]
public class PaginationTagHelper : TagHelper
{
    internal const string TagName = "govuk-pagination";

    private const string CurrentPageAttributeName = "current-page";
    private const string GeneratePageHrefAttributeName = "generate-page-href";
    private const string LandmarkLabelAttributeName = "landmark-label";
    private const string TotalPagesAttributeName = "total-pages";

    private readonly IComponentGenerator _componentGenerator;

    /// <summary>
    /// Creates a new <see cref="PaginationTagHelper"/>.
    /// </summary>
    public PaginationTagHelper(IComponentGenerator componentGenerator)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);

        _componentGenerator = componentGenerator;
    }

    /// <summary>
    /// The current page.
    /// </summary>
    /// <remarks>
    /// Specify this, <c>total-pages</c> and <c>generate-page-href</c> to have the items generated
    /// instead of specifying them with child elements.
    /// </remarks>
    [DisallowNull]
    [HtmlAttributeName(CurrentPageAttributeName)]
    public int? CurrentPage { get; set; }

    /// <summary>
    /// A function that given a page number generates the <c>href</c> attribute for that page.
    /// </summary>
    /// <remarks>
    /// Specify this, <c>current-page</c> and <c>total-pages</c> to have the items generated
    /// instead of specifying them with child elements.
    /// </remarks>
    [DisallowNull]
    [HtmlAttributeName(GeneratePageHrefAttributeName)]
    public Func<int, string>? GeneratePageHref { get; set; }

    /// <summary>
    /// The label for the navigation landmark that wraps the pagination.
    /// </summary>
    /// <remarks>
    /// The default is <c>results</c>.
    /// Cannot be <c>null</c> or empty.
    /// </remarks>
    [DisallowNull]
    [HtmlAttributeName(LandmarkLabelAttributeName)]
    public string? LandmarkLabel { get; set; }

    /// <summary>
    /// The total number of pages.
    /// </summary>
    /// <remarks>
    /// Specify this, <c>current-page</c> and <c>generate-page-href</c> to have the items generated
    /// instead of specifying them with child elements.
    /// </remarks>
    [DisallowNull]
    [HtmlAttributeName(TotalPagesAttributeName)]
    public int? TotalPages { get; set; }

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        context.SetContextItem(new PaginationContext());
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var paginationContext = context.GetContextItem<PaginationContext>();

        _ = await output.GetChildContentAsync();

        var generateItems = CurrentPage is not null || TotalPages is not null || GeneratePageHref is not null;

        IReadOnlyCollection<PaginationOptionsItem> items;
        PaginationOptionsPrevious? previous;
        PaginationOptionsNext? next;

        if (generateItems)
        {
            ValidateGeneratedItemsAttributes();

            if (paginationContext.Items.Count > 0 || paginationContext.Previous is not null || paginationContext.Next is not null)
            {
                throw new InvalidOperationException(
                    $"Child elements cannot be specified when the '{CurrentPageAttributeName}', " +
                    $"'{TotalPagesAttributeName}' and '{GeneratePageHrefAttributeName}' attributes are specified.");
            }

            // If there's only one page, don't render anything
            if (TotalPages is 1)
            {
                output.SuppressOutput();
                return;
            }

            (items, previous, next) = GenerateItems();
        }
        else
        {
            items = paginationContext.Items.OfType<PaginationOptionsItem>().ToArray();
            previous = paginationContext.Previous;
            next = paginationContext.Next;
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        var component = await _componentGenerator.GeneratePaginationAsync(new PaginationOptions
        {
            Items = items,
            Previous = previous,
            Next = next,
            LandmarkLabel = LandmarkLabel,
            Classes = classes,
            Attributes = attributes
        });

        component.ApplyToTagHelper(output);
    }

    private void ValidateGeneratedItemsAttributes()
    {
        if (CurrentPage is null)
        {
            throw ExceptionHelper.TheAttributeMustBeSpecified(CurrentPageAttributeName);
        }

        if (CurrentPage <= 0)
        {
            throw new InvalidOperationException($"The '{CurrentPageAttributeName}' attribute must be greater than 0.");
        }

        if (GeneratePageHref is null)
        {
            throw ExceptionHelper.TheAttributeMustBeSpecified(GeneratePageHrefAttributeName);
        }

        if (TotalPages is null)
        {
            throw ExceptionHelper.TheAttributeMustBeSpecified(TotalPagesAttributeName);
        }

        if (TotalPages <= 0)
        {
            throw new InvalidOperationException($"The '{TotalPagesAttributeName}' attribute must be greater than 0.");
        }

        if (CurrentPage > TotalPages)
        {
            throw new InvalidOperationException(
                $"The '{CurrentPageAttributeName}' attribute cannot be greater than the '{TotalPagesAttributeName}' attribute.");
        }
    }

    private (IReadOnlyCollection<PaginationOptionsItem> Items, PaginationOptionsPrevious? Previous, PaginationOptionsNext? Next) GenerateItems()
    {
        var currentPage = CurrentPage!.Value;
        var totalPages = TotalPages!.Value;

        // As per the guidance, show the first page, page before current, current, page after current and last page.
        // Show a Previous link if this is not page 1, show a Next link if this is not the last page.
        var pageNumbers = new[] { 1, currentPage - 1, currentPage, currentPage + 1, totalPages }
            .Where(p => p >= 1 && p <= totalPages)
            .Distinct()
            .Order()
            .ToArray();

        var items = new List<PaginationOptionsItem>();

        for (var i = 0; i < pageNumbers.Length; i++)
        {
            var pageNumber = pageNumbers[i];

            // Wherever pages have been skipped, show an ellipsis in their place
            if (i > 0 && pageNumber > pageNumbers[i - 1] + 1)
            {
                items.Add(new PaginationOptionsItem { Ellipsis = true });
            }

            items.Add(new PaginationOptionsItem
            {
                Href = GeneratePageHref!(pageNumber),
                Number = pageNumber.ToString(CultureInfo.CurrentCulture),
                Current = pageNumber == currentPage
            });
        }

        var previous = currentPage > 1 ?
            new PaginationOptionsPrevious { Href = GeneratePageHref!(currentPage - 1) } :
            null;

        var next = currentPage < totalPages ?
            new PaginationOptionsNext { Href = GeneratePageHref!(currentPage + 1) } :
            null;

        return (items, previous, next);
    }
}
