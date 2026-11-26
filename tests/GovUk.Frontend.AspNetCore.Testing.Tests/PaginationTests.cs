using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.Testing.Tests;

public class PaginationTests
{
    [Fact]
    public async Task Pagination_ReadsLinksAndItems()
    {
        var doc = await Render.DocumentAsync(g => g.GeneratePaginationAsync(new PaginationOptions
        {
            Previous = new PaginationOptionsPrevious { Href = "?page=1" },
            Next = new PaginationOptionsNext { Href = "?page=3" },
            Items =
            [
                new PaginationOptionsItem { Number = "1", Href = "?page=1" },
                new PaginationOptionsItem { Number = "2", Href = "?page=2", Current = true },
                new PaginationOptionsItem { Number = "3", Href = "?page=3" },
                new PaginationOptionsItem { Ellipsis = true },
                new PaginationOptionsItem { Number = "10", Href = "?page=10" }
            ]
        }));

        var pagination = doc.Pagination;

        Assert.NotNull(pagination);
        Assert.Equal("Previous", pagination.Previous!.Text);
        Assert.Equal("?page=1", pagination.Previous.Href);
        Assert.Equal("Next", pagination.Next!.Text);
        Assert.Equal("?page=3", pagination.Next.Href);

        Assert.Equal(["1", "2", "3", null, "10"], pagination.Items.Select(i => i.Number));
        Assert.True(pagination.Items[3].IsEllipsis);
        Assert.Null(pagination.Items[3].Href);
        Assert.Equal("2", pagination.Current!.Number);
        Assert.Equal("?page=2", pagination.Current.Href);
        Assert.Equal("Pagination: 1 [2] 3 … 10", pagination.ToString());
    }

    [Fact]
    public async Task Pagination_BlockLevel_ReadsLabels()
    {
        var doc = await Render.DocumentAsync(g => g.GeneratePaginationAsync(new PaginationOptions
        {
            Previous = new PaginationOptionsPrevious { Href = "/previous", LabelText = "Applying for a provisional lorry or bus licence" },
            Next = new PaginationOptionsNext { Href = "/next", LabelText = "Driver CPC part 1 test: theory" }
        }));

        var pagination = doc.Pagination!;

        Assert.Empty(pagination.Items);
        Assert.Null(pagination.Current);
        Assert.Equal("Previous", pagination.Previous!.Text);
        Assert.Equal("Applying for a provisional lorry or bus licence", pagination.Previous.Label);
        Assert.Equal("Driver CPC part 1 test: theory", pagination.Next!.Label);
    }

    [Fact]
    public async Task Pagination_FirstPage_HasNoPreviousLink()
    {
        var doc = await Render.DocumentAsync(g => g.GeneratePaginationAsync(new PaginationOptions
        {
            Next = new PaginationOptionsNext { Href = "?page=2" },
            Items =
            [
                new PaginationOptionsItem { Number = "1", Href = "?page=1", Current = true },
                new PaginationOptionsItem { Number = "2", Href = "?page=2" }
            ]
        }));

        var pagination = doc.Pagination!;

        Assert.Null(pagination.Previous);
        Assert.Null(pagination.Next!.Label);
    }
}
