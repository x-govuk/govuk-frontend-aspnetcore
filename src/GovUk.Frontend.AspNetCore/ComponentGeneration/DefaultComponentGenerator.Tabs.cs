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

                ulTag.InnerHtml.AppendHtml(CreateTabListItem(options, item, index));
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

                tabsTag.InnerHtml.AppendHtml(CreateTabPanel(options, item, index));
                index++;
            }
        }

        return GenerateFromHtmlTagAsync(tabsTag);

        HtmlTag CreateTabListItem(TabsOptions options, TabsOptionsItem item, int index)
        {
            var tabPanelId = GetTabPanelId(options, item, index);

            var liTag = new HtmlTag("li", attrs => attrs
                .WithClasses("govuk-tabs__list-item", index == 1 ? "govuk-tabs__list-item--selected" : null));

            var aTag = new HtmlTag("a", attrs => attrs
                .WithClasses("govuk-tabs__tab")
                .With("href", $"#{tabPanelId}")
                .With(item.Attributes));

            aTag.InnerHtml.AppendHtml(item.Label ?? Microsoft.AspNetCore.Html.HtmlString.Empty);
            liTag.InnerHtml.AppendHtml(aTag);

            return liTag;
        }

        HtmlTag CreateTabPanel(TabsOptions options, TabsOptionsItem item, int index)
        {
            var tabPanelId = GetTabPanelId(options, item, index);

            var panelTag = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-tabs__panel", index > 1 ? "govuk-tabs__panel--hidden" : null)
                .With("id", tabPanelId)
                .With(item.Panel?.Attributes));

            if (item.Panel?.Html is not null && !item.Panel.Html.IsEmpty())
            {
                var rawPanelHtml = item.Panel.Html.Value.ToHtmlString(raw: true);
                panelTag.InnerHtml.AppendHtml(new Microsoft.AspNetCore.Html.HtmlString(rawPanelHtml));
            }
            else if (item.Panel?.Text is not null && !item.Panel.Text.IsEmpty())
            {
                var pTag = new HtmlTag("p", attrs => attrs
                    .WithClasses("govuk-body"));
                pTag.InnerHtml.AppendHtml(item.Panel.Text);
                panelTag.InnerHtml.AppendHtml(pTag);
            }

            return panelTag;
        }

        string GetTabPanelId(TabsOptions options, TabsOptionsItem item, int index)
        {
            if (item.Id is not null && !item.Id.IsEmpty())
            {
                return item.Id.Value.ToHtmlString(raw: true);
            }

            var idPrefix = options.IdPrefix?.ToHtmlString(raw: true) ?? string.Empty;
            if (!string.IsNullOrEmpty(idPrefix))
            {
                return $"{idPrefix}-{index}";
            }

            return $"{index}";
        }
    }
}
