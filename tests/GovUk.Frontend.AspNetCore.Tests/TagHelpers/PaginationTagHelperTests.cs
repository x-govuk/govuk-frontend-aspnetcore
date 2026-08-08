using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class PaginationTagHelperTests : TagHelperTestBase<PaginationTagHelper>
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var landmarkLabel = "Landmark";

        var previousHref = "/place?page=4";
        var previousLabelText = "4 of 9";
        var previousText = "Previous page";

        var currentHref = "place?page=5";
        var currentNumber = "5 of 9";
        var currentVisuallyHiddenText = "vht";

        var nextHref = "/place?page=6";
        var nextLabelText = "6 of 9";
        var nextText = "Next page";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var paginationContext = context.GetContextItem<PaginationContext>();

                paginationContext.SetPrevious(new()
                {
                    Href = previousHref,
                    LabelText = previousLabelText,
                    Text = previousText
                });

                paginationContext.AddItem(new PaginationOptionsItem()
                {
                    Number = currentNumber,
                    VisuallyHiddenText = currentVisuallyHiddenText,
                    Href = currentHref,
                    Current = true,
                    Ellipsis = null,
                    Attributes = null
                });

                paginationContext.AddItem(new PaginationOptionsItem()
                {
                    Ellipsis = true
                });

                paginationContext.SetNext(new()
                {
                    Href = nextHref,
                    LabelText = nextLabelText,
                    Text = nextText
                });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<PaginationOptions>(nameof(IComponentGenerator.GeneratePaginationAsync));

        var tagHelper = new PaginationTagHelper(componentGenerator)
        {
            LandmarkLabel = landmarkLabel
        };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(landmarkLabel, actualOptions.LandmarkLabel?.ToHtmlString(HtmlEncoder.Default));
        Assert.NotNull(actualOptions.Items);
        Assert.NotNull(actualOptions.Previous);
        Assert.Equal(actualOptions.Previous.Href, previousHref);
        Assert.Equal(actualOptions.Previous.LabelText, previousLabelText);
        Assert.Equal(actualOptions.Previous.Text, previousText);
        Assert.NotNull(actualOptions.Next);
        Assert.Equal(actualOptions.Next.Href, nextHref);
        Assert.Equal(actualOptions.Next.LabelText, nextLabelText);
        Assert.Equal(actualOptions.Next.Text, nextText);
        Assert.Collection(
            actualOptions.Items,
            i =>
            {
                var item = Assert.IsType<PaginationOptionsItem>(i);
                Assert.Equal(currentNumber, item.Number);
                Assert.Equal(currentHref, item.Href);
                Assert.Equal(currentVisuallyHiddenText, item.VisuallyHiddenText);
            },
            i =>
            {
                var item = Assert.IsType<PaginationOptionsItem>(i);
                Assert.True(item.Ellipsis);
            });
    }

    [Fact]
    public async Task ProcessAsync_WithGeneratedItems_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var landmarkLabel = "Landmark";
        var className = CreateDummyClassName();
        var dataAttributes = CreateDummyDataAttributes();

        var context = CreateTagHelperContext(className: className, attributes: dataAttributes);
        var output = CreateTagHelperOutput(className: className, attributes: dataAttributes, tagMode: TagMode.SelfClosing);

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<PaginationOptions>(nameof(IComponentGenerator.GeneratePaginationAsync));

        var tagHelper = new PaginationTagHelper(componentGenerator)
        {
            CurrentPage = 5,
            TotalPages = 10,
            GeneratePageHref = GetPageHref,
            LandmarkLabel = landmarkLabel
        };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(landmarkLabel, actualOptions.LandmarkLabel?.ToHtmlString(HtmlEncoder.Default));
        Assert.Equal(className, actualOptions.Classes?.ToHtmlString(HtmlEncoder.Default));
        AssertContainsAttributes(dataAttributes, actualOptions.Attributes);

        Assert.NotNull(actualOptions.Previous);
        Assert.Equal(GetPageHref(4), actualOptions.Previous.Href);

        Assert.NotNull(actualOptions.Next);
        Assert.Equal(GetPageHref(6), actualOptions.Next.Href);

        // The first page, the pages either side of the current page and the last page,
        // with an ellipsis wherever pages have been skipped
        Assert.NotNull(actualOptions.Items);
        Assert.Collection(
            actualOptions.Items,
            item => AssertItem(item, 1, current: false),
            AssertEllipsisItem,
            item => AssertItem(item, 4, current: false),
            item => AssertItem(item, 5, current: true),
            item => AssertItem(item, 6, current: false),
            AssertEllipsisItem,
            item => AssertItem(item, 10, current: false));
    }

    [Fact]
    public async Task ProcessAsync_WithGeneratedItemsAndNoPagesSkipped_DoesNotGenerateEllipsisItems()
    {
        // Arrange
        var context = CreateTagHelperContext();
        var output = CreateTagHelperOutput(tagMode: TagMode.SelfClosing);

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<PaginationOptions>(nameof(IComponentGenerator.GeneratePaginationAsync));

        var tagHelper = new PaginationTagHelper(componentGenerator)
        {
            CurrentPage = 2,
            TotalPages = 3,
            GeneratePageHref = GetPageHref
        };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.Items);
        Assert.Collection(
            actualOptions.Items,
            item => AssertItem(item, 1, current: false),
            item => AssertItem(item, 2, current: true),
            item => AssertItem(item, 3, current: false));
    }

    [Fact]
    public async Task ProcessAsync_WithGeneratedItemsOnFirstPage_DoesNotGeneratePreviousLink()
    {
        // Arrange
        var context = CreateTagHelperContext();
        var output = CreateTagHelperOutput(tagMode: TagMode.SelfClosing);

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<PaginationOptions>(nameof(IComponentGenerator.GeneratePaginationAsync));

        var tagHelper = new PaginationTagHelper(componentGenerator)
        {
            CurrentPage = 1,
            TotalPages = 10,
            GeneratePageHref = GetPageHref
        };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Null(actualOptions.Previous);
        Assert.NotNull(actualOptions.Next);
        Assert.Equal(GetPageHref(2), actualOptions.Next.Href);
        Assert.NotNull(actualOptions.Items);
        Assert.Collection(
            actualOptions.Items,
            item => AssertItem(item, 1, current: true),
            item => AssertItem(item, 2, current: false),
            AssertEllipsisItem,
            item => AssertItem(item, 10, current: false));
    }

    [Fact]
    public async Task ProcessAsync_WithGeneratedItemsOnLastPage_DoesNotGenerateNextLink()
    {
        // Arrange
        var context = CreateTagHelperContext();
        var output = CreateTagHelperOutput(tagMode: TagMode.SelfClosing);

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<PaginationOptions>(nameof(IComponentGenerator.GeneratePaginationAsync));

        var tagHelper = new PaginationTagHelper(componentGenerator)
        {
            CurrentPage = 10,
            TotalPages = 10,
            GeneratePageHref = GetPageHref
        };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Null(actualOptions.Next);
        Assert.NotNull(actualOptions.Previous);
        Assert.Equal(GetPageHref(9), actualOptions.Previous.Href);
        Assert.NotNull(actualOptions.Items);
        Assert.Collection(
            actualOptions.Items,
            item => AssertItem(item, 1, current: false),
            AssertEllipsisItem,
            item => AssertItem(item, 9, current: false),
            item => AssertItem(item, 10, current: true));
    }

    [Fact]
    public async Task ProcessAsync_WithGeneratedItemsAndOnlyOnePage_DoesNotRenderAnything()
    {
        // Arrange
        var context = CreateTagHelperContext();
        var output = CreateTagHelperOutput(tagMode: TagMode.SelfClosing);

        var componentGenerator = TestUtils.CreateComponentGeneratorMock();

        var tagHelper = new PaginationTagHelper(componentGenerator.Object)
        {
            CurrentPage = 1,
            TotalPages = 1,
            GeneratePageHref = GetPageHref
        };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Null(output.TagName);
        componentGenerator.Verify(g => g.GeneratePaginationAsync(It.IsAny<PaginationOptions>()), Times.Never);
    }

    [Fact]
    public async Task ProcessAsync_WithGeneratedItemsAndChildElements_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var paginationContext = context.GetContextItem<PaginationContext>();

                paginationContext.AddItem(new PaginationOptionsItem() { Number = "1", Href = "/place?page=1" });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new PaginationTagHelper(TestUtils.CreateComponentGenerator())
        {
            CurrentPage = 2,
            TotalPages = 3,
            GeneratePageHref = GetPageHref
        };
        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            "Child elements cannot be specified when the 'current-page', 'total-pages' and 'generate-page-href' attributes are specified.",
            ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_WithNoCurrentPage_ThrowsInvalidOperationException()
    {
        // Arrange
        var tagHelper = CreateTagHelperWithGeneratedItems(currentPage: null, totalPages: 3, generatePageHref: GetPageHref);

        // Act
        var ex = await ProcessAndRecordException(tagHelper);

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("The 'current-page' attribute must be specified.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_WithNoGeneratePageHref_ThrowsInvalidOperationException()
    {
        // Arrange
        var tagHelper = CreateTagHelperWithGeneratedItems(currentPage: 2, totalPages: 3, generatePageHref: null);

        // Act
        var ex = await ProcessAndRecordException(tagHelper);

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("The 'generate-page-href' attribute must be specified.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_WithNoTotalPages_ThrowsInvalidOperationException()
    {
        // Arrange
        var tagHelper = CreateTagHelperWithGeneratedItems(currentPage: 2, totalPages: null, generatePageHref: GetPageHref);

        // Act
        var ex = await ProcessAndRecordException(tagHelper);

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("The 'total-pages' attribute must be specified.", ex.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ProcessAsync_WithCurrentPageNotGreaterThanZero_ThrowsInvalidOperationException(int currentPage)
    {
        // Arrange
        var tagHelper = CreateTagHelperWithGeneratedItems(currentPage, totalPages: 3, generatePageHref: GetPageHref);

        // Act
        var ex = await ProcessAndRecordException(tagHelper);

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("The 'current-page' attribute must be greater than 0.", ex.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ProcessAsync_WithTotalPagesNotGreaterThanZero_ThrowsInvalidOperationException(int totalPages)
    {
        // Arrange
        var tagHelper = CreateTagHelperWithGeneratedItems(currentPage: 1, totalPages, generatePageHref: GetPageHref);

        // Act
        var ex = await ProcessAndRecordException(tagHelper);

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("The 'total-pages' attribute must be greater than 0.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_WithCurrentPageGreaterThanTotalPages_ThrowsInvalidOperationException()
    {
        // Arrange
        var tagHelper = CreateTagHelperWithGeneratedItems(currentPage: 4, totalPages: 3, generatePageHref: GetPageHref);

        // Act
        var ex = await ProcessAndRecordException(tagHelper);

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("The 'current-page' attribute cannot be greater than the 'total-pages' attribute.", ex.Message);
    }

    private static string GetPageHref(int page) => $"/results?page={page}";

    private PaginationTagHelper CreateTagHelperWithGeneratedItems(
        int? currentPage,
        int? totalPages,
        Func<int, string>? generatePageHref)
    {
        var tagHelper = new PaginationTagHelper(TestUtils.CreateComponentGenerator())
        {
            CurrentPage = currentPage,
            TotalPages = totalPages
        };

        if (generatePageHref is not null)
        {
            tagHelper.GeneratePageHref = generatePageHref;
        }

        return tagHelper;
    }

    private async Task<Exception?> ProcessAndRecordException(PaginationTagHelper tagHelper)
    {
        var context = CreateTagHelperContext();
        var output = CreateTagHelperOutput(tagMode: TagMode.SelfClosing);
        tagHelper.Init(context);

        return await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));
    }

    private static void AssertItem(PaginationOptionsItem? item, int number, bool current)
    {
        Assert.NotNull(item);
        Assert.Equal(number.ToString(), item.Number?.ToHtmlString(HtmlEncoder.Default));
        Assert.Equal(GetPageHref(number), item.Href);
        Assert.Equal(current, item.Current);
        Assert.NotEqual(true, item.Ellipsis);
    }

    private static void AssertEllipsisItem(PaginationOptionsItem? item)
    {
        Assert.NotNull(item);
        Assert.True(item.Ellipsis);
        Assert.Null(item.Number);
        Assert.Null(item.Href);
    }
}
