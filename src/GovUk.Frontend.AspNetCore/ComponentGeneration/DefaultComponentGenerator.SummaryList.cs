namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual ValueTask<GovUkComponent> GenerateSummaryListAsync(SummaryListOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        HtmlTag BuildRowActionLink(SummaryListOptionsRowActionsItem action, SummaryListOptionsCardTitle? cardTitle)
        {
            var aTag = new HtmlTag("a", attrs => attrs
                .WithClasses("govuk-link", action.Classes)
                .With("href", action.Href)
                .With(action.Attributes));

            aTag.InnerHtml.AppendHtml(HtmlOrText(action.Html, action.Text));

            if (!action.VisuallyHiddenText.IsEmpty() || cardTitle is not null)
            {
                var spanTag = new HtmlTag("span", attrs => attrs
                    .WithClasses("govuk-visually-hidden"));

                spanTag.InnerHtml.Append(" ");

                if (action.VisuallyHiddenText?.IsEmpty() == false)
                {
                    spanTag.InnerHtml.AppendHtml(action.VisuallyHiddenText);
                }

                if (cardTitle is not null)
                {
                    spanTag.InnerHtml.Append(" (");
                    spanTag.InnerHtml.AppendHtml(HtmlOrText(cardTitle.Html, cardTitle.Text));
                    spanTag.InnerHtml.Append(")");
                }

                aTag.InnerHtml.AppendHtml(spanTag);
            }

            return aTag;
        }

        HtmlTag BuildCardActionLink(SummaryListOptionsCardActionsItem action, SummaryListOptionsCardTitle? cardTitle)
        {
            var aTag = new HtmlTag("a", attrs => attrs
                .WithClasses("govuk-link", action.Classes)
                .With("href", action.Href)
                .With(action.Attributes));

            aTag.InnerHtml.AppendHtml(HtmlOrText(action.Html, action.Text));

            if (action.VisuallyHiddenText?.IsEmpty() == false || cardTitle is not null)
            {
                var spanTag = new HtmlTag("span", attrs => attrs
                    .WithClasses("govuk-visually-hidden"));

                spanTag.InnerHtml.Append(" ");

                if (!action.VisuallyHiddenText.IsEmpty())
                {
                    spanTag.InnerHtml.AppendHtml(action.VisuallyHiddenText);
                }

                if (cardTitle is not null)
                {
                    spanTag.InnerHtml.Append(" (");
                    spanTag.InnerHtml.AppendHtml(HtmlOrText(cardTitle.Html, cardTitle.Text));
                    spanTag.InnerHtml.Append(")");
                }

                aTag.InnerHtml.AppendHtml(spanTag);
            }

            return aTag;
        }

        HtmlTag BuildSummaryCard(SummaryListOptionsCard card, HtmlTag summaryList)
        {
            var headingLevel = card.Title?.HeadingLevel ?? 2;

            var cardTag = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-summary-card", card.Classes)
                .With(card.Attributes));

            var titleWrapperTag = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-summary-card__title-wrapper")
                .With(card.Title?.Attributes));

            if (card.Title is not null)
            {
                var titleTag = new HtmlTag($"h{headingLevel}", attrs => attrs
                    .WithClasses("govuk-summary-card__title", card.Title.Classes));

                titleTag.InnerHtml.AppendHtml(HtmlOrText(card.Title.Html, card.Title.Text));
                titleWrapperTag.InnerHtml.AppendHtml(titleTag);
            }

            var actionItems = card.Actions?.Items?.Where(i => i is not null).ToArray() ?? [];

            if (actionItems.Length > 0)
            {
                if (actionItems.Length == 1)
                {
                    var actionsTag = new HtmlTag("div", attrs => attrs
                        .WithClasses("govuk-summary-card__actions", card.Actions!.Classes)
                        .With(card.Actions!.Attributes));

                    var actionLink = BuildCardActionLink(actionItems.First()!, card.Title);
                    actionsTag.InnerHtml.AppendHtml(actionLink);
                    titleWrapperTag.InnerHtml.AppendHtml(actionsTag);
                }
                else
                {
                    var actionsTag = new HtmlTag("ul", attrs => attrs
                        .WithClasses("govuk-summary-card__actions", card.Actions!.Classes)
                        .With(card.Actions.Attributes));

                    foreach (var action in actionItems)
                    {
                        var liTag = new HtmlTag("li", attrs => attrs
                            .WithClasses("govuk-summary-card__action"));

                        var actionLink = BuildCardActionLink(action!, card.Title);
                        liTag.InnerHtml.AppendHtml(actionLink);
                        actionsTag.InnerHtml.AppendHtml(liTag);
                    }

                    titleWrapperTag.InnerHtml.AppendHtml(actionsTag);
                }
            }

            cardTag.InnerHtml.AppendHtml(titleWrapperTag);

            var contentTag = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-summary-card__content"));

            contentTag.InnerHtml.AppendHtml(summaryList);
            cardTag.InnerHtml.AppendHtml(contentTag);

            return cardTag;
        }

        var anyRowHasActions = options.Rows?.Any(row => row?.Actions?.Items?.Count > 0) is true;

        var dlTag = new HtmlTag("dl", attrs => attrs
            .WithClasses("govuk-summary-list", options.Classes)
            .With(options.Attributes));

        if (options.Rows is not null)
        {
            foreach (var row in options.Rows)
            {
                if (row is null)
                {
                    continue;
                }

                var rowHasActions = row.Actions?.Items?.Count > 0;

                var rowTag = new HtmlTag("div", attrs => attrs
                    .WithClasses(
                        "govuk-summary-list__row",
                        anyRowHasActions && !rowHasActions ? "govuk-summary-list__row--no-actions" : null,
                        row.Classes)
                    .With(row.Attributes));

                var keyTag = new HtmlTag("dt", attrs => attrs
                    .WithClasses("govuk-summary-list__key", row.Key?.Classes)
                    .With(row.Key?.Attributes));

                keyTag.InnerHtml.AppendHtml(HtmlOrText(row.Key?.Html, row.Key?.Text));
                rowTag.InnerHtml.AppendHtml(keyTag);

                var valueTag = new HtmlTag("dd", attrs => attrs
                    .WithClasses("govuk-summary-list__value", row.Value?.Classes)
                    .With(row.Value?.Attributes));

                valueTag.InnerHtml.AppendHtml(HtmlOrText(row.Value?.Html, row.Value?.Text));
                rowTag.InnerHtml.AppendHtml(valueTag);

                if (rowHasActions)
                {
                    var actionsTag = new HtmlTag("dd", attrs => attrs
                        .WithClasses("govuk-summary-list__actions", row.Actions!.Classes)
                        .With(row.Actions.Attributes));

                    if (row.Actions!.Items!.Count == 1)
                    {
                        var actionLink = BuildRowActionLink(row.Actions.Items.First()!, options.Card?.Title);
                        actionsTag.InnerHtml.AppendHtml(actionLink);
                    }
                    else
                    {
                        var ulTag = new HtmlTag("ul", attrs => attrs
                            .WithClasses("govuk-summary-list__actions-list"));

                        foreach (var action in row.Actions.Items)
                        {
                            if (action is null)
                            {
                                continue;
                            }

                            var liTag = new HtmlTag("li", attrs => attrs
                                .WithClasses("govuk-summary-list__actions-list-item"));

                            var actionLink = BuildRowActionLink(action, options.Card?.Title);
                            liTag.InnerHtml.AppendHtml(actionLink);
                            ulTag.InnerHtml.AppendHtml(liTag);
                        }

                        actionsTag.InnerHtml.AppendHtml(ulTag);
                    }

                    rowTag.InnerHtml.AppendHtml(actionsTag);
                }

                dlTag.InnerHtml.AppendHtml(rowTag);
            }
        }

        var resultTag = options.Card is not null
            ? BuildSummaryCard(options.Card, dlTag)
            : dlTag;

        return GenerateFromHtmlTagAsync(resultTag);
    }
}
