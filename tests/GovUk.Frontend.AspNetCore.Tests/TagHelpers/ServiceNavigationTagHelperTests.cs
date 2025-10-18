using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class ServiceNavigationTagHelperTests : TagHelperTestBase<ServiceNavigationTagHelper>
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var serviceName = "Service";
        var serviceUrl = "#";
        var ariaLabel = "aria-label";
        var menuButtonText = "Menu button text";
        var menuButtonLabel = "Menu button label";
        var navigationLabel = "Navigation label";
        var navigationId = "navigation-id";
        var navigationClassName = CreateDummyClassName();
        var navigationAttributes = CreateDummyDataAttributes();
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();
        var startSlotContent = "Start";
        var endSlotContent = "End";
        var navStartSlotContent = "Start nav";
        var navEndSlotContent = "End nav";

        var item = new ServiceNavigationOptionsNavigationItem();

        var context = CreateTagHelperContext(className: className, attributes: attributes);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var serviceNavigationContext = context.GetContextItem<ServiceNavigationContext>();

                var attributes = new AttributeCollection(navigationAttributes)
                {
                    { "class", navigationClassName }
                };

                serviceNavigationContext.StartSlot = new(startSlotContent, ServiceNavigationStartTagHelper.TagName);

                serviceNavigationContext.Nav = new ServiceNavigationNavContext()
                {
                    TagName = ServiceNavigationNavTagHelper.TagName,
                    AriaLabel = ariaLabel,
                    MenuButtonText = menuButtonText,
                    MenuButtonLabel = menuButtonLabel,
                    Label = navigationLabel,
                    Id = navigationId,
                    Attributes = attributes,
                    FirstItemTagName = ServiceNavigationNavItemTagHelper.TagName,
                    NavigationStartSlot = new(navStartSlotContent, ServiceNavigationNavStartTagHelper.TagName),
                    NavigationEndSlot = new(navEndSlotContent, ServiceNavigationNavEndTagHelper.TagName)
                };

                serviceNavigationContext.Nav.Items.Add(item);

                serviceNavigationContext.EndSlot = new(endSlotContent, ServiceNavigationEndTagHelper.TagName);

                TagHelperContent content = new DefaultTagHelperContent();
                return Task.FromResult(content);
            });

        var (componentGenerator, getActualOptions) =
            CreateComponentGenerator<ServiceNavigationOptions>(nameof(IComponentGenerator.GenerateServiceNavigationAsync));

        var tagHelper = new ServiceNavigationTagHelper(componentGenerator, HtmlEncoder.Default)
        {
            ServiceName = serviceName,
            ServiceUrl = serviceUrl
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions);
        Assert.Equal(className, actualOptions.Classes);
        AssertContainsAttributes(attributes, actualOptions.Attributes);
        Assert.Equal(ariaLabel, actualOptions.AriaLabel);
        Assert.Equal(menuButtonText, actualOptions.MenuButtonText);
        Assert.Equal(menuButtonLabel, actualOptions.MenuButtonLabel);
        Assert.Equal(navigationLabel, actualOptions.NavigationLabel);
        Assert.Equal(navigationId, actualOptions.NavigationId);
        Assert.Equal(navigationClassName, actualOptions.NavigationClasses);
        AssertContainsAttributes(navigationAttributes, actualOptions.NavigationAttributes);
        Assert.Equal(startSlotContent, actualOptions.Slots?.Start);
        Assert.Equal(endSlotContent, actualOptions.Slots?.End);
        Assert.Equal(navStartSlotContent, actualOptions.Slots?.NavigationStart);
        Assert.Equal(navEndSlotContent, actualOptions.Slots?.NavigationEnd);
        Assert.NotNull(actualOptions.Navigation);
        Assert.Collection(actualOptions.Navigation, i => Assert.Same(item, i));
    }
}
