using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class NotificationBannerTagHelperTests : TagHelperTestBase<NotificationBannerTagHelper>
{
    [Fact]
    public async Task ProcessAsync_DefaultType_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var content = "The message.";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<NotificationBannerOptions>(nameof(IComponentGenerator.GenerateNotificationBannerAsync));

        var tagHelper = new NotificationBannerTagHelper(componentGenerator);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(content, actualOptions.Html);
        Assert.Null(actualOptions.Type);
        Assert.Null(actualOptions.Role);
        Assert.Null(actualOptions.DisableAutoFocus);
        Assert.Null(actualOptions.TitleId);
        Assert.Null(actualOptions.TitleHeadingLevel);
        Assert.Null(actualOptions.TitleHtml);
    }

    [Fact]
    public async Task ProcessAsync_SuccessType_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var content = "The message.";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<NotificationBannerOptions>(nameof(IComponentGenerator.GenerateNotificationBannerAsync));

        var tagHelper = new NotificationBannerTagHelper(componentGenerator)
        {
            Type = NotificationBannerType.Success
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(content, actualOptions.Html);
        Assert.Equal("success", actualOptions.Type);
        Assert.Null(actualOptions.Role);
        Assert.Null(actualOptions.DisableAutoFocus);
    }

    [Fact]
    public async Task ProcessAsync_WithDisableAutoFocusSpecified_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var content = "The message.";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<NotificationBannerOptions>(nameof(IComponentGenerator.GenerateNotificationBannerAsync));

        var tagHelper = new NotificationBannerTagHelper(componentGenerator)
        {
            DisableAutoFocus = true,
            Type = NotificationBannerType.Success
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(content, actualOptions.Html);
        Assert.Equal("success", actualOptions.Type);
        Assert.True(actualOptions.DisableAutoFocus);
    }

    [Fact]
    public async Task ProcessAsync_WithRoleSpecified_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var content = "The message.";
        var role = "custom-role";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<NotificationBannerOptions>(nameof(IComponentGenerator.GenerateNotificationBannerAsync));

        var tagHelper = new NotificationBannerTagHelper(componentGenerator)
        {
            Role = role
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(content, actualOptions.Html);
        Assert.Equal(role, actualOptions.Role);
    }

    [Fact]
    public async Task ProcessAsync_WithTitle_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var content = "The message.";
        var titleId = "title-id";
        var titleHeadingLevel = 4;
        var titleContent = new HtmlString("Title");

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var notificationBannerContext = context.GetContextItem<NotificationBannerContext>();
                notificationBannerContext.SetTitle(id: titleId, headingLevel: titleHeadingLevel, content: new TemplateString(titleContent));

                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<NotificationBannerOptions>(nameof(IComponentGenerator.GenerateNotificationBannerAsync));

        var tagHelper = new NotificationBannerTagHelper(componentGenerator);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(content, actualOptions.Html);
        Assert.Equal(titleId, actualOptions.TitleId);
        Assert.Equal(titleHeadingLevel, actualOptions.TitleHeadingLevel);
        Assert.Equal(titleContent.Value, actualOptions.TitleHtml);
    }
}
