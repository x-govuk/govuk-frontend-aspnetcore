using Microsoft.AspNetCore.Html;
using System.Text.Encodings.Web;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual Task<GovUkComponent> GenerateSelectAsync(SelectOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var id = options.Id ?? options.Name;
        var describedBy = options.DescribedBy?.ToString() ?? string.Empty;

        // Build label
        HtmlTag? label = null;
        if (options.Label is not null)
        {
            label = new HtmlTag("label", attrs =>
            {
                attrs
                    .WithClasses("govuk-label", options.Label.Classes)
                    .With("for", id)
                    .With(options.Label.Attributes);
            });

            if (options.Label.IsPageHeading == true)
            {
                var h1 = new HtmlTag("h1", attrs => attrs.WithClasses("govuk-label-wrapper"));
                var innerLabel = GetHtmlContent(options.Label.Html, options.Label.Text);
                h1.Add(innerLabel);
                label.Add(h1);
            }
            else
            {
                label.Add(GetHtmlContent(options.Label.Html, options.Label.Text));
            }
        }

        // Build hint
        HtmlTag? hint = null;
        if (options.Hint is not null)
        {
            var hintId = id + "-hint";
            describedBy = AppendToDescribedBy(describedBy, hintId);
            hint = CreateHintTag(hintId, options.Hint);
        }

        // Build error message
        HtmlTag? errorMessage = null;
        if (options.ErrorMessage is not null)
        {
            var errorId = id + "-error";
            describedBy = AppendToDescribedBy(describedBy, errorId);
            errorMessage = CreateErrorMessageTag(errorId, options.ErrorMessage);
        }

        // Build select element
        var selectTag = new HtmlTag("select", attrs =>
        {
            attrs
                .WithClasses("govuk-select", options.Classes)
                .With("id", id)
                .With("name", options.Name);

            if (errorMessage is not null)
            {
                attrs.WithClasses("govuk-select--error");
            }

            if (!string.IsNullOrWhiteSpace(describedBy))
            {
                attrs.With("aria-describedby", describedBy);
            }

            if (options.Attributes is not null && options.Attributes["disabled"] is not null)
            {
                attrs.WithBoolean("disabled");
            }

            attrs.With(options.Attributes);
        });

        // Add options
        if (options.Items is not null)
        {
            foreach (var item in options.Items)
            {
                var effectiveValue = item.Value ?? new HtmlString(item.Text ?? string.Empty);
                var isSelected = item.Selected ?? (effectiveValue?.ToString() == options.Value?.ToString() && item.Selected != false);

                var option = new HtmlTag("option", attrs =>
                {
                    if (item.Value is not null)
                    {
                        attrs.With("value", item.Value.ToString());
                    }

                    if (isSelected == true)
                    {
                        attrs.WithBoolean("selected");
                    }

                    if (item.Disabled == true)
                    {
                        attrs.WithBoolean("disabled");
                    }

                    attrs.With(item.Attributes);
                });

                option.Add(item.Text ?? string.Empty);
                selectTag.Add(option);
            }
        }

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

        if (label is not null)
        {
            formGroupTag.Add(label);
        }

        if (hint is not null)
        {
            formGroupTag.Add(hint);
        }

        if (errorMessage is not null)
        {
            formGroupTag.Add(errorMessage);
        }

        if (options.FormGroup?.BeforeInput is not null)
        {
            formGroupTag.Add(GetHtmlContent(options.FormGroup.BeforeInput.Html, options.FormGroup.BeforeInput.Text));
        }

        formGroupTag.Add(selectTag);

        if (options.FormGroup?.AfterInput is not null)
        {
            formGroupTag.Add(GetHtmlContent(options.FormGroup.AfterInput.Html, options.FormGroup.AfterInput.Text));
        }

        return GenerateFromHtmlTagAsync(formGroupTag);
    }
}
