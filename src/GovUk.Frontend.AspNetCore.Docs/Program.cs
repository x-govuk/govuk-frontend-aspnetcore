using System.Reflection;
using GovUk.Frontend.AspNetCore;
using GovUk.Frontend.AspNetCore.Docs;
using PuppeteerSharp;

var baseUrl = "http://localhost:5111";

var publish = args is ["publish", ..];

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls(baseUrl);

if (publish)
{
    builder.Logging.SetMinimumLevel(LogLevel.Warning);
}

builder.Services.AddGovUkFrontend();
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseDeveloperExceptionPage();

app.UseGovUkFrontend();

app.UseRouting();

app.MapRazorPages();

using var cts = new CancellationTokenSource();
var webAppTask = app.RunAsync(cts.Token);

if (publish)
{
    try
    {
        await PublishDocsAsync();
    }
    finally
    {
        cts.Cancel();
    }
}

await webAppTask;

async Task PublishDocsAsync()
{
    string GetAssemblyMetadataAttributeValue(string name)
    {
        var attrValue = typeof(Program).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .SingleOrDefault(a => a.Key == name)?
            .Value;

        if (attrValue is null)
        {
            throw new Exception($"{name} attribute not defined.");
        }

        return attrValue;
    }

    var repoRoot = GetAssemblyMetadataAttributeValue("RepoRoot");
    var docsBase = GetAssemblyMetadataAttributeValue("DocsBase");
    var publishOptions = new TemplatePublishOptions(repoRoot, docsBase);

    var browserFetcher = new BrowserFetcher();
    await browserFetcher.DownloadAsync();
    using var browser = await Puppeteer.LaunchAsync(new LaunchOptions()
    {
        Headless = true,
        Args = ["--no-sandbox", "--disable-setuid-sandbox"],
        DefaultViewport = new ViewPortOptions()
        {
            DeviceScaleFactor = 2,
            Height = 768,
            Width = 1024
        }
    });

    var snippetProvider = new RazorSnippetProvider(publishOptions, browser, baseUrl);
    var tagHelperApiProvider = new TagHelperApiProvider(publishOptions);
    var renderer = new TemplateRenderer(publishOptions, snippetProvider, tagHelperApiProvider);

    foreach (var template in Directory.GetFiles("Templates/components"))
    {
        await renderer.RenderTemplateAsync(Path.GetRelativePath("Templates", template));
    }
}

public record TemplatePublishOptions(string RepoAbsolutePath, string DocsRepoRelativePath);
