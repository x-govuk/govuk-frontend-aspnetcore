using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class AccordionTagHelperTests
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
        var classes = "custom-class";
        var dataFooAttrValue = "bar";

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

        var context = new TagHelperContext(
            tagName: "govuk-accordion",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-accordion",
            attributes: new TagHelperAttributeList()
            {
                { "class", classes },
                { "data-foo", dataFooAttrValue }
            },
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

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        AccordionOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateAccordionAsync(It.IsAny<AccordionOptions>())).Callback<AccordionOptions>(o => actualOptions = o);

        var tagHelper = new AccordionTagHelper(componentGeneratorMock.Object)
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

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actualOptions);
        Assert.Equal(id, actualOptions.Id);
        Assert.Equal(headingLevel, actualOptions.HeadingLevel);
        Assert.Equal(rememberExpanded, actualOptions.RememberExpanded);
        Assert.Equal(hideAllSectionsText, actualOptions.HideAllSectionsText);
        Assert.Equal(hideSectionText, actualOptions.HideSectionText);
        Assert.Equal(hideSectionAriaLabelText, actualOptions.HideSectionAriaLabelText);
        Assert.Equal(showAllSectionsText, actualOptions.ShowAllSectionsText);
        Assert.Equal(showSectionText, actualOptions.ShowSectionText);
        Assert.Equal(showSectionAriaLabelText, actualOptions.ShowSectionAriaLabelText);
        Assert.Equal(classes, actualOptions.Classes);
        Assert.NotNull(actualOptions.Attributes);
        Assert.Collection(actualOptions.Attributes, kvp =>
        {
            Assert.Equal("data-foo", kvp.Key);
            Assert.Equal(dataFooAttrValue, kvp.Value);
        });
        Assert.NotNull(actualOptions.Items);
        Assert.Equal(2, actualOptions.Items.Count);

        var firstItem = actualOptions.Items.ElementAt(0);
        Assert.Equal(items[0].Expanded, firstItem.Expanded);
        Assert.NotNull(firstItem.Heading);
        Assert.Equal("First heading", firstItem.Heading.Html);
        Assert.NotNull(firstItem.Summary);
        Assert.Equal("First summary", firstItem.Summary.Html);
        Assert.NotNull(firstItem.Content);
        Assert.Equal("First content", firstItem.Content.Html);

        var secondItem = actualOptions.Items.ElementAt(1);
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
        var context = new TagHelperContext(
            tagName: "govuk-accordion",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-accordion",
            attributes: [],
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

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("The 'id' attribute must be specified.", ex.Message);
    }
}
