using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual async Task<GovUkComponent> GenerateCheckboxesAsync(CheckboxesOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var hasFieldset = options.Fieldset is not null;
        var idPrefix = options.IdPrefix ?? options.Name;

        var describedByParts = new List<TemplateString>();
        if (options.Fieldset?.DescribedBy is var describedBy && !describedBy.IsEmpty())
        {
            describedByParts.Add(describedBy);
        }
        else if (options.DescribedBy is var describedByOption && !describedByOption.IsEmpty())
        {
            describedByParts.Add(describedByOption);
        }

        var formGroupDiv = new HtmlTag("div", attrs => attrs
            .WithClasses("govuk-form-group", options.ErrorMessage is not null ? "govuk-form-group--error" : null, options.FormGroup?.Classes)
            .With(options.FormGroup?.Attributes));

        if (hasFieldset)
        {
            var innerContent = await BuildInnerContentAsync();
            var fieldsetComponent = await GenerateFieldsetAsync(new FieldsetOptions
            {
                DescribedBy = describedByParts.Count > 0 ? TemplateString.Join(" ", describedByParts) : null,
                Classes = options.Fieldset!.Classes,
                Attributes = options.Fieldset.Attributes,
                Legend = options.Fieldset.Legend,
                Html = innerContent.ToTemplateString()
            });

            formGroupDiv.InnerHtml.AppendHtml(fieldsetComponent);
        }
        else
        {
            var innerContent = await BuildInnerContentAsync();
            formGroupDiv.InnerHtml.AppendHtml(innerContent);
        }

        return await GenerateFromHtmlTagAsync(formGroupDiv);

        async Task<IHtmlContent> BuildInnerContentAsync()
        {
            var innerHtmlBuilder = new HtmlContentBuilder();

            if (options.Hint is not null)
            {
                var hintId = new TemplateString($"{idPrefix}-hint");
                describedByParts.Add(hintId);
                var hintComponent = await GenerateHintAsync(options.Hint with { Id = hintId });

                innerHtmlBuilder.AppendHtml(hintComponent);
            }

            if (options.ErrorMessage is not null)
            {
                var errorId = new TemplateString($"{idPrefix}-error");
                describedByParts.Add(errorId);
                var errorMessageComponent = await GenerateErrorMessageAsync(options.ErrorMessage with { Id = errorId });

                innerHtmlBuilder.AppendHtml(errorMessageComponent);
            }

            var checkboxesDiv = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-checkboxes", options.Classes)
                .With("data-module", "govuk-checkboxes")
                .With(options.Attributes));

            if (options.FormGroup?.BeforeInputs is { } beforeInputs)
            {
                var beforeContent = HtmlOrText(beforeInputs.Html, beforeInputs.Text);
                if (!beforeContent.IsEmpty())
                {
                    checkboxesDiv.InnerHtml.AppendHtml(beforeContent);
                }
            }

            var itemIndex = 0;
            if (options.Items is not null)
            {
                foreach (var item in options.Items)
                {
                    if (item is null)
                    {
                        itemIndex++;
                        continue;
                    }

                    if (item.Divider is not null)
                    {
                        var divider = CreateDivider(item);
                        checkboxesDiv.InnerHtml.AppendHtml(divider);
                    }
                    else
                    {
                        var checkboxItem = await CreateCheckboxItemAsync(item, itemIndex, idPrefix, options.Name, hasFieldset, describedByParts, options.Values);
                        checkboxesDiv.InnerHtml.AppendHtml(checkboxItem);
                    }

                    itemIndex++;
                }
            }

            if (options.FormGroup?.AfterInputs is { } afterInputs)
            {
                var afterContent = HtmlOrText(afterInputs.Html, afterInputs.Text);
                if (!afterContent.IsEmpty())
                {
                    checkboxesDiv.InnerHtml.AppendHtml(afterContent);
                }
            }

            innerHtmlBuilder.AppendHtml(checkboxesDiv);
            return innerHtmlBuilder;
        }

        HtmlTag CreateDivider(CheckboxesOptionsItem item)
        {
            var divider = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-checkboxes__divider"))
            {
                item.Divider!
            };

            return divider;
        }

        async Task<IHtmlContent> CreateCheckboxItemAsync(
            CheckboxesOptionsItem item,
            int index,
            TemplateString? parentIdPrefix,
            TemplateString? parentName,
            bool inFieldset,
            List<TemplateString> parentDescribedByParts,
            IReadOnlyCollection<string>? values)
        {
            var itemId = item.Id ?? (index == 0 ? parentIdPrefix : new TemplateString($"{parentIdPrefix}-{index + 1}"));
            var itemName = item.Name ?? parentName;
            var conditionalId = new TemplateString($"conditional-{itemId}");

            // Check if item should be checked
            // If item.Checked is explicitly set, use that value
            // Otherwise, check if the value is in the values list
            var itemValueString = item.Value?.ToHtmlString() ?? string.Empty;
            var isChecked = item.Checked.HasValue 
                ? item.Checked.Value 
                : (values?.Contains(itemValueString) == true);

            // Only include conditional if Html is not empty/false/blank
            // Check matches liquid template's "!= blank" check
            var hasConditional = item.Conditional?.Html is not null 
                && !item.Conditional.Html.IsEmpty();

            var itemDiv = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-checkboxes__item"));

            var itemDescribedByParts = new List<TemplateString>();
            if (!inFieldset)
            {
                itemDescribedByParts.AddRange(parentDescribedByParts);
            }

            if (item.Hint is not null)
            {
                var itemHintId = new TemplateString($"{itemId}-item-hint");
                itemDescribedByParts.Add(itemHintId);
            }

            var input = new HtmlTag("input", attrs => attrs
                .WithClasses("govuk-checkboxes__input")
                .With("id", itemId)
                .With("name", itemName)
                .With("type", "checkbox")
                .With("value", item.Value)
                .WithBoolean("checked", isChecked)
                .WithBoolean("disabled", item.Disabled == true)
                .With("data-aria-controls", hasConditional ? conditionalId : null)
                .With("data-behaviour", item.Behaviour)
                .With("aria-describedby", itemDescribedByParts.Count > 0 ? TemplateString.Join(" ", itemDescribedByParts) : null)
                .With(item.Attributes));

            input.TagRenderMode = TagRenderMode.SelfClosing;

            itemDiv.InnerHtml.AppendHtml(input);

            var labelContent = HtmlOrText(item.Html, item.Text);
            var labelComponent = await GenerateLabelAsync(new LabelOptions
            {
                For = itemId,
                Classes = new TemplateString("govuk-checkboxes__label").AppendCssClasses(item.Label?.Classes),
                Attributes = item.Label?.Attributes,
                Html = labelContent.ToTemplateString()
            });

            itemDiv.InnerHtml.AppendHtml(labelComponent);

            if (item.Hint is not null)
            {
                var itemHintId = new TemplateString($"{itemId}-item-hint");
                var hintComponent = await GenerateHintAsync(new HintOptions
                {
                    Id = itemHintId,
                    Classes = new TemplateString("govuk-checkboxes__hint").AppendCssClasses(item.Hint.Classes),
                    Attributes = item.Hint.Attributes,
                    Html = item.Hint.Html,
                    Text = item.Hint.Text
                });

                itemDiv.InnerHtml.AppendHtml(hintComponent);
            }

            var result = new HtmlContentBuilder();
            result.AppendHtml(itemDiv);

            if (hasConditional)
            {
                var conditional = new HtmlTag("div", attrs => attrs
                    .WithClasses("govuk-checkboxes__conditional", !isChecked ? "govuk-checkboxes__conditional--hidden" : null)
                    .With("id", conditionalId))
                {
                    item.Conditional!.Html!.GetRawHtml()
                };

                result.AppendHtml(conditional);
            }

            return result;
        }
    }
}
