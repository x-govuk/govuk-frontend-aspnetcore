using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using PuppeteerSharp;
using SoftCircuits.HtmlMonkey;

namespace GovUk.Frontend.AspNetCore.Docs;

public sealed class RazorSnippet(string markup, Stream screenshot) : IDisposable
{
    public string Markup { get; } = markup;

    public void Dispose() => screenshot.Dispose();

    public async Task<string> WriteScreenshotAsync(TemplatePublishOptions publishOptions, string path)
    {
        ArgumentNullException.ThrowIfNull(publishOptions);

        var relativePath = Path.Combine(publishOptions.DocsRepoRelativePath, "images", path);
        var fullPath = Path.GetFullPath(relativePath, publishOptions.RepoAbsolutePath);
        await using var fs = File.Create(fullPath);
        await screenshot.CopyToAsync(fs);

        return relativePath;
    }

    public string GetImgTag(string src, string alt)
    {
        ArgumentNullException.ThrowIfNull(src);
        ArgumentNullException.ThrowIfNull(alt);

        var tagBuilder = new TagBuilder("img") { TagRenderMode = TagRenderMode.SelfClosing };
        tagBuilder.Attributes.Add("src", src);
        tagBuilder.Attributes.Add("alt", alt);

        var writer = new StringWriter();
        tagBuilder.WriteTo(writer, HtmlEncoder.Default);
        return writer.ToString();
    }
}

public class RazorSnippetProvider
{
    private readonly IBrowser _browser;
    private readonly string _baseUrl;
    private readonly TemplatePublishOptions _publishOptions;

    public RazorSnippetProvider(TemplatePublishOptions publishOptions, IBrowser browser, string baseUrl)
    {
        ArgumentNullException.ThrowIfNull(publishOptions);
        ArgumentNullException.ThrowIfNull(browser);
        ArgumentNullException.ThrowIfNull(baseUrl);

        _publishOptions = publishOptions;
        _browser = browser;
        _baseUrl = baseUrl;
    }

    public async Task<RazorSnippet> CreateSnippetAsync(string path, string screenshotName)
    {
        if (screenshotName is null)
        {
            throw new ArgumentNullException(nameof(screenshotName));
        }

        ArgumentNullException.ThrowIfNull(path);

        var markupRelativePath = Path.Combine("Pages", path + ".cshtml");
        var markupFullPath = Path.GetFullPath(markupRelativePath);
        var markup = ExtractMarkupToCapture(await File.ReadAllTextAsync(markupFullPath), markupRelativePath);
        var (Contents, _, _, _) = await ScreenshotAsync(path, screenshotName);

        return new RazorSnippet(markup, Contents);
    }

    private static string ExtractMarkupToCapture(string markup, string hintName)
    {
        var doc = HtmlDocument.FromHtml(markup);
        var exampleElements = doc.Find("example").ToArray();

        if (exampleElements.Length == 0)
        {
            throw new InvalidOperationException($"No <example> element found in {hintName}.");
        }
        if (exampleElements.Length > 1)
        {
            throw new InvalidOperationException($"Multiple <example> elements found in {hintName}.");
        }

        var exampleElement = exampleElements.Single();
        return Dedent(exampleElement.InnerHtml);
    }

    private static string Dedent(string markup)
    {
        markup = markup.TrimStart(Environment.NewLine.ToCharArray());
        var indentLevel = markup.TakeWhile(c => c == ' ').Count();

        var output = new StringBuilder();

        var reader = new StringReader(markup);
        while (reader.ReadLine() is string line)
        {
            if (line.Length < indentLevel + 1)
            {
                Debug.Assert(line.Trim().Length == 0);
                output.AppendLine();
                continue;
            }

            output.AppendLine(line[indentLevel..]);
        }

        return output.ToString().TrimEnd();
    }

    private async Task<(Stream Contents, decimal Height, decimal Width, string Name)> ScreenshotAsync(string path, string name)
    {
        var fullPath = _baseUrl.TrimEnd('/') + "/" + path.TrimStart('/');

        await using var page = await _browser.NewPageAsync();
        var response = await page.GoToAsync(fullPath);

        if (!response.Ok)
        {
            throw new ArgumentException(
                $"Unsuccessful response ({(int)response.Status}) for '{path}'.",
                nameof(path));
        }

        var exampleElement = await page.WaitForSelectorAsync("example");
        var contents = await exampleElement.ScreenshotStreamAsync(new ElementScreenshotOptions());
        var boundingBox = await exampleElement.BoundingBoxAsync();

        return (contents, boundingBox.Height, boundingBox.Width, name);
    }
}
