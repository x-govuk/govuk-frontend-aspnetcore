using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class NotificationBannerContextTests
{
    [Fact]
    public void SetTitle_AlreadyGotTitle_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new NotificationBannerContext();
        context.SetTitle("id", headingLevel: 4, content: new TemplateString("Title"));

        // Act
        var ex = Record.Exception(() => context.SetTitle("id", headingLevel: 4, content: new TemplateString("Title")));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"Only one <{NotificationBannerTitleTagHelper.TagName}> or <{NotificationBannerTitleTagHelper.ShortTagName}> " +
                $"element is permitted within each <{NotificationBannerTagHelper.TagName}>.",
            ex.Message);
    }
}
