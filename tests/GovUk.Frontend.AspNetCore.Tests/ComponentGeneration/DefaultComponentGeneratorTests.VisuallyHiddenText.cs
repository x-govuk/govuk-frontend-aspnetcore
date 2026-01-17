using System.Text.RegularExpressions;
using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.Tests.ComponentGeneration;

public partial class DefaultComponentGeneratorTests
{
    [Fact]
    public async Task ErrorMessage_WithVisuallyHiddenText_HasCorrectWhitespace()
    {
        // Arrange
        var options = new ErrorMessageOptions
        {
            VisuallyHiddenText = "Error",
            Text = "Enter your full name"
        };

        // Act
        var result = await _componentGenerator.GenerateErrorMessageAsync(options);
        var html = result.GetHtml();

        // Assert
        // Should have at least one whitespace character after </span>
        var pattern = new Regex(@"<span class=""govuk-visually-hidden"">Error:</span>\s", RegexOptions.Singleline);
        Assert.True(pattern.IsMatch(html), "Error message should have whitespace after visually-hidden span");
    }

    [Fact]
    public async Task ErrorMessage_WithCustomVisuallyHiddenText_HasCorrectWhitespace()
    {
        // Arrange
        var options = new ErrorMessageOptions
        {
            VisuallyHiddenText = "Gwall",
            Text = "Rhowch eich enw llawn"
        };

        // Act
        var result = await _componentGenerator.GenerateErrorMessageAsync(options);
        var html = result.GetHtml();

        // Assert
        // Should have at least one whitespace character after </span>
        var pattern = new Regex(@"<span class=""govuk-visually-hidden"">Gwall:</span>\s", RegexOptions.Singleline);
        Assert.True(pattern.IsMatch(html), "Error message with custom visually-hidden text should have whitespace after visually-hidden span");
    }

    [Fact]
    public async Task WarningText_WithDefaultIconFallbackText_HasCorrectWhitespace()
    {
        // Arrange
        var options = new WarningTextOptions
        {
            Text = "You can be fined up to £5,000 if you don't register."
        };

        // Act
        var result = await _componentGenerator.GenerateWarningTextAsync(options);
        var html = result.GetHtml();

        // Assert
        // Should have at least one whitespace character between </span> and the warning text
        var pattern = new Regex(@"<span class=""govuk-visually-hidden"">Warning</span>\s", RegexOptions.Singleline);
        Assert.True(pattern.IsMatch(html), "Warning text should have whitespace after visually-hidden span");
    }

    [Fact]
    public async Task WarningText_WithCustomIconFallbackText_HasCorrectWhitespace()
    {
        // Arrange
        var options = new WarningTextOptions
        {
            IconFallbackText = "Rhybudd",
            Text = "Gallwch gael dirwy hyd at £5,000 os na fyddwch yn cofrestru."
        };

        // Act
        var result = await _componentGenerator.GenerateWarningTextAsync(options);
        var html = result.GetHtml();

        // Assert
        // Should have at least one whitespace character after </span>
        var pattern = new Regex(@"<span class=""govuk-visually-hidden"">Rhybudd</span>\s", RegexOptions.Singleline);
        Assert.True(pattern.IsMatch(html), "Warning text with custom icon fallback should have whitespace after visually-hidden span");
    }

    [Fact]
    public async Task ExitThisPage_WithDefaultText_HasCorrectWhitespace()
    {
        // Arrange
        var options = new ExitThisPageOptions();

        // Act
        var result = await _componentGenerator.GenerateExitThisPageAsync(options);
        var html = result.GetHtml();

        // Assert
        // Should have at least one whitespace character after </span>
        var pattern = new Regex(@"<span class=""govuk-visually-hidden"">Emergency</span>\s", RegexOptions.Singleline);
        Assert.True(pattern.IsMatch(html), "Exit this page should have whitespace after visually-hidden span");
    }

    [Fact]
    public async Task ExitThisPage_WithCustomText_HasCorrectWhitespace()
    {
        // Arrange
        var options = new ExitThisPageOptions
        {
            Html = "<span class=\"govuk-visually-hidden\">Emergency</span> Leave immediately"
        };

        // Act
        var result = await _componentGenerator.GenerateExitThisPageAsync(options);
        var html = result.GetHtml();

        // Assert
        // Should have at least one whitespace character after </span>
        var pattern = new Regex(@"<span class=""govuk-visually-hidden"">Emergency</span>\s", RegexOptions.Singleline);
        Assert.True(pattern.IsMatch(html), "Exit this page with custom HTML should have whitespace after visually-hidden span");
    }

    [Fact]
    public async Task Pagination_WithPreviousAndNext_HasCorrectWhitespace()
    {
        // Arrange
        var options = new PaginationOptions
        {
            Previous = new PaginationOptionsPrevious
            {
                Href = "/previous"
            },
            Next = new PaginationOptionsNext
            {
                Href = "/next"
            }
        };

        // Act
        var result = await _componentGenerator.GeneratePaginationAsync(options);
        var html = result.GetHtml();

        // Assert
        // Should have at least one whitespace character after opening <span> tag
        var previousPattern = new Regex(@"<span class=""govuk-visually-hidden"">\s+page</span>", RegexOptions.Singleline);
        Assert.True(previousPattern.IsMatch(html), "Pagination should have whitespace before 'page' in visually-hidden span");
    }

