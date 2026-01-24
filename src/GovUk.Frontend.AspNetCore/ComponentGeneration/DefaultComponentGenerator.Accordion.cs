namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual ValueTask<GovUkComponent> GenerateAccordionAsync(AccordionOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.Id is null)
        {
            throw new ArgumentException("Id is required.", nameof(options));
        }

        var headingLevel = options.HeadingLevel ?? 2;
        if (headingLevel is < 1 or > 6)
        {
            throw new ArgumentOutOfRangeException(
                nameof(options),
                "HeadingLevel must be between 1 and 6.");
        }

        var accordionTag = new HtmlTag("div", attrs => attrs
            .WithClasses("govuk-accordion", options.Classes)
            .With("data-module", "govuk-accordion")
            .With("id", options.Id)
            .With("data-i18n.hide-all-sections", options.HideAllSectionsText)
            .With("data-i18n.hide-section", options.HideSectionText)
            .With("data-i18n.hide-section-aria-label", options.HideSectionAriaLabelText)
            .With("data-i18n.show-all-sections", options.ShowAllSectionsText)
            .With("data-i18n.show-section", options.ShowSectionText)
            .With("data-i18n.show-section-aria-label", options.ShowSectionAriaLabelText)
            .With("data-remember-expanded", options.RememberExpanded is false ? "false" : null)
            .With(options.Attributes));

        if (options.Items is not null)
        {
            var index = 1;
            foreach (var item in options.Items)
            {
                if (item is null)
                {
                    continue;
                }

                var sectionTag = new HtmlTag("div", attrs => attrs
                    .WithClasses("govuk-accordion__section", item.Expanded is true ? "govuk-accordion__section--expanded" : null));

                var headerTag = new HtmlTag("div", attrs => attrs
                    .WithClasses("govuk-accordion__section-header"));

                var headingTag = new HtmlTag($"h{headingLevel}", attrs => attrs
                    .WithClasses("govuk-accordion__section-heading"));

                var headingButtonTag = new HtmlTag("span", attrs => attrs
                    .WithClasses("govuk-accordion__section-button")
                    .With("id", new($"{options.Id}-heading-{index}")));

                headingButtonTag.InnerHtml.AppendHtml(HtmlOrText(item.Heading?.Html, item.Heading?.Text));
                headingTag.InnerHtml.AppendHtml(headingButtonTag);
                headerTag.InnerHtml.AppendHtml(headingTag);

                if (item.Summary?.Html is not null || item.Summary?.Text is not null)
                {
                    var summaryTag = new HtmlTag("div", attrs => attrs
                        .WithClasses("govuk-accordion__section-summary", "govuk-body")
                        .With("id", new($"{options.Id}-summary-{index}")));

                    summaryTag.InnerHtml.AppendHtml(HtmlOrText(item.Summary?.Html, item.Summary?.Text));
                    headerTag.InnerHtml.AppendHtml(summaryTag);
                }

                sectionTag.InnerHtml.AppendHtml(headerTag);

                var contentTag = new HtmlTag("div", attrs => attrs
                    .WithClasses("govuk-accordion__section-content")
                    .With("id", new($"{options.Id}-content-{index}")));

                if (item.Content?.Html is { } contentHtml)
                {
                    contentTag.InnerHtml.AppendHtml(contentHtml.GetRawHtml());
                }
                else if (item.Content?.Text is { } contentText)
                {
                    var pTag = new HtmlTag("p", attrs => attrs.WithClasses("govuk-body"));
                    pTag.InnerHtml.AppendHtml(contentText);
                    contentTag.InnerHtml.AppendHtml(pTag);
                }

                sectionTag.InnerHtml.AppendHtml(contentTag);
                accordionTag.InnerHtml.AppendHtml(sectionTag);

                index++;
            }
        }

        return GenerateFromHtmlTagAsync(accordionTag);
    }
}
