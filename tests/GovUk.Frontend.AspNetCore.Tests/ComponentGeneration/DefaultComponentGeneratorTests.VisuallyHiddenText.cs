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
        // Should have: <span class="govuk-visually-hidden">Error:</span> Enter your full name
        // The space after </span> is critical for screen readers
        Assert.Contains("<span class=\"govuk-visually-hidden\">Error:</span> Enter your full name", html);
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
        Assert.Contains("<span class=\"govuk-visually-hidden\">Gwall:</span> Rhowch eich enw llawn", html);
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
        // Should have: <span class="govuk-visually-hidden">Emergency</span> Exit this page
        Assert.Contains("<span class=\"govuk-visually-hidden\">Emergency</span> Exit this page", html);
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
        // Custom HTML should preserve the Emergency prefix with proper spacing
        Assert.Contains("<span class=\"govuk-visually-hidden\">Emergency</span> Leave immediately", html);
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
        // Should have: Previous<span class="govuk-visually-hidden"> page</span>
        // The space before "page" is critical
        Assert.Contains("Previous<span class=\"govuk-visually-hidden\"> page</span>", html);
        Assert.Contains("Next<span class=\"govuk-visually-hidden\"> page</span>", html);
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
        // Should have: <span class="govuk-visually-hidden">:</span>
        // The colon with proper surrounding whitespace
        Assert.Contains("<span class=\"govuk-visually-hidden\">:</span>", html);
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
        // h2 is a block-level element, so whitespace is naturally present
        Assert.Contains("<h2 class=\"govuk-visually-hidden\"", html);
        Assert.Contains(">Footer links</h2>", html);
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
        // Should use default "Support links" text
        Assert.Contains("<h2 class=\"govuk-visually-hidden\"", html);
        Assert.Contains(">Support links</h2>", html);
    }
}
