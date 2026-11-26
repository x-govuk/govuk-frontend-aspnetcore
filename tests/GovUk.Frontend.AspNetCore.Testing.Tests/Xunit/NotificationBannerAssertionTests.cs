using AngleSharp.Html.Dom;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.Testing.Tests.Xunit;

public class NotificationBannerAssertionTests
{
    private const string SuccessHtml =
        "<h3 class=\"govuk-notification-banner__heading\">Training outcome recorded</h3>" +
        "<p class=\"govuk-body\">Contact us if you think there’s a problem.</p>";

    [Theory]
    [InlineData("Training outcome recorded")]
    [InlineData("Contact us if you think there’s a problem.")]
    [InlineData("Training outcome recorded Contact us if you think there’s a problem.")]
    public async Task NotificationBannerContains_WholeLineOrContent_ReturnsBanner(string expectedText)
    {
        var doc = await RenderAsync(new NotificationBannerOptions { Type = "success", Html = new HtmlString(SuccessHtml) });

        var banner = Assert.NotificationBannerContains(doc, expectedText);

        Assert.True(banner.IsSuccess);
    }

    [Fact]
    public async Task NotificationBannerContains_PartOfALine_FailsWithLinesAndHtml()
    {
        var doc = await RenderAsync(new NotificationBannerOptions { Type = "success", Html = new HtmlString(SuccessHtml) });

        var ex = AssertFailure.Throws(
            """
            Assert.NotificationBannerContains() Failure: Text not found
            Expected: "Training outcome"
            Actual:   ["Training outcome recorded", "Contact us if you think there’s a problem."]
            """,
            () => Assert.NotificationBannerContains(doc, "Training outcome"),
            htmlOf: doc.NotificationBanner!.Element);

        AssertFailure.StackTraceStartsIn(nameof(NotificationBannerContains_PartOfALine_FailsWithLinesAndHtml), ex);
    }

    [Fact]
    public async Task NotificationBannerContains_MoreThanOneBanner_ChecksEach()
    {
        var doc = await RenderAsync(
            new NotificationBannerOptions { Text = "You have 7 days left to send your application." },
            new NotificationBannerOptions { Type = "success", Html = new HtmlString(SuccessHtml) });

        Assert.False(Assert.NotificationBannerContains(doc, "You have 7 days left to send your application.").IsSuccess);
        Assert.True(Assert.NotificationBannerContains(doc, "Training outcome recorded").IsSuccess);

        AssertFailure.Throws(
            """
            Assert.NotificationBannerContains() Failure: Text not found
            Expected: "Application sent"
            Actual:   ["You have 7 days left to send your application.", "Training outcome recorded", "Contact us if you think there’s a problem."]
            """,
            () => Assert.NotificationBannerContains(doc, "Application sent"));
    }

    [Fact]
    public void NotificationBannerContains_NoBanner_Fails()
    {
        var doc = Render.Document("<p>Nothing here</p>");

        AssertFailure.Throws(
            """
            Assert.NotificationBannerContains() Failure: Notification banner not found
            Expected: a notification banner containing "Training outcome recorded"
            Actual:   no notification banner
            """,
            () => Assert.NotificationBannerContains(doc, "Training outcome recorded"));
    }

    [Fact]
    public async Task SuccessNotificationBannerContains_SuccessBanner_ReturnsBanner()
    {
        var doc = await RenderAsync(new NotificationBannerOptions { Type = "success", Html = new HtmlString(SuccessHtml) });

        var banner = Assert.SuccessNotificationBannerContains(doc, "Training outcome recorded");

        Assert.Equal("Success", banner.Title);
    }

    [Fact]
    public async Task SuccessNotificationBannerContains_NotSuccessVariant_FailsWithTitleAndHtml()
    {
        var doc = await RenderAsync(new NotificationBannerOptions { Text = "Training outcome recorded" });

        var ex = AssertFailure.Throws(
            """
            Assert.SuccessNotificationBannerContains() Failure: Notification banner is not the success variant
            Expected: a success notification banner containing "Training outcome recorded"
            Actual:   a notification banner with the title "Important"
            """,
            () => Assert.SuccessNotificationBannerContains(doc, "Training outcome recorded"),
            htmlOf: doc.NotificationBanner!.Element);

        AssertFailure.StackTraceStartsIn(nameof(SuccessNotificationBannerContains_NotSuccessVariant_FailsWithTitleAndHtml), ex);
    }

    [Fact]
    public void SuccessNotificationBannerContains_NoBanner_Fails()
    {
        var doc = Render.Document("<p>Nothing here</p>");

        AssertFailure.Throws(
            """
            Assert.SuccessNotificationBannerContains() Failure: Notification banner not found
            Expected: a success notification banner containing "Training outcome recorded"
            Actual:   no notification banner
            """,
            () => Assert.SuccessNotificationBannerContains(doc, "Training outcome recorded"));
    }

    private static Task<IHtmlDocument> RenderAsync(params NotificationBannerOptions[] banners) =>
        Render.DocumentAsync(banners
            .Select<NotificationBannerOptions, Func<DefaultComponentGenerator, ValueTask<GovUkComponent>>>(b => g => g.GenerateNotificationBannerAsync(b))
            .ToArray());
}
