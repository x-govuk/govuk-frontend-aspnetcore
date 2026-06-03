using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class NotificationBannerTitleTagHelperTests : TagHelperTestBase<NotificationBannerTitleTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsTitleOnContext()
    {
        // Arrange
        var notificationBannerContext = new NotificationBannerContext();

        var context = CreateTagHelperContext(contexts: notificationBannerContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Title");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new NotificationBannerTitleTagHelper()
        {
            HeadingLevel = 3,
            Id = "my-title"
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(notificationBannerContext.Title);
        Assert.Equal("Title", notificationBannerContext.Title?.Content?.ToHtmlString());
        Assert.Equal(3, notificationBannerContext.Title?.HeadingLevel);
        Assert.Equal("my-title", notificationBannerContext.Title?.Id);
    }
}
