using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual Task<GovUkComponent> GenerateRadiosAsync(RadiosOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var idPrefix = options.IdPrefix ?? options.Name;
        var hasFieldset = options.Fieldset is not null;
        var describedBy = hasFieldset && options.Fieldset!.DescribedBy is not null 
            ? options.Fieldset.DescribedBy.ToString() 
            : string.Empty;

        // Build hint
        HtmlTag? hint = null;
        if (options.Hint is not null)
        {
            var hintId = idPrefix + "-hint";
            describedBy = AppendToDescribedBy(describedBy, hintId);
            hint = CreateHintTag(hintId, options.Hint);
        }

        // Build error message
        HtmlTag? errorMessage = null;
        if (options.ErrorMessage is not null)
        {
            var errorId = idPrefix + "-error";
            describedBy = AppendToDescribedBy(describedBy, errorId);
            errorMessage = CreateErrorMessageTag(errorId, options.ErrorMessage);
        }

        // Build radios container
        var radiosTag = new HtmlTag("div", attrs =>
        {
            attrs
                .WithClasses("govuk-radios", options.Classes)
                .With("data-module", "govuk-radios")
                .With(options.Attributes);
        });

        if (options.FormGroup?.BeforeInputs is not null)
        {
            radiosTag.Add(GetHtmlContent(options.FormGroup.BeforeInputs.Html, options.FormGroup.BeforeInputs.Text));
        }

        // Build items
        var itemIndex = 1;
        if (options.Items is not null)
        {
            foreach (var item in options.Items)
            {
                if (item.Divider is not null)
                {
                    var divider = new HtmlTag("div", attrs => attrs.WithClasses("govuk-radios__divider"));
                    divider.Add(item.Divider.ToString()!);
                    radiosTag.Add(divider);
                }
                else
                {
                    var itemId = item.Id ?? (itemIndex == 1 ? idPrefix : $"{idPrefix}-{itemIndex}");
                    var conditionalId = $"conditional-{itemId}";
                    var isChecked = item.Checked ?? (item.Value == options.Value && item.Checked != false);
                    var hasHint = item.Hint?.Text is not null || item.Hint?.Html is not null;
                    var itemHintId = hasHint ? $"{itemId}-item-hint" : null;

                    var itemTag = new HtmlTag("div", attrs => attrs.WithClasses("govuk-radios__item"));

                    // Input
                    var input = new HtmlTag("input", attrs =>
                    {
                        attrs
                            .WithClasses("govuk-radios__input")
                            .With("id", itemId)
                            .With("name", options.Name)
                            .With("type", "radio")
                            .With("value", item.Value);

                        if (isChecked == true)
                        {
                            attrs.WithBoolean("checked");
                        }

                        if (item.Disabled == true)
                        {
                            attrs.WithBoolean("disabled");
                        }

                        if (item.Conditional?.Html is not null)
                        {
                            attrs.With("data-aria-controls", conditionalId);
                        }

                        if (itemHintId is not null)
                        {
                            attrs.With("aria-describedby", itemHintId);
                        }

                        attrs.With(item.Attributes);
                    });
                    input.TagRenderMode = TagRenderMode.SelfClosing;
                    itemTag.Add(input);

                    // Label
                    var label = new HtmlTag("label", attrs =>
                    {
                        attrs
                            .WithClasses("govuk-radios__label", item.Label?.Classes)
                            .With("for", itemId)
                            .With(item.Label?.Attributes);
                    });
                    label.Add(GetHtmlContent(item.Html ?? item.Label?.Html, item.Text ?? item.Label?.Text));
                    itemTag.Add(label);

                    // Item hint
                    if (hasHint)
                    {
                        var itemHint = new HtmlTag("div", attrs =>
                        {
                            attrs
                                .WithClasses("govuk-radios__hint", item.Hint!.Classes)
                                .With("id", itemHintId)
                                .With(item.Hint.Attributes);
                        });
                        itemHint.Add(GetHtmlContent(item.Hint!.Html, item.Hint.Text));
                        itemTag.Add(itemHint);
                    }

                    radiosTag.Add(itemTag);

                    // Conditional
                    if (item.Conditional?.Html is not null)
                    {
                        var conditional = new HtmlTag("div", attrs =>
                        {
                            attrs
                                .WithClasses("govuk-radios__conditional")
                                .With("id", conditionalId);

                            if (isChecked != true)
                            {
                                attrs.WithClasses("govuk-radios__conditional--hidden");
                            }
                        });
                        conditional.Add(new HtmlString(item.Conditional.Html.ToString()!));
                        radiosTag.Add(conditional);
                    }
                }

                itemIndex++;
            }
        }

        if (options.FormGroup?.AfterInputs is not null)
        {
            radiosTag.Add(GetHtmlContent(options.FormGroup.AfterInputs.Html, options.FormGroup.AfterInputs.Text));
        }

        // Build inner HTML (hint + error + radios)
        var innerHtml = new HtmlContentBuilder();
        if (hint is not null)
        {
            innerHtml.AppendHtml(hint);
        }
        if (errorMessage is not null)
        {
            innerHtml.AppendHtml(errorMessage);
        }
        innerHtml.AppendHtml(radiosTag);

        // Build form group
        var formGroupTag = new HtmlTag("div", attrs =>
        {
            attrs
                .WithClasses("govuk-form-group")
                .With(options.FormGroup?.Attributes);

            if (errorMessage is not null)
            {
                attrs.WithClasses("govuk-form-group--error");
            }

            if (options.FormGroup?.Classes is not null)
            {
                attrs.WithClasses(options.FormGroup.Classes);
            }
        });

        // Wrap in fieldset if needed
        if (hasFieldset)
        {
            var fieldsetTag = CreateFieldsetTag(describedBy, options.Fieldset!, innerHtml);
            formGroupTag.Add(fieldsetTag);
        }
        else
        {
            formGroupTag.Add(innerHtml);
        }

        return GenerateFromHtmlTagAsync(formGroupTag);
    }

    private static string AppendToDescribedBy(string? describedBy, string value)
    {
        return string.IsNullOrWhiteSpace(describedBy) ? value : $"{describedBy} {value}";
    }

    private HtmlTag CreateHintTag(string? id, HintOptions hint)
    {
        var hintTag = new HtmlTag("div", attrs =>
        {
            attrs
                .WithClasses("govuk-hint", hint.Classes)
                .With("id", id)
                .With(hint.Attributes);
        });
        hintTag.Add(GetHtmlContent(hint.Html, hint.Text));
        return hintTag;
    }

    private HtmlTag CreateErrorMessageTag(string? id, ErrorMessageOptions error)
    {
        var errorTag = new HtmlTag("p", attrs =>
        {
            attrs
                .WithClasses("govuk-error-message", error.Classes)
                .With("id", id)
                .With(error.Attributes);
        });

        var visuallyHiddenText = error.VisuallyHiddenText ?? "Error";
        var span = new HtmlTag("span", attrs => attrs.WithClasses("govuk-visually-hidden"));
        span.Add(visuallyHiddenText.ToString()!);
        errorTag.Add(span);
        errorTag.Add(GetHtmlContent(error.Html, error.Text));
        
        return errorTag;
    }

    private HtmlTag CreateFieldsetTag(string? describedBy, FieldsetOptions fieldset, IHtmlContent content)
    {
        var fieldsetTag = new HtmlTag("fieldset", attrs =>
        {
            attrs
                .WithClasses("govuk-fieldset", fieldset.Classes)
                .With("aria-describedby", describedBy)
                .With("role", fieldset.Role)
                .With(fieldset.Attributes);
        });

        if (fieldset.Legend is not null)
        {
            var legendTag = new HtmlTag("legend", attrs =>
            {
                attrs
                    .WithClasses("govuk-fieldset__legend", fieldset.Legend.Classes)
                    .With(fieldset.Legend.Attributes);
            });

            if (fieldset.Legend.IsPageHeading == true)
            {
                var h1 = new HtmlTag("h1", attrs => attrs.WithClasses("govuk-fieldset__heading"));
                h1.Add(GetHtmlContent(fieldset.Legend.Html, fieldset.Legend.Text));
                legendTag.Add(h1);
            }
            else
            {
                legendTag.Add(GetHtmlContent(fieldset.Legend.Html, fieldset.Legend.Text));
            }

            fieldsetTag.Add(legendTag);
        }

        fieldsetTag.Add(content);
        return fieldsetTag;
    }

    private static IHtmlContent GetHtmlContent(TemplateString? html, string? text)
    {
        if (html is not null)
        {
            return new HtmlString(html.ToString()!);
        }
        if (text is not null)
        {
            return new HtmlString(HtmlEncoder.Default.Encode(text));
        }
        return HtmlString.Empty;
    }
}
