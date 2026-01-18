namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual async Task<GovUkComponent> GenerateTaskListAsync(TaskListOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var idPrefix = options.IdPrefix?.ToHtmlString() ?? "task-list";

        var ulTag = new HtmlTag("ul", attrs => attrs
            .WithClasses("govuk-task-list", options.Classes)
            .With(options.Attributes));

        if (options.Items is not null)
        {
            var index = 1;
            foreach (var item in options.Items)
            {
                ulTag.InnerHtml.AppendHtml(await CreateTaskListItemAsync(item, index, idPrefix));
                index++;
            }
        }

        return new HtmlTagGovUkComponent(ulTag);

        async Task<HtmlTag> CreateTaskListItemAsync(TaskListOptionsItem item, int index, string idPrefix)
        {
            var hintId = $"{idPrefix}-{index}-hint";
            var statusId = $"{idPrefix}-{index}-status";

            var liTag = new HtmlTag("li", attrs =>
            {
                attrs.WithClasses("govuk-task-list__item");

                if (item.Href is not null)
                {
                    attrs.WithClasses("govuk-task-list__item--with-link");
                }

                if (item.Classes is not null)
                {
                    attrs.WithClasses(item.Classes);
                }
            });

            // Create name and hint container
            var nameAndHintDiv = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-task-list__name-and-hint"));

            // Create title (either as link or div)
            if (item.Href is not null)
            {
                var ariaDescribedBy = item.Hint is not null ? $"{hintId} {statusId}" : statusId;

                var aTag = new HtmlTag("a", attrs => attrs
                    .WithClasses("govuk-link", "govuk-task-list__link", item.Title?.Classes)
                    .With("href", item.Href)
                    .With("aria-describedby", ariaDescribedBy));

                aTag.InnerHtml.AppendHtml(HtmlOrText(item.Title?.Html, item.Title?.Text));
                nameAndHintDiv.InnerHtml.AppendHtml(aTag);
            }
            else
            {
                var divTag = new HtmlTag("div", attrs =>
                {
                    if (item.Title?.Classes is not null)
                    {
                        attrs.WithClasses(item.Title.Classes);
                    }
                });

                divTag.InnerHtml.AppendHtml(HtmlOrText(item.Title?.Html, item.Title?.Text));
                nameAndHintDiv.InnerHtml.AppendHtml(divTag);
            }

            // Add hint if present
            if (item.Hint is not null)
            {
                var hintDiv = new HtmlTag("div", attrs => attrs
                    .With("id", hintId)
                    .WithClasses("govuk-task-list__hint"));

                hintDiv.InnerHtml.AppendHtml(HtmlOrText(item.Hint.Html, item.Hint.Text));
                nameAndHintDiv.InnerHtml.AppendHtml(hintDiv);
            }

            liTag.InnerHtml.AppendHtml(nameAndHintDiv);

            // Create status div
            var statusDiv = new HtmlTag("div", attrs => attrs
                .With("id", statusId)
                .WithClasses("govuk-task-list__status", item.Status?.Classes));

            if (item.Status?.Tag is not null)
            {
                var tagComponent = await GenerateTagAsync(item.Status.Tag);
                statusDiv.InnerHtml.AppendHtml(tagComponent.GetHtml());
            }
            else if (item.Status is not null)
            {
                statusDiv.InnerHtml.AppendHtml(HtmlOrText(item.Status.Html, item.Status.Text));
            }

            liTag.InnerHtml.AppendHtml(statusDiv);

            return liTag;
        }
    }
}
