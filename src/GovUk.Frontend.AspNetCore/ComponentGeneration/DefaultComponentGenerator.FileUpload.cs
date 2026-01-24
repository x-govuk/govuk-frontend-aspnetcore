using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual async Task<GovUkComponent> GenerateFileUploadAsync(FileUploadOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var id = options.Id ?? options.Name;

        var describedByParts = new List<TemplateString>();
        if (options.DescribedBy is var describedBy && !describedBy.IsEmpty())
        {
            describedByParts.Add(describedBy);
        }

        var formGroupDiv = new HtmlTag("div", attrs => attrs
            .WithClasses("govuk-form-group", options.ErrorMessage is not null ? "govuk-form-group--error" : null, options.FormGroup?.Classes)
            .With(options.FormGroup?.Attributes));

        var innerContent = await BuildInnerContentAsync();
        formGroupDiv.InnerHtml.AppendHtml(innerContent);

        return await GenerateFromHtmlTagAsync(formGroupDiv);

        async Task<IHtmlContent> BuildInnerContentAsync()
        {
            var innerHtmlBuilder = new HtmlContentBuilder();

            if (options.Label is not null)
            {
                var labelComponent = await GenerateLabelAsync(options.Label with { For = id });
                innerHtmlBuilder.AppendHtml(labelComponent);
            }

            if (options.Hint is not null)
            {
                var hintId = new TemplateString($"{id}-hint");
                describedByParts.Add(hintId);
                var hintComponent = await GenerateHintAsync(options.Hint with { Id = hintId });

                innerHtmlBuilder.AppendHtml(hintComponent);
            }

            if (options.ErrorMessage is not null)
            {
                var errorId = new TemplateString($"{id}-error");
                describedByParts.Add(errorId);
                var errorMessageComponent = await GenerateErrorMessageAsync(options.ErrorMessage with { Id = errorId });

                innerHtmlBuilder.AppendHtml(errorMessageComponent);
            }

            if (options.FormGroup?.BeforeInput is { } beforeInput)
            {
                var beforeContent = HtmlOrText(beforeInput.Html, beforeInput.Text);
                if (!beforeContent.IsEmpty())
                {
                    innerHtmlBuilder.AppendHtml(beforeContent);
                }
            }

            if (options.JavaScript == true)
            {
                var dropZoneDiv = new HtmlTag("div", attrs => attrs
                    .WithClasses("govuk-drop-zone")
                    .With("data-module", "govuk-file-upload"));

                AddI18nAttribute(dropZoneDiv, "choose-files-button", options.ChooseFilesButtonText);
                AddI18nAttribute(dropZoneDiv, "no-file-chosen", options.NoFileChosenText);
                AddI18nAttribute(dropZoneDiv, "drop-instruction", options.DropInstructionText);
                AddI18nAttribute(dropZoneDiv, "entered-drop-zone", options.EnteredDropZoneText);
                AddI18nAttribute(dropZoneDiv, "left-drop-zone", options.LeftDropZoneText);

                if (options.MultipleFilesChosenText is { } multipleFilesChosenText)
                {
                    if (!multipleFilesChosenText.One.IsEmpty())
                    {
                        dropZoneDiv.Attributes.Set($"data-i18n.multiple-files-chosen.one", multipleFilesChosenText.One);
                    }
                    if (!multipleFilesChosenText.Other.IsEmpty())
                    {
                        dropZoneDiv.Attributes.Set($"data-i18n.multiple-files-chosen.other", multipleFilesChosenText.Other);
                    }
                }

                var inputTag = CreateFileInputTag();
                dropZoneDiv.InnerHtml.AppendHtml(inputTag);

                innerHtmlBuilder.AppendHtml(dropZoneDiv);
            }
            else
            {
                var inputTag = CreateFileInputTag();
                innerHtmlBuilder.AppendHtml(inputTag);
            }

            if (options.FormGroup?.AfterInput is { } afterInput)
            {
                var afterContent = HtmlOrText(afterInput.Html, afterInput.Text);
                if (!afterContent.IsEmpty())
                {
                    innerHtmlBuilder.AppendHtml(afterContent);
                }
            }

            return innerHtmlBuilder;
        }

        HtmlTag CreateFileInputTag()
        {
            var inputTag = new HtmlTag("input", attrs => attrs
                .With("type", "file")
                .With("id", id)
                .With("name", options.Name)
                .WithClasses("govuk-file-upload", options.Classes, options.ErrorMessage is not null ? "govuk-file-upload--error" : null)
                .WithBoolean("disabled", options.Disabled == true)
                .WithBoolean("multiple", options.Multiple == true)
                .With("value", options.Value)
                .With("aria-describedby", describedByParts.Count > 0 ? TemplateString.Join(" ", describedByParts) : null)
                .With(options.Attributes));

            inputTag.TagRenderMode = TagRenderMode.SelfClosing;

            return inputTag;
        }

        void AddI18nAttribute(HtmlTag tag, string key, TemplateString? value)
        {
            if (value is var v && !v.IsEmpty())
            {
                tag.Attributes.Set($"data-i18n.{key}", v);
            }
        }
    }
}
