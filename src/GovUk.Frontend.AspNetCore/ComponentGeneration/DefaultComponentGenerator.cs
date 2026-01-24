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
        _parser = new FluidParser(new FluidParserOptions
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

    protected Task<GovUkComponent> EmptyComponentTask { get; } = Task.FromResult((GovUkComponent)EmptyComponent.Instance);

    public virtual Task<GovUkComponent> GenerateCharacterCountAsync(CharacterCountOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("character-count", options);
    }

    public virtual Task<GovUkComponent> GenerateFileUploadAsync(FileUploadOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return RenderTemplateAsync("file-upload", options);
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
        if (html?.IsEmpty() is false)
        {
            return html.GetRawHtml();
        }

        if (text?.IsEmpty() is false)
        {
            return text;
        }

        return new HtmlString(fallback);
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

    protected sealed class EmptyComponent : GovUkComponent
    {
        private readonly IHtmlContent _content = new HtmlString(string.Empty);

        private EmptyComponent() { }

        public static EmptyComponent Instance { get; } = new();

        public override void ApplyToTagHelper(TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(output);

            output.SuppressOutput();
        }

        public override IHtmlContent GetContent() => _content;
    }

    private static TemplateString Capitalize(TemplateString? input)
    {
        if (input.IsEmpty())
        {
            return TemplateString.Empty;
        }

        var encodedInput = input.ToHtmlString();

#pragma warning disable CA1308
        return TemplateString.FromEncoded(char.ToUpperInvariant(encodedInput[0]) + encodedInput[1..].ToLowerInvariant());
#pragma warning restore CA1308
    }

    private class HtmlTagGovUkComponent : GovUkComponent
    {
        public HtmlTagGovUkComponent(HtmlTag tag)
        {
            ArgumentNullException.ThrowIfNull(tag);

            Tag = tag;
        }

        public HtmlTag Tag { get; }

        public override IHtmlContent GetContent() => Tag;

        public override void ApplyToTagHelper(TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(output);

            var tagMode = Tag.TagRenderMode switch
            {
                TagRenderMode.StartTag => TagMode.StartTagOnly,
                TagRenderMode.SelfClosing => TagMode.SelfClosing,
                TagRenderMode.Normal => TagMode.StartTagAndEndTag,
                _ => throw new InvalidOperationException($"Cannot apply an HtmlTag with TagRenderMode '{Tag.TagRenderMode}' to a tag helper.")
            };

            output.TagName = Tag.TagName;
            output.TagMode = tagMode;

            output.Attributes.Clear();

            foreach (var attribute in Tag.Attributes.ToTagHelperAttributes())
            {
                output.Attributes.Add(attribute);
            }

            output.Content.AppendHtml(Tag.InnerHtml);
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

            TagHelperAdapter.ApplyComponentHtml(output, GetContent());
        }

        public override IHtmlContent GetContent() => new HtmlString(_html);
    }
}
