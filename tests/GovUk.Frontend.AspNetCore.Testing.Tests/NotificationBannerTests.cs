using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.Testing.Tests;

public class NotificationBannerTests
{
    [Fact]
    public async Task NotificationBanner_WithText_ReadsTitleAndHeading()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateNotificationBannerAsync(new NotificationBannerOptions
        {
            Text = "You have 7 days left to send your application."
        }));

        var banner = doc.NotificationBanner;

        Assert.NotNull(banner);
        Assert.Equal("Important", banner.Title);
        Assert.Equal("You have 7 days left to send your application.", banner.Heading);
        Assert.Equal("You have 7 days left to send your application.", banner.Content);
        Assert.False(banner.IsSuccess);
    }

    [Fact]
    public async Task NotificationBanner_SuccessWithHtml_ReadsHeadingAndContent()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateNotificationBannerAsync(new NotificationBannerOptions
        {
            Type = "success",
            Html = new HtmlString(
                "<h3 class=\"govuk-notification-banner__heading\">Training outcome recorded</h3>" +
                "<p class=\"govuk-body\">Contact <a class=\"govuk-notification-banner__link\" href=\"#\">example@department.gov.uk</a> if you think there’s a problem.</p>")
        }));

        var banner = doc.NotificationBanner!;

        Assert.True(banner.IsSuccess);
        Assert.Equal("Success", banner.Title);
        Assert.Equal("Training outcome recorded", banner.Heading);
        Assert.Equal(
            ["Training outcome recorded", "Contact example@department.gov.uk if you think there’s a problem."],
            banner.ContentLines);
        Assert.Equal(
            "Training outcome recorded Contact example@department.gov.uk if you think there’s a problem.",
            banner.Content);
    }

    [Fact]
    public async Task NotificationBanner_HtmlWithoutHeading_HasNullHeading()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateNotificationBannerAsync(new NotificationBannerOptions
        {
            Html = new HtmlString("<p class=\"govuk-body\">Some content</p>")
        }));

        var banner = doc.NotificationBanner!;

        Assert.Null(banner.Heading);
        Assert.Equal("Some content", banner.Content);
    }

    [Fact]
    public async Task NotificationBanners_MoreThanOne_SingularThrowsAndPluralReturnsAll()
    {
        var doc = await Render.DocumentAsync(
            g => g.GenerateNotificationBannerAsync(new NotificationBannerOptions { Text = "First" }),
            g => g.GenerateNotificationBannerAsync(new NotificationBannerOptions { Text = "Second", Type = "success" }));

        Assert.Equal(["First", "Second"], doc.NotificationBanners.Select(b => b.Heading));

        var ex = Assert.Throws<ComponentLookupException>(() => doc.NotificationBanner);
        Assert.Equal("More than one notification banner found.", ex.Failure.Summary);
        Assert.Equal("2 notification banners", ex.Failure.Found);
    }

    [Fact]
    public void NotificationBanner_NoneOnPage_ReturnsNull()
    {
        var doc = Render.Document("<p>Nothing here</p>");

        Assert.Null(doc.NotificationBanner);
        Assert.Empty(doc.NotificationBanners);
    }
}
