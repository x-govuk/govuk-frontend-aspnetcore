using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.HtmlGeneration;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.ConformanceTests;

public partial class ComponentTests
{
    [Theory]
    [ComponentFixtureData("notification-banner", typeof(OptionsJson.NotificationBanner))]
    public void NotificationBanner(ComponentTestCaseData<OptionsJson.NotificationBanner> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) =>
            {
                var type = options.Type == "success" ?
                    NotificationBannerType.Success :
                    NotificationBannerType.Default;

                var titleContent = TextOrHtmlHelper.GetHtmlContent(options.TitleText, options.TitleHtml);

                // The 'text' option gets wrapped in a <p>
                var content = options.Html is not null ?
                    new HtmlString(options.Html) :
                    options.Text is not null ?
                    new HtmlString(
                        "<p class=\"govuk-notification-banner__heading\">" +
                        HtmlEncoder.Default.Encode(options.Text) +
                        "</p>") :
                    null;

                var attributes = options.Attributes.ToAttributesDictionary()
                    .MergeAttribute("class", options.Classes);

                return generator.GenerateNotificationBanner(
                        type,
                        options.Role,
                        options.DisableAutoFocus,
                        options.TitleId,
                        options.TitleHeadingLevel ?? ComponentGenerator.NotificationBannerDefaultTitleHeadingLevel,
                        titleContent,
                        content,
                        attributes)
                    .ToHtmlString();
            });
}
