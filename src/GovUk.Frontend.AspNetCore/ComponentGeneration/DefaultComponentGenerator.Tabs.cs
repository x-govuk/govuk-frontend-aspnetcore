namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual Task<GovUkComponent> GenerateTabsAsync(TabsOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var tabsTag = new HtmlTag("div", attrs => attrs
            .With("id", options.Id)
            .WithClasses("govuk-tabs", options.Classes)
            .With(options.Attributes)
            .With("data-module", "govuk-tabs"));

        var titleTag = new HtmlTag("h2", attrs => attrs
            .WithClasses("govuk-tabs__title"));
        titleTag.InnerHtml.AppendHtml(HtmlOrText(options.Title, null, fallback: "Contents"));
        tabsTag.InnerHtml.AppendHtml(titleTag);

        if (options.Items is not null && options.Items.Count > 0)
        {
            var ulTag = new HtmlTag("ul", attrs => attrs
                .WithClasses("govuk-tabs__list"));

            var index = 1;
            foreach (var item in options.Items)
            {
                if (item is null)
                {
                    continue;
                }

                ulTag.InnerHtml.AppendHtml(CreateTabListItem(item, index));
                index++;
            }

            tabsTag.InnerHtml.AppendHtml(ulTag);

            index = 1;
            foreach (var item in options.Items)
            {
                if (item is null)
                {
                    continue;
                }

                tabsTag.InnerHtml.AppendHtml(CreateTabPanel(item, index));
                index++;
            }
        }

        return GenerateFromHtmlTagAsync(tabsTag);

        HtmlTag CreateTabListItem(TabsOptionsItem item, int index)
        {
            var tabPanelId = GetTabPanelId(item, index);

            var liTag = new HtmlTag("li", attrs => attrs
                .WithClasses("govuk-tabs__list-item", index == 1 ? "govuk-tabs__list-item--selected" : null));

            var aTag = new HtmlTag("a", attrs => attrs
                .WithClasses("govuk-tabs__tab")
                .With("href", new TemplateString($"#{tabPanelId}"))
                .With(item.Attributes));

            aTag.InnerHtml.AppendHtml(item.Label ?? TemplateString.Empty);
            liTag.InnerHtml.AppendHtml(aTag);

            return liTag;
        }

        HtmlTag CreateTabPanel(TabsOptionsItem item, int index)
        {
            var tabPanelId = GetTabPanelId(item, index);

            var panelTag = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-tabs__panel", index > 1 ? "govuk-tabs__panel--hidden" : null)
                .With("id", tabPanelId)
                .With(item.Panel?.Attributes));

            if (!(item.Panel?.Html).IsEmpty())
            {
                panelTag.InnerHtml.AppendHtml(item.Panel.Html.GetRawHtml());
            }
            else if (!(item.Panel?.Text).IsEmpty())
            {
                var pTag = new HtmlTag("p", attrs => attrs
                    .WithClasses("govuk-body"));
                pTag.InnerHtml.AppendHtml(item.Panel.Text);
                panelTag.InnerHtml.AppendHtml(pTag);
            }

            return panelTag;
        }

        TemplateString GetTabPanelId(TabsOptionsItem item, int index)
        {
            if (!item.Id.IsEmpty())
            {
                return item.Id;
            }

            var idPrefix = options.IdPrefix;

            return !idPrefix.IsEmpty() ? new TemplateString($"{idPrefix}-{index}") : $"{index}";
        }
    }
}
