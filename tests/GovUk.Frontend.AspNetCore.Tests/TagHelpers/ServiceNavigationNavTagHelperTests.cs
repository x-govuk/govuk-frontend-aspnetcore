using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class ServiceNavigationNavTagHelperTests() : TagHelperTestBase(ServiceNavigationNavTagHelper.TagName)
{
    [Fact]
    public async Task ProcessAsync_SetsNavOnContext()
    {
        // Arrange
        var ariaLabel = "aria-label";
        var menuButtonText = "Menu button text";
        var menuButtonLabel = "Menu button label";
        var label = "Navigation label";
        var id = "navigation-id";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();
        var navStartSlotContent = "Start nav";
        var navEndSlotContent = "End nav";

        var item = new ServiceNavigationOptionsNavigationItem();

        var serviceNavigationContext = new ServiceNavigationContext();

        var context = CreateTagHelperContext(
            className: className,
            attributes: attributes,
            contexts: serviceNavigationContext);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var serviceNavigationNavContext = context.GetContextItem<ServiceNavigationNavContext>();

                serviceNavigationNavContext.NavigationStartSlot = new(navStartSlotContent, ServiceNavigationNavStartTagHelper.TagName);

                serviceNavigationNavContext.Items.Add(item);

                serviceNavigationNavContext.NavigationEndSlot = new(navEndSlotContent, ServiceNavigationNavEndTagHelper.TagName);

                TagHelperContent content = new DefaultTagHelperContent();
                return Task.FromResult(content);
            });

        var tagHelper = new ServiceNavigationNavTagHelper()
        {
            AriaLabel = ariaLabel,
            MenuButtonText = menuButtonText,
            MenuButtonLabel = menuButtonLabel,
            Label = label,
            Id = id
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(serviceNavigationContext.Nav);
        Assert.Equal(ariaLabel, serviceNavigationContext.Nav.AriaLabel);
        Assert.Equal(menuButtonText, serviceNavigationContext.Nav.MenuButtonText);
        Assert.Equal(menuButtonLabel, serviceNavigationContext.Nav.MenuButtonLabel);
        Assert.Equal(label, serviceNavigationContext.Nav.Label);
        Assert.Equal(id, serviceNavigationContext.Nav.Id);
        AssertContainsAttributes(attributes, serviceNavigationContext.Nav.Attributes);
        Assert.Equal(navStartSlotContent, serviceNavigationContext.Nav.NavigationStartSlot?.Html);
        Assert.Equal(navEndSlotContent, serviceNavigationContext.Nav.NavigationEndSlot?.Html);
    }
}
