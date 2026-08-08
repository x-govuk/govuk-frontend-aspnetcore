using System.Net;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace GovUk.Frontend.AspNetCore.PackageTests.Infrastructure;

/// <summary>
/// The URLs the library's page template advertises for the files the targets restored.
/// </summary>
public sealed record HostedPage(
    string Stylesheet,
    string Script,
    string FavIcon,
    string Manifest,
    string CspScriptHashes,
    string InlineInitScript)
{
    public static async Task<HostedPage> GetAsync(FixtureApp app, string path = "/")
    {
        using var response = await app.Client.GetAsync(path);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            Assert.Fail($"GET {path} returned {(int)response.StatusCode}.{Environment.NewLine}{await response.Content.ReadAsStringAsync()}");
        }

        await using var content = await response.Content.ReadAsStreamAsync();
        var document = (IHtmlDocument)await new HtmlParser().ParseDocumentAsync(content);

        // The page template emits the import as a module with a src and the initAll call as an inline
        // module alongside it.
        var inlineInitScript = document.QuerySelector("script[type=module]:not([src])")?.TextContent ??
            throw new InvalidOperationException(
                $"No inline module script found.{Environment.NewLine}{document.DocumentElement.OuterHtml}");

        return new HostedPage(
            Attribute(document, "link[rel=stylesheet]", "href"),
            Attribute(document, "script[type=module][src]", "src"),
            Attribute(document, "link[rel=icon][sizes='48x48']", "href"),
            Attribute(document, "link[rel=manifest]", "href"),
            Attribute(document, "meta[name=csp-script-hashes]", "content"),
            inlineInitScript);

        static string Attribute(IHtmlDocument document, string selector, string attributeName)
        {
            var element = document.QuerySelector(selector) ??
                throw new InvalidOperationException($"No element matched '{selector}'.{Environment.NewLine}{document.DocumentElement.OuterHtml}");

            return element.GetAttribute(attributeName) ??
                throw new InvalidOperationException($"'{selector}' has no '{attributeName}' attribute.");
        }
    }
}