    [Fact]
    public async Task Pagination_WithLabelText_HasCorrectWhitespace()
    {
        // Arrange
        var options = new PaginationOptions
        {
            Previous = new PaginationOptionsPrevious
            {
                Href = "/previous",
                LabelText = "1 of 3"
            },
            Next = new PaginationOptionsNext
            {
                Href = "/next",
                LabelText = "3 of 3"
            }
        };

        // Act
        var result = await _componentGenerator.GeneratePaginationAsync(options);
        var html = result.GetHtml();

        // Assert
        // Should have at least one whitespace character before or after the colon
        var pattern = new Regex(@"<span class=""govuk-visually-hidden"">\s*:\s*</span>", RegexOptions.Singleline);
        Assert.True(pattern.IsMatch(html), "Pagination with label text should have visually-hidden colon");
    }

    [Fact]
    public async Task SummaryList_WithRowAction_HasCorrectWhitespace()
    {
        // Arrange
        var options = new SummaryListOptions
        {
            Rows = new[]
            {
                new SummaryListOptionsRow
                {
                    Key = new SummaryListOptionsRowKey { Text = "Name" },
                    Value = new SummaryListOptionsRowValue { Text = "Sarah Philips" },
                    Actions = new SummaryListOptionsRowActions
                    {
                        Items = new[]
                        {
                            new SummaryListOptionsRowActionsItem
                            {
                                Href = "#",
                                Text = "Change",
                                VisuallyHiddenText = "name"
                            }
                        }
                    }
                }
            }
        };

        // Act
        var result = await _componentGenerator.GenerateSummaryListAsync(options);
        var html = result.GetHtml();

        // Assert
        // Should have at least one whitespace character after opening <span> tag
        var pattern = new Regex(@"<span class=""govuk-visually-hidden"">\s", RegexOptions.Singleline);
        Assert.True(pattern.IsMatch(html), "Summary list action should have whitespace after opening visually-hidden span tag");
    }

    [Fact]
    public async Task SummaryList_WithCardAction_HasCorrectWhitespace()
    {
        // Arrange
        var options = new SummaryListOptions
        {
            Card = new SummaryListOptionsCard
            {
                Title = new SummaryListOptionsCardTitle { Text = "University of Gloucestershire" },
                Actions = new SummaryListOptionsCardActions
                {
                    Items = new[]
                    {
                        new SummaryListOptionsCardActionsItem
                        {
                            Href = "#",
                            Text = "Delete choice",
                            VisuallyHiddenText = "of University of Gloucestershire"
                        }
                    }
                }
            },
            Rows = new[]
            {
                new SummaryListOptionsRow
                {
                    Key = new SummaryListOptionsRowKey { Text = "Course" },
                    Value = new SummaryListOptionsRowValue { Text = "English" }
                }
            }
        };

        // Act
        var result = await _componentGenerator.GenerateSummaryListAsync(options);
        var html = result.GetHtml();

        // Assert
        // Should have at least one whitespace character after opening <span> tag
        var pattern = new Regex(@"<span class=""govuk-visually-hidden"">\s", RegexOptions.Singleline);
        Assert.True(pattern.IsMatch(html), "Summary card action should have whitespace after opening visually-hidden span tag");
    }

    [Fact]
    public async Task Footer_WithMeta_HasCorrectWhitespace()
    {
        // Arrange
        var options = new FooterOptions
        {
            Meta = new FooterOptionsMeta
            {
                VisuallyHiddenTitle = "Footer links"
            }
        };

        // Act
        var result = await _componentGenerator.GenerateFooterAsync(options);
        var html = result.GetHtml();

        // Assert
        // Should have at least one whitespace character after opening <h2> tag
        var pattern = new Regex(@"<h2 class=""govuk-visually-hidden""[^>]*>\s*Footer links\s*</h2>", RegexOptions.Singleline);
        Assert.True(pattern.IsMatch(html), "Footer with meta should have visually-hidden h2 with custom title");
    }

    [Fact]
    public async Task Footer_WithDefaultVisuallyHiddenTitle_HasCorrectWhitespace()
    {
        // Arrange
        var options = new FooterOptions
        {
            Meta = new FooterOptionsMeta()
        };

        // Act
        var result = await _componentGenerator.GenerateFooterAsync(options);
        var html = result.GetHtml();

        // Assert
        // Should have at least one whitespace character after opening <h2> tag
        var pattern = new Regex(@"<h2 class=""govuk-visually-hidden""[^>]*>\s*Support links\s*</h2>", RegexOptions.Singleline);
        Assert.True(pattern.IsMatch(html), "Footer with default meta should have visually-hidden h2 with default title");
    }
}
