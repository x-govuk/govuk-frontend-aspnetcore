using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.Testing.Tests;

/// <summary>
/// Renders components with the library's own generator, so the tests run over the markup apps actually get.
/// </summary>
internal static class Render
{
    public static DefaultComponentGenerator Generator { get; } = new();

    public static async Task<IHtmlDocument> DocumentAsync(params Func<DefaultComponentGenerator, ValueTask<GovUkComponent>>[] components)
    {
        var html = new List<string>();

        foreach (var component in components)
        {
            html.Add((await component(Generator)).GetHtml());
        }

        return Document(string.Concat(html));
    }

    public static IHtmlDocument Document(string bodyHtml) =>
        new HtmlParser().ParseDocument($"<!DOCTYPE html><html><head><title>Test</title></head><body>{bodyHtml}</body></html>");
}
