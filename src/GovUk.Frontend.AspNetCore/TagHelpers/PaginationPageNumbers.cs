namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Works out which page numbers a generated pagination component shows.
/// </summary>
internal static class PaginationPageNumbers
{
    /// <summary>
    /// Gets the page numbers to show, in order, with <c>null</c> in place of each run of skipped pages.
    /// </summary>
    /// <remarks>
    /// <para>
    /// As per the guidance, this shows the first page, the page before the current page, the current page,
    /// the page after the current page and the last page, and an ellipsis in place of any skipped pages.
    /// </para>
    /// <para>
    /// An ellipsis takes up as much space as a page number, so where it would replace a single page,
    /// that page is shown instead, e.g. <c>1 2 3 [4] 5 … 100</c> rather than <c>1 … 3 [4] 5 … 100</c>.
    /// This follows the examples in the guidance.
    /// </para>
    /// </remarks>
    public static IReadOnlyList<int?> Get(int currentPage, int totalPages)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(totalPages, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(currentPage, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(currentPage, totalPages);

        var pageNumbers = new[] { 1, currentPage - 1, currentPage, currentPage + 1, totalPages }
            .Where(p => p >= 1 && p <= totalPages)
            .Distinct()
            .Order()
            .ToArray();

        var result = new List<int?>();

        for (var i = 0; i < pageNumbers.Length; i++)
        {
            var pageNumber = pageNumbers[i];

            if (i > 0)
            {
                var previousPageNumber = pageNumbers[i - 1];
                var skipped = pageNumber - previousPageNumber - 1;

                if (skipped == 1)
                {
                    result.Add(previousPageNumber + 1);
                }
                else if (skipped > 1)
                {
                    result.Add(null);
                }
            }

            result.Add(pageNumber);
        }

        return result;
    }
}
