using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class AccordionTagHelperTests : TagHelperTestBase<AccordionTagHelper>
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var id = "test-accordion";
        var headingLevel = 3;
        var rememberExpanded = false;
        var hideAllSectionsText = "Collapse all";
        var hideSectionText = "Collapse";
        var hideSectionAriaLabelText = "Collapse this";
        var showAllSectionsText = "Expand all";
        var showSectionText = "Expand";
        var showSectionAriaLabelText = "Expand this";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();

        var items = new[]
        {
            new AccordionOptionsItem()
            {
                Content = new AccordionOptionsItemContent() { Html = "First content" },
                Expanded = false,
                Heading = new AccordionOptionsItemHeading() { Html = "First heading" },
                Summary = new AccordionOptionsItemSummary() { Html = "First summary" }
            },
            new AccordionOptionsItem()
            {
                Content = new AccordionOptionsItemContent() { Html = "Second content" },
                Expanded = true,
                Heading = new AccordionOptionsItemHeading() { Html = "Second heading" }
            }
        };

        var context = CreateTagHelperContext(className: className, attributes: attributes);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var accordionContext = context.GetContextItem<AccordionContext>();

                foreach (var item in items)
                {
                    accordionContext.AddItem(item);
                }

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<AccordionOptions>(nameof(IComponentGenerator.GenerateAccordionAsync));

        var tagHelper = new AccordionTagHelper(componentGenerator)
        {
            Id = id,
            HeadingLevel = headingLevel,
            RememberExpanded = rememberExpanded,
            HideAllSectionsText = hideAllSectionsText,
            HideSectionText = hideSectionText,
            HideSectionAriaLabelText = hideSectionAriaLabelText,
            ShowAllSectionsText = showAllSectionsText,
            ShowSectionText = showSectionText,
            ShowSectionAriaLabelText = showSectionAriaLabelText
        };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(id, actualOptions.Id);
        Assert.Equal(headingLevel, actualOptions.HeadingLevel);
        Assert.Equal(rememberExpanded, actualOptions.RememberExpanded);
        Assert.Equal(hideAllSectionsText, actualOptions.HideAllSectionsText);
        Assert.Equal(hideSectionText, actualOptions.HideSectionText);
        Assert.Equal(hideSectionAriaLabelText, actualOptions.HideSectionAriaLabelText);
        Assert.Equal(showAllSectionsText, actualOptions.ShowAllSectionsText);
        Assert.Equal(showSectionText, actualOptions.ShowSectionText);
        Assert.Equal(showSectionAriaLabelText, actualOptions.ShowSectionAriaLabelText);
        Assert.Equal(className, actualOptions.Classes);
        AssertContainsAttributes(attributes, actualOptions.Attributes);
        Assert.NotNull(actualOptions.Items);
        Assert.Equal(2, actualOptions.Items.Count);

        var firstItem = actualOptions.Items.ElementAt(0);
        Assert.NotNull(firstItem);
        Assert.Equal(items[0].Expanded, firstItem.Expanded);
        Assert.NotNull(firstItem.Heading);
        Assert.Equal("First heading", firstItem.Heading.Html);
        Assert.NotNull(firstItem.Summary);
        Assert.Equal("First summary", firstItem.Summary.Html);
        Assert.NotNull(firstItem.Content);
        Assert.Equal("First content", firstItem.Content.Html);

        var secondItem = actualOptions.Items.ElementAt(1);
        Assert.NotNull(secondItem);
        Assert.Equal(items[1].Expanded, secondItem.Expanded);
        Assert.NotNull(secondItem.Heading);
        Assert.Equal("Second heading", secondItem.Heading.Html);
        Assert.Null(secondItem.Summary);
        Assert.NotNull(secondItem.Content);
        Assert.Equal("Second content", secondItem.Content.Html);
    }

    [Fact]
    public async Task ProcessAsync_NoId_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var accordionContext = context.GetContextItem<AccordionContext>();

                accordionContext.AddItem(new AccordionOptionsItem()
                {
                    Content = new AccordionOptionsItemContent() { Html = "First content" },
                    Expanded = false,
                    Heading = new AccordionOptionsItemHeading() { Html = "First heading" },
                    Summary = new AccordionOptionsItemSummary() { Html = "First summary" }
                });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new AccordionTagHelper(TestUtils.CreateComponentGenerator());
        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("The 'id' attribute must be specified.", ex.Message);
    }
}
