using System.Diagnostics;
using System.Text;
using Fluid;
using Fluid.Values;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Docs;

public class TemplateRenderer
{
    private readonly TemplatePublishOptions _publishOptions;
    private readonly FluidParser _parser;
    private readonly RazorSnippetProvider _razorSnippetProvider;
    private readonly TagHelperApiProvider _tagHelperApiProvider;

    public TemplateRenderer(
        TemplatePublishOptions publishOptions,
        RazorSnippetProvider razorSnippetProvider,
        TagHelperApiProvider tagHelperApiProvider)
    {
        ArgumentNullException.ThrowIfNull(publishOptions);
        ArgumentNullException.ThrowIfNull(razorSnippetProvider);
        ArgumentNullException.ThrowIfNull(tagHelperApiProvider);

        _publishOptions = publishOptions;
        _razorSnippetProvider = razorSnippetProvider;
        _tagHelperApiProvider = tagHelperApiProvider;

        _parser = new FluidParser(
            new FluidParserOptions()
            {
                AllowFunctions = true
            });
    }

    public async Task RenderTemplateAsync(string path)
    {
        var templatePath = Path.GetFullPath(Path.Combine("Templates", path));

        var relativePath = Path.Combine(_publishOptions.DocsRepoRelativePath, path.Replace(".liquid", ".md"));
        var fullPath = Path.GetFullPath(relativePath, _publishOptions.RepoAbsolutePath);

        await using var fs = File.CreateText(fullPath);
        await fs.WriteLineAsync($"<!-- Generated from {Path.GetRelativePath(_publishOptions.RepoAbsolutePath, templatePath).Replace("\\", "/")} -->");

        var source = await File.ReadAllTextAsync(templatePath);

        var template = _parser.Parse(source);

        var context = new TemplateContext();
        context.SetValue("tag_helper_example", new FunctionValue(TagHelperExampleFunction));
        context.SetValue("tag_helper_api", new FunctionValue(TagHelperApiFunction));
        context.SetValue("output", new StringValue(relativePath));

        await template.RenderAsync(fs, context);
    }

    private async ValueTask<FluidValue> TagHelperExampleFunction(FunctionArguments args, TemplateContext context)
    {
        var path = args.At(0).ToStringValue();
        var screenshotName = args.At(1).ToStringValue();
        var label = args.At(2).ToStringValue();

        var razorSnippet = await _razorSnippetProvider.CreateSnippetAsync(path, screenshotName);
        var screenshotRelativePath = await razorSnippet.WriteScreenshotAsync(_publishOptions, screenshotName);

        var templateOutput = context.GetValue("output").ToStringValue();
        var screenshotPathRelativeToTemplateOutput = Path.GetRelativePath(Path.GetDirectoryName(templateOutput)!, screenshotRelativePath);

        var sb = new StringBuilder();

        sb.AppendLine(razorSnippet.GetImgTag(screenshotPathRelativeToTemplateOutput, alt: label));

        sb.AppendLine();

        sb.AppendLine("```razor");
        sb.AppendLine(razorSnippet.Markup);
        sb.AppendLine("```");

        return FluidValue.Create(sb.ToString(), context.Options);
    }

    private FluidValue TagHelperApiFunction(FunctionArguments args, TemplateContext context)
    {
        var tagHelperName = args.At(0).ToStringValue();

        var tagHelperApi = _tagHelperApiProvider.GetTagHelperApi(tagHelperName);
        var contentDescription = tagHelperApi.ContentDescription;

        var sb = new StringBuilder();

        sb.Append($"#### `<{tagHelperApi.TagName}>`");
        sb.AppendLine();

        if (tagHelperApi.TagStructure == TagStructure.WithoutEndTag)
        {
            Debug.Assert(contentDescription is null);

            contentDescription =
                """
                > [!NOTE]
                > This tag helper should not have any child content.
                """;
        }

        if (!string.IsNullOrEmpty(contentDescription))
        {
            sb.AppendLine();
            sb.AppendLine(contentDescription);
        }

        if (tagHelperApi.ParentTagNames is not [])
        {
            sb.AppendLine();
            sb.AppendLine($"Must be inside a {string.Join(" or ", tagHelperApi.ParentTagNames.Select(t => $"`<{t}>`"))} element.");
        }

        if (tagHelperApi.Attributes.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("| Attribute | Type | Description |");
            sb.AppendLine("| --- | --- | --- |");

            foreach (var attribute in tagHelperApi.Attributes)
            {
                var formattedAttributeName = attribute.Name.StartsWith("(") ? attribute.Name : $"`{attribute.Name}`";
                var formattedType = !string.IsNullOrEmpty(attribute.Type) ? $"`{attribute.Type}`" : "";
                var formattedDescription = attribute.Description;

                sb.Append($"| {formattedAttributeName} | {formattedType} | {formattedDescription} |");
                sb.AppendLine();
            }
        }

        return FluidValue.Create(sb.ToString(), context.Options);
    }
}
