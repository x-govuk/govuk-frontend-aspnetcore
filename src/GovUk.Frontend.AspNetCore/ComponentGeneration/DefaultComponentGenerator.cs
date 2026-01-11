using System.Collections.Concurrent;
using System.Text.Encodings.Web;
using Fluid;
using Fluid.Values;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.FileProviders;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator : IComponentGenerator
{
    internal const string DefaultErrorSummaryTitleHtml = "There is a problem";

    private static readonly HtmlEncoder _encoder = HtmlEncoder.Default;

    private readonly FluidParser _parser;
    private readonly ConcurrentDictionary<string, IFluidTemplate> _templates;
    private readonly TemplateOptions _templateOptions;

    public DefaultComponentGenerator()
    {
        _parser = new FluidParser(new FluidParserOptions()
        {
            AllowFunctions = true,
            AllowParentheses = true
        });

        var optionsJsonSerializerOptions = ComponentOptionsJsonSerializerOptions.Instance;

        _templates = new ConcurrentDictionary<string, IFluidTemplate>();

        _templateOptions = new TemplateOptions
        {
            MemberAccessStrategy = new UnsafeMemberAccessStrategy(),
            Trimming = TrimmingFlags.TagLeft,

            FileProvider = new ManifestEmbeddedFileProvider(
            typeof(GovUkFrontendExtensions).Assembly,
            root: "ComponentGeneration/Templates")
        };

        _templateOptions.Filters.AddFilter("nj_default", Filters.DefaultAsync);
        _templateOptions.Filters.AddFilter("indent", Filters.IndentAsync);
        _templateOptions.Filters.AddFilter("strip", Filters.StripAsync);

        _templateOptions.ValueConverters.Add(v =>
        {
            if (v is TemplateString templateString)
            {
                return templateString.ToFluidValue(_encoder);
            }

            // If the object is an Options class, convert its property names to camel case
            return v.GetType().Namespace?.StartsWith(GetType().Namespace!, StringComparison.Ordinal) == true && !v.GetType().IsArray
                ? new ConvertNamesFromJsonTypeInfoObjectValue(v, optionsJsonSerializerOptions)
                : (object?)null;
        });
    }

    public virtual Task<GovUkComponent> GenerateCharacterCountAsync(CharacterCountOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("character-count", options);
    }

    public virtual Task<GovUkComponent> GenerateCheckboxesAsync(CheckboxesOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("checkboxes", options);
    }

    public virtual Task<GovUkComponent> GenerateCookieBannerAsync(CookieBannerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("cookie-banner", options);
    }

    public virtual Task<GovUkComponent> GenerateDateInputAsync(DateInputOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("date-input", options);
    }

    public virtual Task<GovUkComponent> GenerateDetailsAsync(DetailsOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("details", options);
    }



    public virtual Task<GovUkComponent> GenerateErrorSummaryAsync(ErrorSummaryOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("error-summary", options);
    }

    public virtual Task<GovUkComponent> GenerateExitThisPageAsync(ExitThisPageOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("exit-this-page", options);
    }



    public virtual Task<GovUkComponent> GenerateFileUploadAsync(FileUploadOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("file-upload", options);
    }

    public virtual Task<GovUkComponent> GenerateFooterAsync(FooterOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("footer", options);
    }

    public virtual Task<GovUkComponent> GenerateHeaderAsync(HeaderOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("header", options);
    }



    public virtual Task<GovUkComponent> GenerateInsetTextAsync(InsetTextOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("inset-text", options);
    }

    public virtual Task<GovUkComponent> GenerateInputAsync(InputOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("input", options);
    }



    public virtual Task<GovUkComponent> GenerateNotificationBannerAsync(NotificationBannerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("notification-banner", options);
    }

    public virtual Task<GovUkComponent> GenerateServiceNavigationAsync(ServiceNavigationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("service-navigation", options);
    }

    public virtual Task<GovUkComponent> GeneratePanelAsync(PanelOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("panel", options);
    }

    public virtual Task<GovUkComponent> GeneratePhaseBannerAsync(PhaseBannerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("phase-banner", options);
    }

    public virtual Task<GovUkComponent> GeneratePaginationAsync(PaginationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("pagination", options);
    }

    public virtual Task<GovUkComponent> GeneratePasswordInputAsync(PasswordInputOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("password-input", options);
    }

    public virtual Task<GovUkComponent> GenerateSkipLinkAsync(SkipLinkOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("skip-link", options);
    }

    public virtual Task<GovUkComponent> GenerateSummaryListAsync(SummaryListOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("summary-list", options);
    }

    public virtual Task<GovUkComponent> GenerateTableAsync(TableOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("table", options);
    }

    public virtual Task<GovUkComponent> GenerateTabsAsync(TabsOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("tabs", options);
    }

    public virtual Task<GovUkComponent> GenerateTagAsync(TagOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("tag", options);
    }

    public virtual Task<GovUkComponent> GenerateTaskListAsync(TaskListOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("task-list", options);
    }

    public virtual Task<GovUkComponent> GenerateTextareaAsync(TextareaOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("textarea", options);
    }

    public virtual Task<GovUkComponent> GenerateWarningTextAsync(WarningTextOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("warning-text", options);
    }

    private Task<GovUkComponent> GenerateFromHtmlTagAsync(HtmlTag tag) =>
        Task.FromResult((GovUkComponent)new HtmlTagGovUkComponent(tag));

    private IFluidTemplate GetTemplate(string templateName) =>
        _templates.GetOrAdd(templateName, _ =>
        {
            var templateFileInfo = _templateOptions.FileProvider.GetFileInfo($"{templateName}.liquid");
            if (!templateFileInfo.Exists)
            {
                throw new ArgumentException($"Template '{templateName}' not found.", nameof(templateName));
            }

            using var sourceStream = templateFileInfo.CreateReadStream();
            using var reader = new StreamReader(sourceStream);
            var source = reader.ReadToEnd();

            var template = _parser.Parse(source);

            return template;
        });

    private IHtmlContent HtmlOrText(TemplateString? html, TemplateString? text, string? fallback = null)
    {
        if (html is not null)
        {
            return new HtmlString(html.ToHtmlString(raw: true));
        }

        if (text is not null)
        {
            return text;
        }

        if (fallback is not null)
        {
            return new HtmlString(fallback);
        }

        return HtmlString.Empty;
    }

    private async Task<GovUkComponent> RenderTemplateAsync(string templateName, object componentOptions)
    {
        var context = new TemplateContext(_templateOptions);
        context.SetValue("array", new FunctionValue(Functions.Array));
        context.SetValue("dict", new FunctionValue(Functions.Dict));
        context.SetValue("govukAttributes", new FunctionValue(Functions.GovukAttributesAsync));
        context.SetValue("govukI18nAttributes", new FunctionValue(Functions.GovukI18nAttributes));
        context.SetValue("ifelse", new FunctionValue(Functions.IfElse));
        context.SetValue("istruthy", new FunctionValue(Functions.IsTruthy));
        context.SetValue("not", new FunctionValue(Functions.Not));
        context.SetValue("string", new FunctionValue(Functions.String));
        context.SetValue("params", componentOptions);  // To match the nunjucks templates

        var template = GetTemplate(templateName);
        var result = await template.RenderAsync(context, _encoder);
        return new FluidTemplateGovUkComponent(result.TrimStart());
    }

    private class HtmlTagGovUkComponent : GovUkComponent
    {
        private readonly HtmlTag _tag;

        public HtmlTagGovUkComponent(HtmlTag tag)
        {
            ArgumentNullException.ThrowIfNull(tag);

            _tag = tag;
        }

        public override string GetHtml() => _tag.ToHtmlString(HtmlEncoder.Default);

        public override void ApplyToTagHelper(TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(output);

            var tagMode = _tag.TagRenderMode switch
            {
                TagRenderMode.StartTag => TagMode.StartTagOnly,
                TagRenderMode.SelfClosing => TagMode.SelfClosing,
                TagRenderMode.Normal => TagMode.StartTagAndEndTag,
                _ => throw new InvalidOperationException($"Cannot apply an HtmlTag with TagRenderMode '{_tag.TagRenderMode}' to a tag helper.")
            };

            output.TagName = _tag.TagName;
            output.TagMode = tagMode;

            output.Attributes.Clear();

            foreach (var attribute in _tag.Attributes.ToTagHelperAttributes())
            {
                output.Attributes.Add(attribute);
            }

            output.Content.AppendHtml(_tag.InnerHtml);
        }
    }

    private class FluidTemplateGovUkComponent : GovUkComponent
    {
        private readonly string _html;

        public FluidTemplateGovUkComponent(string html)
        {
            ArgumentNullException.ThrowIfNull(html);

            _html = html;
        }

        public override void ApplyToTagHelper(TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(output);

            TagHelperAdapter.ApplyComponentHtml(output, GetHtml());
        }

        public override string GetHtml() => _html;
    }
}
