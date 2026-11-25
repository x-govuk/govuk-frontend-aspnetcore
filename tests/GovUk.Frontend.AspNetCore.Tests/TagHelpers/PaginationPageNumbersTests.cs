using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class PaginationPageNumbersTests
{
    [Theory]
    // The examples from https://design-system.service.gov.uk/components/pagination/
    [InlineData(1, 100, "[1] 2 … 100")]
    [InlineData(2, 100, "1 [2] 3 … 100")]
    [InlineData(3, 100, "1 2 [3] 4 … 100")]
    [InlineData(4, 100, "1 2 3 [4] 5 … 100")]
    [InlineData(5, 100, "1 … 4 [5] 6 … 100")]
    [InlineData(98, 100, "1 … 97 [98] 99 100")]
    [InlineData(99, 100, "1 … 98 [99] 100")]
    [InlineData(100, 100, "1 … 99 [100]")]
    [InlineData(7, 42, "1 … 6 [7] 8 … 42")]
    // A single skipped page next to the last page is shown, as it is next to the first page
    [InlineData(97, 100, "1 … 96 [97] 98 99 100")]
    // Few pages
    [InlineData(1, 1, "[1]")]
    [InlineData(1, 2, "[1] 2")]
    [InlineData(2, 2, "1 [2]")]
    [InlineData(1, 3, "[1] 2 3")]
    [InlineData(2, 3, "1 [2] 3")]
    [InlineData(3, 3, "1 2 [3]")]
    [InlineData(1, 4, "[1] 2 3 4")]
    [InlineData(1, 5, "[1] 2 … 5")]
    [InlineData(3, 5, "1 2 [3] 4 5")]
    [InlineData(4, 7, "1 2 3 [4] 5 6 7")]
    // Large totals
    [InlineData(500_000, 1_000_000, "1 … 499999 [500000] 500001 … 1000000")]
    [InlineData(int.MaxValue, int.MaxValue, "1 … 2147483646 [2147483647]")]
    public void Get_ReturnsExpectedPageNumbers(int currentPage, int totalPages, string expected)
    {
        // Act
        var result = PaginationPageNumbers.Get(currentPage, totalPages);

        // Assert
        var actual = string.Join(' ', result.Select(p => p is int n ? (n == currentPage ? $"[{n}]" : $"{n}") : "…"));
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(11, 10)]
    [InlineData(1, 0)]
    public void Get_WithOutOfRangeArguments_ThrowsArgumentOutOfRangeException(int currentPage, int totalPages)
    {
        // Act
        var ex = Record.Exception(() => PaginationPageNumbers.Get(currentPage, totalPages));

        // Assert
        Assert.IsType<ArgumentOutOfRangeException>(ex);
    }
}
